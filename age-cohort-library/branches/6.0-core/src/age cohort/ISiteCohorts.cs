namespace Landis.Library.Cohort.AgeOnly
{
    /// <summary>
    /// All the age cohorts at a site.
    /// </summary>
    public interface ISiteCohorts
        : Cohorts.ISiteCohorts<ISpeciesCohorts>
    {
        /// <summary>
        /// Removes the cohorts which are damaged by a disturbance.
        /// </summary>
        void DamageBy(ICohortDisturbance disturbance);

        //---------------------------------------------------------------------

        /// <summary>
        /// Removes the cohorts which are damaged by a disturbance.
        /// </summary>
        void DamageBy(ISpeciesCohortsDisturbance disturbance);
    }
}
