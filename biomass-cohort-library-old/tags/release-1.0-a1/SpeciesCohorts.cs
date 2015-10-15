using Landis.Cohorts;
using Landis.Landscape;
using Landis.Species;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Landis.Biomass
{
    /// <summary>
    /// The cohorts for a particular species at a site.
    /// </summary>
    public class SpeciesCohorts
        : ISpeciesCohorts<ICohort>, ISpeciesCohorts<AgeCohort.ICohort>,
          IEnumerable<ushort>
    {
        private ISpecies species;
        private bool isMaturePresent;

        //  Cohort data is in oldest to youngest order.
        private List<CohortData> cohortData;

        //---------------------------------------------------------------------

        public int Count
        {
            get {
                return cohortData.Count;
            }
        }

        //---------------------------------------------------------------------

        public ISpecies Species
        {
            get {
                return species;
            }
        }

        //---------------------------------------------------------------------

        public bool IsMaturePresent
        {
            get {
                return isMaturePresent;
            }
        }

        //---------------------------------------------------------------------

        public ushort this[int index]
        {
            get {
                return cohortData[index].Age;
            }
        }

        //---------------------------------------------------------------------

        public IEnumerable<ushort> Ages
        {
            get {
                return this;
            }
        }

        //---------------------------------------------------------------------

        IEnumerator<ushort> IEnumerable<ushort>.GetEnumerator()
        {
            foreach (CohortData data in cohortData)
                yield return data.Age;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// An iterator from the oldest cohort to the youngest.
        /// </summary>
        public OldToYoungIterator OldToYoung
        {
            get {
                return new OldToYoungIterator(this);
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with one young cohort (age = 1).
        /// </summary>
        public SpeciesCohorts(ISpecies species,
                              ushort   initialBiomass)
        {
            this.species = species;
            this.cohortData = new List<CohortData>();
            this.isMaturePresent = false;
            AddNewCohort(initialBiomass);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a new cohort.
        /// </summary>
        public void AddNewCohort(ushort initialBiomass)
        {
            this.cohortData.Add(new CohortData(1, initialBiomass));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Combines all young cohorts into a single cohort whose age is the
        /// succession timestep - 1 and whose biomass is the sum of all the
        /// biomasses of the young cohorts.
        /// </summary>
        /// <remarks>
        /// The age of the combined cohort is set to the succession timestep -
        /// 1 so that when the combined cohort undergoes annual growth, its
        /// age will end up at the succession timestep.
        /// <p>
        /// For this method, young cohorts are those whose age is less than or
        /// equal to the succession timestep.  We include the cohort whose age
        /// is equal to the timestep because such a cohort is generated when
        /// reproduction occurs during a succession timestep.
        /// </remarks>
        public void CombineYoungCohorts()
        {
            //  Work from the end of cohort data since the array is in old-to-
            //  young order.
            int youngCount = 0;
            int totalBiomass = 0;
            for (int i = cohortData.Count - 1; i >= 0; i--) {
                CohortData data = cohortData[i];
                if (data.Age <= Cohorts.SuccessionTimeStep) {
                    youngCount++;
                    totalBiomass += data.Biomass;
                }
                else
                    break;
            }

            if (youngCount > 0) {
                cohortData.RemoveRange(cohortData.Count - youngCount, youngCount);
                cohortData.Add(new CohortData((ushort) (Cohorts.SuccessionTimeStep - 1),
                                              (ushort) totalBiomass));
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Grows an individual cohort for a year, incrementing its age by 1
        /// and updating its biomass for annual growth and mortality.
        /// </summary>
        /// <param name="index">
        /// The index of the cohort to grow; it must be between 0 and Count - 1.
        /// </param>
        /// <param name="site">
        /// The site where the species' cohorts are located.
        /// </param>
        /// <param name="siteBiomass">
        /// The total biomass at the site.  This parameter is changed by the
        /// same amount as the current cohort's biomass.
        /// </param>
        /// <param name="prevYearSiteMortality">
        /// The total mortality at the site during the previous year.
        /// </param>
        /// <param name="cohortMortality">
        /// The total mortality (excluding annual leaf litter) for the current
        /// cohort.
        /// </param>
        /// <returns>
        /// The index of the next younger cohort.  Note this may be the same
        /// as the index passed in if that cohort dies due to senescence.
        /// </returns>
        public int GrowCohort(int        index,
                              ActiveSite site,
                              ref int    siteBiomass,
                              int        prevYearSiteMortality,
                              out int    cohortMortality)
        {
            Debug.Assert(0 <= index && index <= cohortData.Count);
            Debug.Assert(site != null);

            Cohort cohort = new Cohort(species, cohortData[index]);

            //  Check for senescence
            if (cohort.Age >= species.Longevity) {
                siteBiomass -= cohort.Biomass;
                cohortMortality = cohort.Biomass;
                cohortData.RemoveAt(index);
                Cohorts.CohortDeath(cohort, site);
                return index;
            }

            cohort.IncrementAge();
            int biomassChange = Cohorts.BiomassCalculator.ComputeChange(cohort, site, siteBiomass, prevYearSiteMortality);
            cohort.ChangeBiomass(biomassChange);
            siteBiomass += biomassChange;
            cohortMortality = Cohorts.BiomassCalculator.MortalityWithoutLeafLitter;
            if (cohort.Biomass > 0) {
                cohortData[index] = cohort.Data;
                return index + 1;
            }
            else {
                cohortData.RemoveAt(index);
                Cohorts.CohortDeath(cohort, site);
                return index;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Updates the IsMaturePresent property.
        /// </summary>
        /// <remarks>
        /// Should be called after all the species' cohorts have grown.
        /// </remarks>
        public void UpdateMaturePresent()
        {
            isMaturePresent = false;
            for (int i = 0; i < cohortData.Count; i++) {
                if (cohortData[i].Age >= species.Maturity) {
                    isMaturePresent = true;
                    break;
                }
            }
        }

        //---------------------------------------------------------------------

        public void Remove(SelectMethod<ICohort> selectMethod)
        {
            //  Go backwards through list of cohort data, so the removal of an
            //  item doesn't mess up the loop.
            isMaturePresent = false;
            for (int i = cohortData.Count - 1; i >= 0; i--) {
                ICohort cohort = new Cohort(species, cohortData[i]);
                if (selectMethod(cohort)) {
                    cohortData.RemoveAt(i);
                    Cohorts.CohortDeath(cohort, null /* current site? */);
                    //  FIXME:  Need to pass in current site, which means
                    //          modifying the interface in core
                }
                else if (cohortData[i].Age >= species.Maturity)
                    isMaturePresent = true;
            }
        }

        //---------------------------------------------------------------------

        IEnumerator<ICohort> IEnumerable<ICohort>.GetEnumerator()
        {
            foreach (CohortData data in cohortData)
                yield return new Cohort(species, data);
        }
 
        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ICohort>) this).GetEnumerator();
        }

        //---------------------------------------------------------------------

        IEnumerator<AgeCohort.ICohort> IEnumerable<AgeCohort.ICohort>.GetEnumerator()
        {
            foreach (CohortData data in cohortData)
                yield return new AgeCohort.Cohort(species, data.Age);
        }

        //---------------------------------------------------------------------

        void ISpeciesCohorts<AgeCohort.ICohort>.Remove(SelectMethod<AgeCohort.ICohort> selectMethod)
        {
            Remove(new WrappedSelectMethod(selectMethod).Select);
        }
    }
}
