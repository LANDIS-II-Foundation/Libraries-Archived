// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Landis.SpatialModeling;
using System.Collections.Generic;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A site-selection method.
    /// </summary>
    public interface ISiteSelector
    {
        /// <summary>
        /// Returns a collection of the sites selected from a stand and its
        /// neighbors.
        /// </summary>
        IEnumerable<ActiveSite> SelectSites(Stand stand);

        //---------------------------------------------------------------------

        /// <summary>
        /// The total area of the selected sites.
        /// </summary>
        double AreaSelected
        {
            get;
        }
    }
}
