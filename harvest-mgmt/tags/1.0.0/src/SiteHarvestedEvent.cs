// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.SpatialModeling;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// Represents when an individual site is harvested.
    /// </summary>
    public class SiteHarvestedEvent
    {
        /// <summary>
        /// Information about an event that's passed to event handlers.
        /// </summary>
        public class Args : System.EventArgs
        {
            /// <summary>
            /// The site that was just harvested.
            /// </summary>
            public ActiveSite Site { get; protected set; }

            /// <summary>
            /// Creates a new instance.
            /// </summary>
            /// <param name="site">The site that was just harvested.</param>
            public Args(ActiveSite site)
            {
                Site = site;
            }
        }

        /// <summary>
        /// Handler for this type of event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="eventArgs">Details about the event.</param>
        public delegate void Handler(object sender,
                                     Args   eventArgs);
    }
}
