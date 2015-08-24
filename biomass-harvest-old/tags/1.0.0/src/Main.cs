// This file is part of the Biomass Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/biomass-harvest/trunk/

using Landis.Core;

namespace Landis.Library.BiomassHarvest
{
    /// <summary>
    /// Main entry point for initializing and configuring the library.
    /// </summary>
    public static class Main
    {
        /// <summary>
        /// Initialize the library for use by client code.
        /// </summary>
        public static void InitializeLib(ICore modelCore)
        {
            Landis.Library.SiteHarvest.Main.InitializeLib(modelCore);

            Model.Core = modelCore;
            SiteVars.Initialize();
            PartialThinning.InitializeClass();
        }
    }
}
