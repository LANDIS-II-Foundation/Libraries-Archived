// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.Core;

namespace Landis.Library.HarvestManagement
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
            parameters = new EconomicRankParameters[Model.Core.Species.Count];
        }
    }
}
