// This file is part of the Biomass Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/biomass-harvest/trunk/

using Landis.Core;
using Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;

namespace Landis.Library.BiomassHarvest
{
    public static class SiteVars
    {
        public static ISiteVar<ISiteCohorts> Cohorts { get; private set; }

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            Cohorts = Model.Core.GetSiteVar<ISiteCohorts>("Succession.BiomassCohorts");
        }
    }
}
