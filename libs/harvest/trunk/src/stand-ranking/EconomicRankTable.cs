// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Landis.Core;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A collection of parameters for computing the economic ranks of species.
    /// </summary>
    public class EconomicRankTable
    {
        private EconomicRankParameters[] parameters;

        //---------------------------------------------------------------------

        public EconomicRankParameters this[ISpecies species]
        {
            get {
                return parameters[species.Index];
            }

            set {
                parameters[species.Index] = value;
            }
        }

        //---------------------------------------------------------------------

        public EconomicRankTable()
        {
            parameters = new EconomicRankParameters[PlugIn.ModelCore.Species.Count];
        }
    }
}
