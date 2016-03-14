using Landis.Species;

namespace Landis.Cohorts.TypeIndependent
{
    /// <summary>
    /// A type-independent interface to an individual cohort for a species.
    /// </summary>
    public interface ICohort
    {
        /// <summary>
        /// The cohort's species.
        /// </summary>
        ISpecies Species
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the value of a cohort's attribute.
        /// </summary>
        object this[CohortAttribute attribute]
        {
            get;
        }
    }
}
