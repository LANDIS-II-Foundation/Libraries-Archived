using Landis.Landscape;
using Landis.Species;
using Landis.Util;

using System;
using System.Collections.Generic;

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

				Console.WriteLine("Neighborhood for {0}: {1} cells",
				                  species.Name, neighborhood.Count);
#if PRINT_NEIGHBORHOOD
				Console.WriteLine();
				Console.WriteLine("Neighborhood for {0}", species.Name);
				foreach (NeighborInfo neighborInfo in neighborhood)
					Console.WriteLine("  {0} : probability = {1}",
					                  neighborInfo.RelativeLocation,
					                  neighborInfo.DistanceProbability);
#endif
			}
		}

		//---------------------------------------------------------------------

		public static double ComputeDistance(int rowOffset,
		                                     int columnOffset)
		{
			return Model.CellLength * (Math.Sqrt(rowOffset*rowOffset + columnOffset*columnOffset) - 1);
		}

		//---------------------------------------------------------------------

		public static bool Algorithm(ISpecies   species,
		                             ActiveSite site)
		{
			if (! Reproduction.SufficientLight(species, site) ||
			    ! Reproduction.Establish(species, site))
				return false;

			foreach (NeighborInfo neighborInfo in neighborhoods[species.Index]) {
				ActiveSite neighbor = site.GetNeighbor(neighborInfo.RelativeLocation) as ActiveSite;
				if (neighbor != null) {
					if (Util.Random.GenerateUniform() < neighborInfo.DistanceProbability)
						return true;
				}
			}
			return false;
		}
	}
}
