using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;
using Landis.PlugIns;

namespace Landis.Biomass
{
    /// <summary>
    /// A disturbance that damages cohorts thereby reducing their biomass.
    /// </summary>
    public interface IDisturbance
    {
        /// <summary>
        /// The disturbance's type.
        /// </summary>
        PlugInType Type
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The current site that the disturbance is damaging.
        /// </summary>
        ActiveSite CurrentSite
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes how a cohort is damaged by the disturbance.
        /// </summary>
        /// <returns>
        /// The amount of biomass that the cohort's biomass is to be reduced
        /// by.
        /// </returns>
        ushort Damage(ICohort cohort);
    }
}
