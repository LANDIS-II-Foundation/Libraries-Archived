// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A ranking requirement which requires a stand's neighbors be at 
    /// least a certain minimum age to be eligible for ranking.
    /// </summary>
    public class SpatialArrangement
        : IRequirement
    {
        private ushort minAge;

        //---------------------------------------------------------------------

        public SpatialArrangement(ushort age)
        {
            minAge = age;
        }

        //---------------------------------------------------------------------

        //require that the stand's neighbors are of a certain minimum age 
        //before this stand can be harvested.
        bool IRequirement.MetBy(Stand stand)
        {
            //initially declare stand available for ranking
            bool allow_rank = true;
            //loop through stand's neighbors.
            //if any of the stand's neighbors are too young,
            //don't rank this stand (set rank = 0)
            foreach (Stand neighbor in stand.Neighbors) {
                //Model.Core.UI.WriteLine("neighbor {0} age = {1}", neighbor.MapCode, neighbor.Age);
                if (neighbor.Age < minAge) {
                    //don't allow stand to be ranked
                    allow_rank = false;
                    break;
                }
            }
            //return allow_rank.  if false, stand will be ranked 0
            //Model.Core.UI.WriteLine("stand {0} allow_rank = {1}", stand.MapCode, allow_rank);
            return allow_rank;
        }
        
    }
}
