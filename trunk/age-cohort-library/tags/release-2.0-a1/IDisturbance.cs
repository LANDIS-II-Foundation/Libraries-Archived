using Landis.Landscape;

namespace Landis.AgeCohort
{
    /// <summary>
    /// A disturbance that damages cohorts.
    /// </summary>
    public interface IDisturbance
    {
        /// <summary>
        /// The current site that the disturbance is damaging.
        /// </summary>
        ActiveSite CurrentSite
        {
            get;
        }

        //---------------------------------------------------------------------

    	/// <summary>
    	/// Determines if a cohort is damaged by the disturbance.
    	/// </summary>
    	bool Damage(ICohort cohort);
    }
}
