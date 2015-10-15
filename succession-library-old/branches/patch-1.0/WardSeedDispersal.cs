using Landis.Landscape;
using Landis.Species;
using Landis.Util;

using System;

namespace Landis.Succession
{
	/// <summary>
	/// Brendan Ward's seed dispersal algorithm.  Code closely follows the
	/// structure of the C++ version of algorithm in BioLandis.
	/// </summary>
	public static class WardSeedDispersal
	{
		//---------------------------------------------------------------------

		private static MutableSite neighbor;

		//---------------------------------------------------------------------

		static private void findX1X2(out int x1, out int x2, int x, int y, int y1, int r)
		//This will find the scan line (x1,x2) of a circle or radius r given that the
		//center of the circle is at (x,y) and the scan line runs from (x1,y1) to
		//(x2,y1).  It will also make sure that the entire scan line lies within the
		//map boundaries.
		{
			int u, v;
			int snc=(int) Model.Landscape.Columns;
			
			u = Math.Abs(y1-y);
			v = (r*r) - (u*u);
			v = (int)(Math.Sqrt((float)v)+.999);
			
			if ((x-v)>1) x1=(x-v);
			 else x1=1;
			if ((x+v)<snc) x2=(x+v);
			 else x2=snc;
		}

		//---------------------------------------------------------------------

		public static bool Algorithm(ISpecies   species,
		                             ActiveSite site)
		{
			if (! Reproduction.SufficientLight(species, site) ||
			    ! Reproduction.Establish(species, site))
				return false;

			if (Reproduction.MaturePresent(species, site))
				return true;

			int row = (int) site.Location.Row;
			int col = (int) site.Location.Column;

			int cellDiam = (int) Model.CellLength;
			int EffD = species.EffectiveSeedDist;
			int MaxD = species.MaxSeedDist;

			double ratio = 0.95;//the portion of the probability in the effective distance
			//lambda1 parameterized for effective distance
			double lambda1 = Math.Log(1-ratio)/EffD;
			//lambda2 parameterized for maximum distance
			double lambda2 = Math.Log(0.01)/MaxD;

			double lowBound=0, upBound=0;
			//bool suitableDist=false;//flag to trigger if seed (plural) can get to a site based on distance probability
			double distanceProb=0.0;
			int pixRange = Math.Max((int) ((float) MaxD / (float) cellDiam), 1);
			int maxrow = (int) Math.Min(row+pixRange, Model.Landscape.Rows);
			int minrow = Math.Max(row-pixRange, 1);
			for (int i=minrow; i<=maxrow; i++) {
				int x1, x2;
				//float b = 5.0;
				findX1X2(out x1, out x2, col, row, i, pixRange);
				for (int j=x1; j<=x2;j++) {
					Location loc = new Location((uint) i, (uint) j);
					if (Model.Landscape.GetSite(loc, ref neighbor) && neighbor.IsActive) {
						if (Reproduction.MaturePresent(species, neighbor)) {
							float distance=(float)Math.Sqrt((float)((row-i)*(row-i)+(col-j)*(col-j))) * cellDiam;  //Pythag

							//set lower boundary to the theoretical (straight-line) edge of parent cell
							lowBound = distance - cellDiam;
							if(lowBound < 0) lowBound=0;

							//set upper boundary to the outer theoretical boundary of the cell
							upBound = distance;
					
							if(cellDiam<=EffD)
							{//Draw probabilities from either EffD or MaxD curves
								if(distance<=(float)EffD)
								{//BCW May 04
									distanceProb = Math.Exp(lambda1*lowBound) - Math.Exp(lambda1*upBound);
								}
								else
								{//BCW May 04
									distanceProb = (1-ratio)*Math.Exp(lambda2*(lowBound-EffD)) - (1-ratio)*Math.Exp(lambda2*(upBound-EffD));
								}
							}
							else
							{
								if(distance<=cellDiam)
								{//Draw probabilities from both EffD and MaxD curves
									distanceProb = Math.Exp(lambda1*lowBound)-(1-ratio)*Math.Exp(lambda2*(upBound-EffD));
								}
								else
								{
									distanceProb = (1-ratio)*Math.Exp(lambda2*(lowBound-EffD)) - (1-ratio)*Math.Exp(lambda2*(upBound-EffD));
								}
							}
							if (distanceProb > Landis.Util.Random.GenerateUniform())// && frand() < l->probRepro(speciesNum))  Modified BCW May '04
							{
								//  success = sites(row,col)->addNewCohort(s, sa, 10);
								return true;
							}
						}
					}  // if neighor is active
				} // for each column, j, of current row in neighborhood
			} // for each row, i, in the neighborhood

			//  Search failed.
			return false;
		}
	}
}
