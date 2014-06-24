// This file is part of the Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest/trunk/

using Landis.Core;

namespace Landis.Library.Harvest
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
