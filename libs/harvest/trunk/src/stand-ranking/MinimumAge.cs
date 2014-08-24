// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A ranking requirement which requires a stand be at least a certain
    /// minimum age to be eligible for ranking.
    /// </summary>
    public class MinimumAge
        : IRequirement
    {
        private ushort minAge;

        //---------------------------------------------------------------------

        public MinimumAge(ushort age)
        {
            minAge = age;
        }

        //---------------------------------------------------------------------

        bool IRequirement.MetBy(Stand stand)
        {
            return minAge <= stand.Age;
        }
    }
}
