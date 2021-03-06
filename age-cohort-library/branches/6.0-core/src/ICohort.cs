//using Landis.Species;
using Landis.Core;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// A species cohort with only age information.
    /// </summary>
    public interface ICohort
    {
        /// <summary>
        /// The cohort's age (years).
        /// </summary>
        ushort Age
        {
            get;
        }

        /// <summary>
        /// The cohort's species.
        /// </summary>
        
        ISpecies Species
        {
            get;
        }
    }
}
