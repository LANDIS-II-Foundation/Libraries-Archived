
using Landis.Core;
using System.Collections.Generic;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// The age cohorts for a particular species at a site.
    /// </summary>
    public interface ISpeciesCohorts//<ICohort>
        : IEnumerable<ICohort>
    {
        /// <summary>
        /// The number of cohorts in the collection.
        /// </summary>
        int Count
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The species of the cohorts.
        /// </summary>
        ISpecies Species
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Is at least one sexually mature cohort present?
        /// </summary>
        bool IsMaturePresent
        {
            get;
        }
    }
}
