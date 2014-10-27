// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// Main interface for initializing and configuring the library.
    /// </summary>
    public static class Main
    {
        /// <summary>
        /// Initialize the library for use by client code.
        /// </summary>
        /// <param name="modelCore">
        /// The model's core framework.
        /// </param>
        public static void InitializeLib(ICore modelCore)
        {
            Landis.Library.SiteHarvest.Main.InitializeLib(modelCore);
            Model.Core = modelCore;
            SiteVars.Initialize();
        }

        /// <summary>
        /// A delegate that determines if harvesting is allowed at a site.
        /// </summary>
        public delegate bool IsHarvestAllowedAt(ActiveSite site);

        /// <summary>
        /// Register a new method for determining if harvesting is allowed at
        /// an active site.
        /// </summary>
        /// <remarks>
        /// By default, harvesting is allowed at any active site, so harvest
        /// extensions will not need to register a method.  It is expected that
        /// the land use extension will register a method so that a site's land
        /// use will determine if harvesting is allowed at the site.
        /// </remarks>
        public static void RegisterMethod(IsHarvestAllowedAt isHarvestAllowedAt)
        {
            // TO DO: store the delegate somewhere -- here or in another class?
        }
    }
}
