using Landis.Species;

namespace Landis.Cohorts
{
    /// <summary>
    /// An individual cohort for a species.
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
    }
}
