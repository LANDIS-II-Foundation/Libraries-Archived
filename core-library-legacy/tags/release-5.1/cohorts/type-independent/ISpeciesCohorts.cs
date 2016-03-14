using Landis.Species;
using System.Collections.Generic;

namespace Landis.Cohorts.TypeIndependent
{
    /// <summary>
    /// The cohorts for a particular species at a site.
    /// </summary>
    public interface ISpeciesCohorts
        : IEnumerable<ICohort>
    {
        /// <summary>
        /// The species of the cohorts.
        /// </summary>
        ISpecies Species
        {
            get;
        }
    }
}
