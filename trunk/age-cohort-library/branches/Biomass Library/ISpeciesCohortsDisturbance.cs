namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// A disturbance that considers all the cohorts of a species together at
    /// once.
    /// </summary>
    public interface ISpeciesCohortsDisturbance
        : IDisturbance
    {
        /// <summary>
        /// Determines which cohorts for a species are damaged by the
        /// disturbance.
        /// </summary>
        /// <param name="cohorts">
        /// The species' cohorts to consider.
        /// </param>
        /// <param name="isDamaged">
        /// Boolean values that indicate which cohorts are damaged.
        /// </param>
        void MarkCohortsForDeath(ISpeciesCohorts cohorts,
                          ISpeciesCohortBoolArray isDamaged);
    }
}
