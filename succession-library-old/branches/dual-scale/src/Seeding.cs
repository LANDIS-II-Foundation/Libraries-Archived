using Landis.Species;
using log4net;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Succession
{
    public class Seeding
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        private SeedingAlgorithm seedingAlgorithm;
        public static List<RelativeLocationWeighted> MaxSeedQuarterNeighborhood;


        //---------------------------------------------------------------------

        public Seeding(SeedingAlgorithm seedingAlgorithm)
        {
            this.seedingAlgorithm = seedingAlgorithm;
        }

        //---------------------------------------------------------------------

        public void Do(ActiveSite site)
        {
            Do(site, null);
        }
        
        //---------------------------------------------------------------------

        public void Do(ActiveSite site,
                       BitArray   hasSeeded)
        {
            for (int i = 0; i < Model.Core.Species.Count; i++) {
                if (hasSeeded != null && hasSeeded[i])
                    continue;
                ISpecies species = Model.Core.Species[i];
                if (seedingAlgorithm(species, site)) {
                    Reproduction.AddNewCohort(species, site);
                    if (isDebugEnabled)
                        log.DebugFormat("site {0}: seeded {1}",
                                        site.Location, species.Name);
                    if (hasSeeded != null)
                        hasSeeded[i] = true;
                }
            }
        }
        
        //---------------------------------------------------------------------
        // Generate a  RelativeLocation array for one quarter of the neighborhood.  
        // Check each cell within a box to the northeast the center point.  This will
        // create a set of POTENTIAL neighbors.  These potential neighbors 
        // will need to be later checked to ensure that they are within the landscape
        // and active.

        public static void InitializeMaxSeedNeighborhood()
        {
            int maxSeedDistance = 0;
            foreach(ISpecies species in Model.Core.Species)
                maxSeedDistance = Math.Max(maxSeedDistance, species.MaxSeedDist);

            double cellLength = (double) Model.Core.CellLength;
            UI.WriteLine("   Creating Dispersal Neighborhood List.");
                        
            List<RelativeLocationWeighted> neighborhood = new List<RelativeLocationWeighted>();

            double neighborRadius = maxSeedDistance + (cellLength / 2.0);
            int numCellRadius = (int) (neighborRadius / cellLength);
            UI.WriteLine("   Dispersal:  NeighborRadius={0}, CellLength={1}, numCellRadius={2}",
                        neighborRadius, cellLength, numCellRadius);
            double centroidDistance = 0.0;
            
            for(int row=1; row <= numCellRadius + 1; row++)
            {
                for(int col=0; col <= numCellRadius + 1; col++)
                {
                    centroidDistance = DistanceFromCenter((double) row, (double) col);

                        //UI.WriteLine("Centroid Distance = {0}.", centroidDistance);
                    if(centroidDistance  <= neighborRadius)
                    {
                        RelativeLocation reloc = new RelativeLocation(row, col);
                        neighborhood.Add(new RelativeLocationWeighted(reloc, centroidDistance));
                    }
                }
            }
            
            WeightComparer weightComp = new WeightComparer();
            neighborhood.Sort(weightComp);
            //foreach(RelativeLocationWeighted reloc in neighborhood)
                //UI.WriteLine("Row = {0}. Col = {1}. Neighbor distance = {2}.", reloc.Location.Row, reloc.Location.Column, reloc.Weight);
            
            MaxSeedQuarterNeighborhood = neighborhood;
                
            return;
        }

        //-------------------------------------------------------
        //Calculate the distance from a location to a center
        //point (row and column = 0).
        private static double DistanceFromCenter(double row, double column)
        {
            double cellLength = (double) Model.Core.CellLength;
            row = System.Math.Abs(row) * cellLength;
            column = System.Math.Abs(column) * cellLength;
            double aSq = System.Math.Pow(column, 2);
            double bSq = System.Math.Pow(row, 2);
            return System.Math.Sqrt(aSq + bSq);
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Compares weights
        /// </summary>

        public class WeightComparer : IComparer<RelativeLocationWeighted>
        {
            public int Compare(RelativeLocationWeighted x,
                               RelativeLocationWeighted y)
            {
                int myCompare = x.Weight.CompareTo(y.Weight);
                return myCompare;
            }
        }

        
    }
}
