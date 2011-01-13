using Landis.Cohorts;
using Landis.Landscape;
using Landis.Species;

using System;

namespace Landis.Biomass
{
    /// <summary>
    /// An iterator for a species' cohorts from oldest to youngest.
    /// </summary>
    public class OldToYoungIterator
    {
        private SpeciesCohorts cohorts;
 
        //  Index of the current cohort among the set of cohorts.
        private int index;

        //  Age of the current cohort
        private int currentCohortAge;

        //  Index of the next cohort among the set of cohorts.
        private int nextIndex;

        //---------------------------------------------------------------------

        /// <summary>
        /// The age of the current cohort.
        /// </summary>
        /// <remarks>
        /// After MoveNext method returns false, this property is 0.
        /// </remarks>
        public int Age
        {
            get {
                return currentCohortAge;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The set of cohorts the iterator is iterating through.
        /// </summary>
        public SpeciesCohorts SpeciesCohorts
        {
            get {
                return cohorts;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance for a set of species cohorts.
        /// </summary>
        public OldToYoungIterator(SpeciesCohorts cohorts)
        {
            this.cohorts = cohorts;
            this.currentCohortAge = cohorts[0];
            this.index = 0;
            this.nextIndex = 0;  // In case MoveNext is called before 1st call
                                 // to GrowCurrentCohort.
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Grows the current cohort for one year.
        /// </summary>
        /// <param name="site">
        /// The site where the cohort is located.
        /// </param>
        /// <param name="siteBiomass">
        /// The total biomass at the site.  This parameter is changed by the
        /// same amount as the current cohort's biomass.
        /// </param>
        /// <param name="prevYearMortality">
        /// The total mortality at the site during the previous year.
        /// </param>
        /// <returns>
        /// The total mortality (excluding annual leaf litter) for the current
        /// cohort.
        /// </returns>
        public int GrowCurrentCohort(ActiveSite site,
                                     ref int    siteBiomass,
                                     int        prevYearMortality)
        {
            if (currentCohortAge == 0)
                throw new InvalidOperationException("Iterator has no current cohort");

            int cohortMortality;
            nextIndex = cohorts.GrowCohort(index, site, ref siteBiomass,
                                           prevYearMortality, out cohortMortality);
            return cohortMortality;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Advances the iterator to the next cohort.
        /// </summary>
        /// <returns>
        /// True if there is another cohort to process.  False if there are no
        /// more cohorts.
        /// </returns>
        public bool MoveNext()
        {
            index = nextIndex;
            if (0 <= index && index < cohorts.Count) {
                currentCohortAge = cohorts[index];
                return true;
            }
            else {
                //  No more cohorts
                currentCohortAge = 0;
                return false;
            }
        }
    }
}
