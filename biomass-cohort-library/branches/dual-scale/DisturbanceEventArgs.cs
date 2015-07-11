using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;
using Landis.PlugIns;

namespace Landis.Biomass
{
    /// <summary>
    /// Information about a disturbance event at a site.
    /// </summary>
    public class DisturbanceEventArgs
    {
        private ActiveSite site;
        private PlugInType disturbanceType;

        //---------------------------------------------------------------------

        /// <summary>
        /// The site where the cohort died.
        /// </summary>
        public ActiveSite Site
        {
            get {
                return site;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The type of disturbance that killed the cohort.
        /// </summary>
        public PlugInType DisturbanceType
        {
            get {
                return disturbanceType;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public DisturbanceEventArgs(ActiveSite site,
                                    PlugInType disturbanceType)
        {
            this.site = site;
            this.disturbanceType = disturbanceType;
        }
    }
}
