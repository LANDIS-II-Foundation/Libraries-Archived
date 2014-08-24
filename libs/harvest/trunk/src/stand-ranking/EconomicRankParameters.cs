// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// The parameters for computing the economic rank for a species.
    /// </summary>
    public struct EconomicRankParameters
    {
        private byte economicRank;
        private ushort minAge;

        //---------------------------------------------------------------------

        /// <summary>
        /// The species' economic rank.
        /// </summary>
        public byte Rank
        {
            get {
                return economicRank;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The minimum age at which the species has economic value.
        /// </summary>
        public ushort MinimumAge
        {
            get {
                return minAge;
            }
        }

        //---------------------------------------------------------------------

        public EconomicRankParameters(byte   rank,
                                      ushort minAge)
        {
            this.economicRank = rank;
            this.minAge = minAge;
        }
    }
}
