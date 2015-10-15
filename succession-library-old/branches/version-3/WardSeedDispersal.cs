using Edu.Wisc.Forest.Flel.Grids;

using Landis.Landscape;
using Landis.Species;
using Landis.Util;

using System;
using System.Reflection;

using log4net;

namespace Landis.Succession
{
    /// <summary>
    /// Brendan Ward's seed dispersal algorithm.  Code closely follows the
    /// structure of the C++ version of algorithm in BioLandis.
    /// </summary>
    public static class WardSeedDispersal
    {
        //private static MutableSite neighbor;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

        public static void Algorithm(//ISpecies   species,
                                     ActiveSite site)
        {

            foreach (RelativeLocationWeighted reloc in Seeding.MaxSeedQuarterNeighborhood)
            {
                double distance = reloc.Weight;

                foreach(ISpecies species in Model.Core.Species)
                {

                    if (species.EffectiveSeedDist == EffectiveSeedDist.Universal)
                        if(Reproduction.SufficientLight(species, site) &&
                                Reproduction.Establish(species, site))
                        {
                            Reproduction.AddNewCohort(species, site);
                            break;
                        }


                    int EffD = species.EffectiveSeedDist;
                    int MaxD = species.MaxSeedDist;
                    
                    if(distance > MaxD) break;  //Check no further
                    
                    double dispersalProb = 0.0;
                    if(reloc.Location.Row == 0 && reloc.Location.Column == 0)  //Check seeds on site
                    {
                        if(Reproduction.SufficientLight(species, site) &&
                                Reproduction.Establish(species, site) &&
                                Reproduction.MaturePresent(species, site))
                        {
                            Reproduction.AddNewCohort(species, site);
                            break;
                        }
                    }
                    else
                    {
                        dispersalProb = GetDispersalProbability(EffD, MaxD, distance);
                    
                        //First check the Southeast quadrant:
                        Site neighbor = site.GetNeighbor(reloc.Location);
                        if (neighbor != null && neighbor.IsActive)
                            if (Reproduction.MaturePresent(species, neighbor)) 
                                if (dispersalProb > Landis.Util.Random.GenerateUniform())
                                {
                                    Reproduction.AddNewCohort(species, site);
                                    break;
                                 }
                              
                        //Next, check all other quadrants:        
                        neighbor = site.GetNeighbor(new RelativeLocation(reloc.Location.Row * -1, reloc.Location.Column));
                        if (neighbor != null && neighbor.IsActive)
                            if (Reproduction.MaturePresent(species, neighbor)) 
                                if (dispersalProb > Landis.Util.Random.GenerateUniform())
                                {
                                    Reproduction.AddNewCohort(species, site);
                                    break;
                                 }

                        neighbor = site.GetNeighbor(new RelativeLocation(reloc.Location.Row, reloc.Location.Column * -1));
                        if (neighbor != null && neighbor.IsActive)
                            if (Reproduction.MaturePresent(species, neighbor)) 
                                if (dispersalProb > Landis.Util.Random.GenerateUniform())
                                {
                                    Reproduction.AddNewCohort(species, site);
                                    break;
                                 }

                        neighbor = site.GetNeighbor(new RelativeLocation(reloc.Location.Row * -1, reloc.Location.Column * -1));
                        if (neighbor != null && neighbor.IsActive)
                            if (Reproduction.MaturePresent(species, neighbor)) 
                                if (dispersalProb > Landis.Util.Random.GenerateUniform())
                                {
                                    Reproduction.AddNewCohort(species, site);
                                    break;
                                 }
                    }
                }  //end species loop
            }  // end foreach relativelocation

            return;
        }
        
        private static double GetDispersalProbability(int EffD, int MaxD, double distance)
        {
            double ratio = 0.95;//the portion of the probability in the effective distance
            double lambda1 = Math.Log(1-ratio) / EffD; //lambda1 parameterized for effective distance
            double lambda2 = Math.Log(0.01) / MaxD;  //lambda2 parameterized for maximum distance
            double distanceProb = 0.0;
            double lowBound = 0.0;
            double upBound = 0.0;
            double cellDiam = Model.Core.CellLength;

                    
            //set lower boundary to the theoretical (straight-line) edge of parent cell
            lowBound = distance - cellDiam;
            if(lowBound < 0) lowBound = 0.0;

            //set upper boundary to the outer theoretical boundary of the cell
            upBound = distance;
                    
            if(cellDiam <= EffD)
            {//Draw probabilities from either EffD or MaxD curves
                if(distance <= (double) EffD)
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
                if(distance <= cellDiam)
                {//Draw probabilities from both EffD and MaxD curves
                    distanceProb = Math.Exp(lambda1*lowBound)-(1-ratio)*Math.Exp(lambda2*(upBound-EffD));
                }
                else
                {
                    distanceProb = (1-ratio)*Math.Exp(lambda2*(lowBound-EffD)) - (1-ratio)*Math.Exp(lambda2*(upBound-EffD));
                }
            }
            
            return distanceProb;
        }
    }
}
