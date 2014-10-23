// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A stand ranking method where the oldest stands are harvested first.
    /// </summary>
    public class MaxCohortAge
        : StandRankingMethod
    {
        public MaxCohortAge()
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        /// <remarks>
        /// The stand's rank is its age.
        /// </remarks>
        protected override double ComputeRank(Stand stand, int i)
        {
            return stand.Age;
        }
    }
}
