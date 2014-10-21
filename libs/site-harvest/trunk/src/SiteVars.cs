// This file is part of the Site Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/site-harvest/trunk/

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Library.SiteHarvest
{
    /// <summary>
    /// The library's site variables.
    /// </summary>
    public static class SiteVars
    {
        /// <summary>
        /// The site variable with cohorts (accessed as age-only cohorts).
        /// </summary>
        public static ISiteVar<ISiteCohorts> Cohorts { get; private set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the library's site variables.
        /// </summary>
        public static void Initialize()
        {
            Cohorts = Model.Core.GetSiteVar<ISiteCohorts>("Succession.AgeCohorts");
        }
    }
}
