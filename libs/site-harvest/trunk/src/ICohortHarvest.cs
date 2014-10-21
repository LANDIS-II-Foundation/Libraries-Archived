// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.SpatialModeling;

namespace Landis.Extension.LandUse.LandCover
{
    /// <summary>
    /// Interface for cutting cohorts at a site.
    /// </summary>
    interface ICohortHarvest
    {
        /// <summary>
        /// Cut cohorts at an individual site.
        /// </summary>
        void Cut(ActiveSite site);
    }
}
