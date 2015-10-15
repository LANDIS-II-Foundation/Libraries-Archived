using Landis.Core;
using log4net;
using System;
using System.Reflection;
using Landis.SpatialModeling;

namespace Landis.Library.Succession
{
    /// <summary>
    /// Brendan Ward's seed dispersal algorithm.  Code originally derived from the
    /// structure of the C++ version of algorithm in BioLandis.
    /// </summary>
    public static class WardSeedDispersal
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;
        private const int Universal = -1;

        //---------------------------------------------------------------------

        public static bool Algorithm(ISpecies   species,
                                     ActiveSite site)
        {
            if (species.EffectiveSeedDist == Universal)
                return UniversalDispersal.Algorithm(species, site);

            if (! Reproduction.SufficientResources(species, site)) {
                if (isDebugEnabled)
                    log.DebugFormat("site {0}: {1} not seeded: insufficient light",
                                    site.Location, species.Name);
                return false;
            }

            if (! Reproduction.Establish(species, site)) {
                if (isDebugEnabled)
                    log.DebugFormat("site {0}: {1} not seeded: cannot establish",
                                    site.Location, species.Name);
                return false;
            }

            if (Reproduction.MaturePresent(species, site)) {
                if (isDebugEnabled)
                    log.DebugFormat("site {0}: {1} seeded on site",
                                    site.Location, species.Name);
                return true;
            }

            if (isDebugEnabled)
                log.DebugFormat("site {0}: search neighbors for {1}",
                                site.Location, species.Name);

            foreach (RelativeLocationWeighted reloc in Seeding.MaxSeedQuarterNeighborhood)
            {
                double distance = reloc.Weight;
                int rRow = (int) reloc.Location.Row;
                int rCol = (int) reloc.Location.Column;
                
                double EffD = (double) species.EffectiveSeedDist;
                double MaxD = (double) species.MaxSeedDist;
                    
                if(distance > MaxD + ((double) Model.Core.CellLength / 2.0 * 1.414)) 
                    return false;  //Check no further
                    
                double dispersalProb = GetDispersalProbability(EffD, MaxD, distance);

                //First check the Southeast quadrant:
                if (dispersalProb > Model.Core.GenerateUniform())
                {
                    Site neighbor = site.GetNeighbor(reloc.Location);
                    if (neighbor != null && neighbor.IsActive)
                        if (Reproduction.MaturePresent(species, (ActiveSite) neighbor)) 
                            return true;
                }                              
                
                //Next, check all other quadrants:        
                if (dispersalProb > Model.Core.GenerateUniform())
                {
                    Site neighbor = site.GetNeighbor(new RelativeLocation(rRow * -1, rCol));
                    if(rCol == 0)
                        neighbor = site.GetNeighbor(new RelativeLocation(0, rRow));
                    if (neighbor != null && neighbor.IsActive)
                        if (Reproduction.MaturePresent(species, (ActiveSite) neighbor)) 
                            return true;
                }

                if (dispersalProb > Model.Core.GenerateUniform())
                {
                    Site neighbor = site.GetNeighbor(new RelativeLocation(rRow * -1, rCol * -1));
                    if (neighbor != null && neighbor.IsActive)
                        if (Reproduction.MaturePresent(species, (ActiveSite) neighbor)) 
                            return true;
                 }

                if (dispersalProb > Model.Core.GenerateUniform())
                {
                    Site neighbor = site.GetNeighbor(new RelativeLocation(rRow, rCol * -1));
                    if(rCol == 0)
                        neighbor = site.GetNeighbor(new RelativeLocation(0, rRow * -1));
                    if (neighbor != null && neighbor.IsActive)
                        if (Reproduction.MaturePresent(species, (ActiveSite) neighbor)) 
                            return true;
                }

            }  // end foreach relativelocation

            return false;
        }
        
        private static double GetDispersalProbability(double EffD, double MaxD, double distance)
        {
            //UI.WriteLine("  Get Dispersal Prob.  EffD = {0}. MaxD = {1}.  Distance = {2}.", EffD, MaxD, distance);
            double ratio = 0.95;//the portion of the probability in the effective distance
            double lambda1 = Math.Log(1 - ratio) / EffD; //lambda1 parameterized for effective distance
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
