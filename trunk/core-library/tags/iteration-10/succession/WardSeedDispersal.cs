using Edu.Wisc.Forest.Flel.Util;

using Landis.Landscape;
using Landis.Species;
using Landis.Util;

using System;
using System.Collections.Generic;

using log4net;

namespace Landis.Succession
{
	/// <summary>
	/// Brendan Ward's seed dispersal algorithm.
	/// </summary>
	public static class WardSeedDispersal
	{
		public struct NeighborInfo
		{
			public RelativeLocation RelativeLocation;
			public double DistanceProbability;

			public NeighborInfo(int    rowOffset,
			                    int    columnOffset,
			                    double probability)
			{
				this.RelativeLocation = new RelativeLocation(rowOffset, columnOffset);
				this.DistanceProbability = probability;
			}
		}

		//---------------------------------------------------------------------

		private static List<NeighborInfo>[] neighborhoods;
		private static ILog logger;

		//---------------------------------------------------------------------

		public class ProbabilityComputer
		{
			private double effDist;
			private double maxDist;
			private const double ratio = 0.95f;
			private double lambda1;
			private double lambda2;

			public ProbabilityComputer(ISpecies species)
			{
				effDist = species.EffectiveSeedDist;
				maxDist = species.MaxSeedDist;
				lambda1 = Math.Log((1-ratio)/effDist);
				lambda2 = Math.Log(0.01)/maxDist;
			}

			public double Compute(double distance)
			{
				double lowBound = distance - Model.CellLength;
				if (lowBound < 0)
					lowBound = 0;
				double upBound = distance;

				if (Model.CellLength <= effDist) {
					//  BCW: Draw probabilities from either EffD or MaxD curves
					if (distance <= effDist)
						return Math.Exp(lambda1*lowBound) - Math.Exp(lambda1*upBound);
					else
						return (1-ratio)*(Math.Exp(lambda2*(lowBound-effDist)) - Math.Exp(lambda2*(upBound-effDist)));
				}
				else {
					if (distance <= effDist)
						//  BCW: Draw probabilities from both EffD and MaxD curves
						return Math.Exp(lambda1*lowBound) - (1-ratio)*Math.Exp(lambda2*(upBound-effDist));
					else
						return (1-ratio)*(Math.Exp(lambda2*(lowBound-effDist)) - Math.Exp(lambda2*(upBound-effDist)));
				}
			}
		}

		//---------------------------------------------------------------------

		static WardSeedDispersal()
		{
			logger = LogManager.GetLogger(typeof(WardSeedDispersal));
			timer = new QueryPerfCounter();

			//  Initialize neighborhoods for each species
			neighborhoods = new List<NeighborInfo>[Model.Species.Count];
			foreach (ISpecies species in Model.Species) {
				List<NeighborInfo> neighborhood = new List<NeighborInfo>();
				neighborhoods[species.Index] = neighborhood;

				ProbabilityComputer probabilityComputer = new ProbabilityComputer(species);

				//	Maximum seeding distance in units of cells
				int maxSeedDist = (int) Math.Ceiling(species.MaxSeedDist / Model.CellLength);

				//	Neighborhood defined by maximum seeding distance.
				//	Start at the center of the top row in the neighborhood,
				//	and move left one cell at a time until the cell's distance
				//	to the center of neighborhood > maximum distance.  Then
				//  go down 1 row, and repeat the process starting from the
				//  center.  Keep moving down a row until the row with the
				//	neighborhood's center cell.  In essence, scanning the upper
				//  left quadrant of the neighborhood.
				for (int rowOffset = -maxSeedDist; rowOffset <= 0; rowOffset++) {
					int startColumnOffset = 0;
					if (rowOffset == 0)
						// Skip center cell of neighborhood
						startColumnOffset = -1;
					double distance;
					for (int columnOffset = startColumnOffset;
					     	(distance = ComputeDistance(rowOffset, columnOffset)) <= species.MaxSeedDist;
					     	--columnOffset) {
						double probability = probabilityComputer.Compute(distance);
						neighborhood.Add(new NeighborInfo(rowOffset, columnOffset, probability));

						//  If not center cell in row, then add the mirror cell
						//  in the upper right quadrant on the same row.
						if (columnOffset != 0)
							neighborhood.Add(new NeighborInfo(rowOffset, -columnOffset, probability));

						//  If not the center row in the neighborhood, then add
						//	the mirror cell(s) in the lower half of the
						//	neighborhood.
						if (rowOffset != 0) {
							if (columnOffset == 0)
								//  Center cell in the row, so just add the
								//	center cell in the mirror row in the lower
								//  lower half of neighborhood.
								neighborhood.Add(new NeighborInfo(-rowOffset, 0, probability));
							else {
								//	Add mirror cells in lower left and lower
								//	right quadrants.
								neighborhood.Add(new NeighborInfo(-rowOffset, columnOffset, probability));
								neighborhood.Add(new NeighborInfo(-rowOffset, -columnOffset, probability));
							}
						}
					}
				}

				if (logger.IsDebugEnabled) {
					logger.Debug(string.Format("Neighborhood for {0}: {1} cells",
					                           species.Name, neighborhood.Count));

					if (neighborhood.Count <= 100)
						foreach (NeighborInfo neighborInfo in neighborhood)
							logger.Debug(string.Format("  {0} : probability = {1}",
							                           neighborInfo.RelativeLocation,
							                           neighborInfo.DistanceProbability));
				}

				neighborhood.Sort(CompareProbabilities);
				if (logger.IsDebugEnabled) {
					logger.Debug("  Neighborhood sorted.");
				}
			}
		}

		//---------------------------------------------------------------------

		public static int CompareProbabilities(NeighborInfo x,
		                                       NeighborInfo y)
		{
			if (x.DistanceProbability > y.DistanceProbability)
				return -1;
			if (x.DistanceProbability < y.DistanceProbability)
				return 1;
			return 0;
		}

		//---------------------------------------------------------------------

		public static IEnumerable<NeighborInfo> GetNeighborhood(ISpecies species)
		{
			if (species == null)
				return null;
			return neighborhoods[species.Index];
		}

		//---------------------------------------------------------------------

		public static double ComputeDistance(int rowOffset,
		                                     int columnOffset)
		{
			return Model.CellLength * (Math.Sqrt(rowOffset*rowOffset + columnOffset*columnOffset) - 1);
		}

		//---------------------------------------------------------------------

		private static QueryPerfCounter timer;
		private static MutableSite neighbor;

		//---------------------------------------------------------------------

		public static bool Algorithm(ISpecies   species,
		                             ActiveSite site)
		{
			if (! Reproduction.SufficientLight(species, site) ||
			    ! Reproduction.Establish(species, site))
				return false;

			if (Reproduction.MaturePresent(species, site))
				return true;

#if COUNT_SITES_SEARCHED
			timer.Start();

			int offLandscapeCount = 0;
			int activeCount = 0;
			int inactiveCount = 0;
#endif

			//	Use "for" loop instead of "foreach" to avoid temporary object
			//	for enumerator.
			bool found = false;
			List<NeighborInfo> neighborhood = neighborhoods[species.Index];
			int neighborCount = neighborhood.Count;
			for (int i = 0; i < neighborCount; i++) {
				if (! site.GetNeighbor(neighborhood[i].RelativeLocation, ref neighbor)) {
#if COUNT_SITES_SEARCHED
					offLandscapeCount++;
#endif
					continue;
				}
				if (neighbor.IsActive) {
#if COUNT_SITES_SEARCHED
					activeCount++;
#endif
					if (Reproduction.MaturePresent(species, neighbor) &&
					    	Util.Random.GenerateUniform() < neighborhood[i].DistanceProbability) {
						found = true;
						break;
					}
				}
				else {
#if COUNT_SITES_SEARCHED
					inactiveCount++;
#endif
				}
			}
#if COUNT_SITES_SEARCHED
			timer.Stop();
			if (logger.IsDebugEnabled) {
				logger.Debug(string.Format("{0} Search for {1} {2}",
				                           site.Location, species.Name,
				                           (found ? "succeeded" : "failed")));
				int totalCount = activeCount + inactiveCount + offLandscapeCount;
				logger.Debug(string.Format("  Sites: {0} total, {1} active, {2} inactive, {3} off-grid",
				                           totalCount, activeCount, inactiveCount, offLandscapeCount));
				logger.Debug(string.Format("  {0} milliseconds",
				                           timer.Duration(1) / 1000000));
			}
#endif

			return found;
		}
	}
}
