// This file is part of the Biomass Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/biomass-harvest/trunk/

using Landis.Core;

namespace Landis.Library.BiomassHarvest
{
    internal static class Model
    {
        /// <summary>
        /// The model core instance used across the library's internal
        /// components.
        /// </summary>
        internal static ICore Core { get; set; }
    }
}
