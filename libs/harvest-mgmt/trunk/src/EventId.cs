// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// Helper methods for generating id numbers for events.
    /// </summary>
    public static class EventId
    {
        private static int mostRecentId = 0;

        /// <summary>
        /// Make the id number for a new disturbance event.
        /// </summary>
        public static int MakeNewId()
        {
            int newId = ++mostRecentId;
            return newId;
        }
    }
}
