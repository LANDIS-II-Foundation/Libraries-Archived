// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using System.Collections.Generic;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A collection of management areas.
    /// </summary>
    public interface IManagementAreaDataset
        : IEnumerable<ManagementArea>
    {
        /// <summary>
        /// Finds a management area by its map code.
        /// </summary>
        /// <returns>
        /// null if there is no management area with the specified map code.
        /// </returns>
        ManagementArea Find(uint mapCode);
    }
}
