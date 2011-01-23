using Landis.Core;
using Landis.SpatialModeling;
//using Landis.Cohorts;

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
        /// Removes the cohorts which are damaged by a disturbance.
        /// </summary>
        void RemoveMarkedCohorts(ICohortDisturbance disturbance);

        //---------------------------------------------------------------------

        /// <summary>
        /// Removes the cohorts which are damaged by a disturbance.
        /// </summary>
        void RemoveMarkedCohorts(ISpeciesCohortsDisturbance disturbance);


        void AddNewCohort(ISpecies species);

        void Grow(ushort years,
                  ActiveSite site,
                  int? successionTimestep,
                  ICore mCore);
    }
}
