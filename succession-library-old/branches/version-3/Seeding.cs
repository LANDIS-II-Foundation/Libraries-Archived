using Landis.Landscape;
using Landis.Species;
using Edu.Wisc.Forest.Flel.Grids;

using System.Reflection;
using System.Collections.Generic;

using System;
using log4net;

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
            seedingAlgorithm(site);
                    
        }
        
        //---------------------------------------------------------------------
        // Generate a  RelativeLocation array for one quarter of the neighborhood.  
        // Check each cell within a box to the northeast the center point.  This will
        // create a set of POTENTIAL neighbors.  These potential neighbors 
        // will need to be later checked to ensure that they are within the landscape
        // and active.

        private static IEnumerable<RelativeLocationWeighted> InitializeMaxSeedNeighborhood()
        {
            int maxSeedDistance = 0;
            foreach(ISpecies species in Model.Core.Species)
                    maxSeedDistance = Math.Max(maxSeedDistance, species.MaxSeedDist);

            double CellLength = Model.Core.CellLength;
            UI.WriteLine("   Creating Dispersal Neighborhood List.");
                        
            List<RelativeLocationWeighted> neighborhood = new List<RelativeLocationWeighted>();

            int neighborRadius = maxSeedDistance;
            int numCellRadius = (int) (neighborRadius / CellLength);
            UI.WriteLine("   Dispersal:  NeighborRadius={0}, CellLength={1}, numCellRadius={2}",
                        neighborRadius, CellLength, numCellRadius);
            double centroidDistance = 0;
            double cellLength = CellLength;
            
            for(int row=1; row<=numCellRadius; row++)
            {
                for(int col=0; col<=numCellRadius; col++)
                {
                    centroidDistance = DistanceFromCenter(row, col);

                        //UI.WriteLine("Centroid Distance = {0}.", centroidDistance);
                    if(centroidDistance  <= neighborRadius)
                    {
                        RelativeLocation reloc = new RelativeLocation(row, col);
                        neighborhood.Add(new RelativeLocationWeighted(reloc, centroidDistance));
                    }
                }
            }
            
            //Add same cell:    
            neighborhood.Add(new RelativeLocationWeighted(new RelativeLocation(0, 0), 0.0));
            
            WeightComparer weightComp = new WeightComparer();
            neighborhood.Sort(weightComp);
            foreach(RelativeLocationWeighted reloc in neighborhood)
                UI.WriteLine("Neighbor distance = {0}.", reloc.Weight);
                
            return neighborhood;
        }

        //-------------------------------------------------------
        //Calculate the distance from a location to a center
        //point (row and column = 0).
        private static double DistanceFromCenter(double row, double column)
        {
            double CellLength = Model.Core.CellLength;
            row = System.Math.Abs(row) * CellLength;
            column = System.Math.Abs(column) * CellLength;
            double aSq = System.Math.Pow(column,2);
            double bSq = System.Math.Pow(row,2);
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
                /*if (x.Weight < y.Weight)
                    return -1;
                else if (x.Weight > y.Weight)
                    return 1;
                else
                    return 0;
                 * */
                int myCompare = x.Weight.CompareTo(y.Weight);
                return myCompare;
            }
        }

        
        /*public class RelativeLocationWeighted
        {
            private RelativeLocation location;
            private double weight;

            //---------------------------------------------------------------------
            public RelativeLocation Location
            {
                get {
                    return location;
                }
                set {
                    location = value;
                }
            }
            
            public double Weight
            {
                get {
                    return weight;
                }
                set {
                    weight = value;
                }
            }
        
            public RelativeLocationWeighted (RelativeLocation location, double weight)
            {
                this.location = location;
                this.weight = weight;
            }
        }*/
    }
}
