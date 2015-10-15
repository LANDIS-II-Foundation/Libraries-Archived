using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;
using Landis.PlugIns;

namespace Landis.Biomass
{
    /// <summary>
    /// Information about a cohort's death.
    /// </summary>
    public class DeathEventArgs
        : Landis.Cohorts.DeathEventArgs<ICohort, PlugInType>
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public DeathEventArgs(ICohort    cohort,
                              ActiveSite site,
                              PlugInType disturbanceType)
            :base(cohort, site, disturbanceType)
        {
        }
    }
}
