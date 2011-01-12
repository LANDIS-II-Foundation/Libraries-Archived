using Landis.Landscape;
using Landis.PlugIns;

namespace Landis.AgeCohort
{
    /// <summary>
    /// Information about a cohort's death.
    /// </summary>
    public class DeathEventArgs
        : Cohorts.DeathEventArgs<ICohort, PlugInType>
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
