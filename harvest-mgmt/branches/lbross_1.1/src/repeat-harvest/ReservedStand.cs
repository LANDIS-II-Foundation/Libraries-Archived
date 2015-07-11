// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A stand that's been set aside for a future harvest.
    /// </summary>
    public struct ReservedStand
    {
        public Stand Stand;
        public int NextTimeToHarvest;

        //---------------------------------------------------------------------

        public ReservedStand(Stand stand,
                             int   nextTimeToHarvest)
        {
            this.Stand = stand;
            this.NextTimeToHarvest = nextTimeToHarvest;
        }
    }
}
