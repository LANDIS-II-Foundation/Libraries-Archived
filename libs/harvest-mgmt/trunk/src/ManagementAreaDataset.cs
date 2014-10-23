// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Edu.Wisc.Forest.Flel.Util;

using System.Collections;
using System.Collections.Generic;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A collection of management areas.
    /// </summary>
    public class ManagementAreaDataset
        : IManagementAreaDataset
    {
        private Dictionary<uint, ManagementArea> mgmtAreas;

        //---------------------------------------------------------------------

        public ManagementAreaDataset()
        {
            mgmtAreas = new Dictionary<uint, ManagementArea>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a new management area to the dataset.
        /// </summary>
        public void Add(ManagementArea mgmtArea)
        {
            Require.ArgumentNotNull(mgmtArea);
            mgmtAreas[mgmtArea.MapCode] = mgmtArea;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Finds a management area by its map code.
        /// </summary>
        /// <returns>
        /// null if there is no management area with the specified map code.
        /// </returns>
        public ManagementArea Find(uint mapCode)
        {
            ManagementArea mgmtArea;
            if (mgmtAreas.TryGetValue(mapCode, out mgmtArea))
                return mgmtArea;
            return null;
        }

        //---------------------------------------------------------------------

        IEnumerator<ManagementArea> IEnumerable<ManagementArea>.GetEnumerator()
        {
            foreach (ManagementArea mgmtArea in mgmtAreas.Values)
                yield return mgmtArea;
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ManagementArea>) this).GetEnumerator();
        }
    }
}
