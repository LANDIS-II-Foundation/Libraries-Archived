using Landis.Species;
using System.Collections.Generic;

namespace Landis.Cohorts
{
    /// <summary>
    /// The cohorts for a particular species at a site.
    /// </summary>
    public interface ISpeciesCohorts<TCohort>
        : IEnumerable<TCohort>
        where TCohort : ICohort
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
