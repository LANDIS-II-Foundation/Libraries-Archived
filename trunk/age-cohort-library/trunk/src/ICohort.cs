using Landis.Core;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// A species cohort with only age information.
    /// </summary>
    public interface ICohort
        : Landis.Library.Cohorts.ICohort
    {
        /// <summary>
        /// The cohort's age (years).
        /// </summary>
        ushort Age
        {
            get;
        }
    }
}
