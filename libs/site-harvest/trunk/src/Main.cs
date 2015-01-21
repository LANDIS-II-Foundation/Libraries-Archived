// This file is part of the Site Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/site-harvest/trunk/

using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Library.SiteHarvest
{
    /// <summary>
    /// Main interface for initializing and configuring the library.
    /// </summary>
    public static class Main
    {
        private static bool libInitialized = false;

        /// <summary>
        /// Initialize the library for use by client code.
        /// </summary>
        /// <param name="modelCore">
        /// The model's core framework.
        /// </param>
        public static void InitializeLib(ICore modelCore)
        {
            // Only initialize the library once.  This method may be called
            // multiple times if harvest and land-use extensions are both
            // in a scenario.  The harvest extension initializes the Harvest
            // Management, which in turns initializes this library.  The Land
            // Use extension also initializes this library since it's a client
            // of this library.
            if (! libInitialized)
            {
                Model.Core = modelCore;
                SiteVars.Initialize();
                AgeRangeParsing.InitializeClass();
                libInitialized = true;
            }
        }
    }
}
