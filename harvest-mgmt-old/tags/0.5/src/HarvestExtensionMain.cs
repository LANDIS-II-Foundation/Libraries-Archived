// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.Core;

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
    }
}
