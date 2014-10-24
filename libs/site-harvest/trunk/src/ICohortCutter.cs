// This file is part of the Site Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/site-harvest/trunk/

using Landis.SpatialModeling;

namespace Landis.Library.SiteHarvest
{
    /// <summary>
    /// Interface for cutting selected cohorts at a site.
    /// </summary>
    public interface ICohortCutter
    {
        /// <summary>
        /// The object responsible for selecting which cohorts to be cut.
        /// </summary>
        ICohortSelector CohortSelector { get; }

        /// <summary>
        /// Cut cohorts at an individual site.
        /// </summary>
        /// <returns>
        /// The number of cohorts cut at the site.
        /// </returns>
        int Cut(ActiveSite site);
    }
}
