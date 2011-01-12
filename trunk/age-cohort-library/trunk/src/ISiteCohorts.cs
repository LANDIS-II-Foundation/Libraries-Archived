using Landis.Core;
using Landis.SpatialModeling;
using Landis.Cohorts;

using System.Collections.Generic;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// All the age cohorts at a site.
    /// </summary>
    public interface ISiteCohorts
        : Cohorts.ISiteCohorts<ISpeciesCohorts>//IEnumerable<ISpeciesCohorts>
    {

        /// <summary>
        /// Gets the cohorts for a particular species.
        /// </summary>
        /*ISpeciesCohorts this[ISpecies species]
        {
            get;
        }*/

        //---------------------------------------------------------------------

        /// <summary>
        /// Is at least one sexually mature cohort present for a particular
        /// species?
        /// </summary>
        //bool IsMaturePresent(ISpecies species);

        //bool HasAge(); //{return false;}  
        //bool HasBiomass(); //{return false;}
        //bool HasLeafBiomass(); //{return false;}

        /// <summary>
        /// Removes the cohorts which are damaged by a disturbance.
        /// </summary>
        // void RemoveMarkedCohorts(ICohortDisturbance disturbance);

        //---------------------------------------------------------------------

        /// <summary>
        /// Removes the cohorts which are damaged by a disturbance.
        /// </summary>
        // void RemoveMarkedCohorts(ISpeciesCohortsDisturbance disturbance);


        void AddNewCohort(ISpecies species);

        // TO BE FIXED !!
        //void AddNewCohort(ISpecies species, int initialBiomass);

        //void Grow(ActiveSite site, bool successionTimestep);

        void Grow(ushort years,
                  ActiveSite site,
                  int? successionTimestep,
                  ICore mCore);
    }
}
