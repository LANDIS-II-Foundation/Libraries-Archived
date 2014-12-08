// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using System.Collections.Generic;   //added to use List data structure

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A ranking requirement which requires a stand be no more than a certain
    /// maximum age to be eligible for ranking.
    /// </summary>
    public class RandomRank
        : StandRankingMethod
    {
    
    private int [] rank_array;

        //---------------------------------------------------------------------

        public RandomRank()
        {
        }

        //---------------------------------------------------------------------

        protected override void InitializeForRanking(List<Stand> stands, int standCount)
        {

            List<int> stand_rankings = new List<int>(standCount);
            
            int i = 0;
            for (i = 0; i < standCount; i++) {
                stand_rankings.Add(i+1);
            }
			
            //Model.Core.Shuffle(stand_rankings);
            stand_rankings = Model.Core.shuffle(stand_rankings);
            
            rank_array = stand_rankings.ToArray();
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        /// <remarks>
        /// The stand's rank is a random number.
        /// </remarks>
        protected override double ComputeRank(Stand stand, int i)
        {
            //just return current rank from the list
            return rank_array[i];

        }
    }
}
