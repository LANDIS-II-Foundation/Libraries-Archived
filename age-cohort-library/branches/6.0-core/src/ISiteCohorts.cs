//using Landis.Species;
using Landis.Core;
using System.Collections.Generic;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// All the age cohorts at a site.
    /// </summary>
    public interface ISiteCohorts//<ISpeciesCohorts>
        : IEnumerable<ISpeciesCohorts>
    {

        /// <summary>
        /// Gets the cohorts for a particular species.
        /// </summary>
        ISpeciesCohorts this[ISpecies species]
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Is at least one sexually mature cohort present for a particular
        /// species?
        /// </summary>
        bool IsMaturePresent(ISpecies species);

        bool HasAge(); //{return false;}  
        bool HasBiomass(); //{return false;}
        bool HasLeafBiomass(); //{return false;}

        /// <summary>
        /// Removes the cohorts which are damaged by a disturbance.
        /// </summary>
        void MarkCohortsForDeath(ICohortDisturbance disturbance);

        //---------------------------------------------------------------------

        /// <summary>
        /// Removes the cohorts which are damaged by a disturbance.
        /// </summary>
        void MarkCohortsForDeath(ISpeciesCohortsDisturbance disturbance);
    }
}
