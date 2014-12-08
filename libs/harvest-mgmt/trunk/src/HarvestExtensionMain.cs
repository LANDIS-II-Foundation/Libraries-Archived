// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// Base class for the main class of a harvest extension.
    /// </summary>
    public abstract class HarvestExtensionMain
        : ExtensionMain 
    {
        /// <summary>
        /// The extension type for harvest extensions.
        /// </summary>
        public static readonly ExtensionType ExtType = new ExtensionType("disturbance:harvest");

        public HarvestExtensionMain(string name)
            : base(name, ExtType)
        {
        }

        /// <summary>
        /// Raised when an individual site has been harvested.  Handlers
        /// can be added that do additional bookkeeping for the site.
        /// </summary>
        public static event SiteHarvestedEvent.Handler SiteHarvestedEvent;

        /// <summary>
        /// Signals that a site has just been harvested.
        /// </summary>
        public static void OnSiteHarvest(object sender,
                                         ActiveSite site)
        {
            if (SiteHarvestedEvent != null)
                SiteHarvestedEvent(sender, new SiteHarvestedEvent.Args(site));
        }
    }
}
