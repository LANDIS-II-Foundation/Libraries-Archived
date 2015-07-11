// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A site-selection method that spreads to neighboring stands based on
    /// their rankings.
    /// </summary>
    public abstract class StandSpreading
    {
        private StandRanking[] rankings;
        private List<Stand> harvestedNeighbors;
        private List<Stand> unharvestedNeighbors;

        //---------------------------------------------------------------------

        /// <summary>
        /// The rankings of all the stands in the management area that the
        /// site-selection method is currently being applied to.
        /// </summary>
        public StandRanking[] StandRankings
        {
            get {
                return rankings;
            }

            set {
                rankings = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The list of neighboring stands that were harvested.
        /// </summary>
        public List<Stand> HarvestedNeighbors
        {
            get {
                return harvestedNeighbors;
            }
        }

        //---------------------------------------------------------------------
        
        /// <summary>
        /// The list of neighboring stands that were NOT harvested,
        /// and should be 'frozen' if there is a stand-adjacency constraint
        /// </summary>
        public List<Stand> UnharvestedNeighbors
        {
            get {
                return unharvestedNeighbors;
            }
        }
        
        //---------------------------------------------------------------------

        protected StandSpreading()
        {
            harvestedNeighbors = new List<Stand>();
            unharvestedNeighbors = new List<Stand>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the ranking for an unharvested stand from among the whole set
        /// of stand rankings.
        /// </summary>
        public StandRanking GetRanking(Stand stand)
        {
            //  Search backward through the stand rankings because unharvested
            //  stands are at the end of the list.
            for (int i = rankings.Length - 1; i >= 0; i--) {
                if (rankings[i].Stand == stand)
                    return rankings[i];
            }
            //throw new System.ApplicationException("ERROR: Stand not found in rankings");
            //instead of throwing an exception, just return 0
            rankings[0].Stand = stand;
            rankings[0].Rank = 0;
            return rankings[0];
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a stand's unharvested neighbors and their rankings to a
        /// sorted list of stand rankings.
        /// </summary>
        /// <remarks>
        /// The stand rankings are in highest to lowest order.  A neighbor is
        /// only added to the list if its rank is > 0 and it isn't already in
        /// the list.
        /// </remarks>
        public void AddUnharvestedNeighbors(Stand              stand,
                                            List<StandRanking> neighborRankings)
        {
            foreach (Stand neighbor in stand.Neighbors) {
                if (! neighbor.Harvested) {
                    bool inList = false;
                    foreach (StandRanking ranking in neighborRankings) {
                        if (ranking.Stand == neighbor) {
                            inList = true;
                            break;
                        }
                    }
                    if (inList)
                        continue;

                    StandRanking neighborRanking = GetRanking(neighbor);
                    if (neighborRanking.Rank <= 0)
                        continue;

                    int i;
                    for (i = 0; i < neighborRankings.Count; i++) {
                        if (neighborRankings[i].Rank < neighborRanking.Rank)
                            break;
                    }

                    //Model.Core.UI.WriteLine("   place={0}, rank={1}.", i, neighborRanking.Rank);
                    neighborRankings.Insert(i, neighborRanking);
                    
                }
            }
        }

        //---------------------------------------------------------------------

        public static void ValidateTargetSize(InputValue<double> targetSize)
        {
            if (targetSize.Actual < 0)
                throw new InputValueException(targetSize.String,
                                              "Target harvest size cannot be negative");
        }

        //---------------------------------------------------------------------
        public static void ValidateTargetSizes(
            InputValue<double> minTargetSize,
            InputValue<double> maxTargetSize) {
            if (minTargetSize.Actual < 0)
                throw new InputValueException(
                    minTargetSize.String,
                    "Min target harvest size cannot be negative");

            if (maxTargetSize.Actual <= 0)
                throw new InputValueException(maxTargetSize.String,
                    "Max target harvest size must be positive");

            if (minTargetSize.Actual > maxTargetSize.Actual)
                throw new InputValueException(minTargetSize.String + " " +
                    maxTargetSize.String,
                    "Max target harvest size cannot be greater than min.");
        } 
        

    }
}
