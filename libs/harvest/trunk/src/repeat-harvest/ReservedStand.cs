// This file is part of the Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest/trunk/

namespace Landis.Library.Harvest
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
