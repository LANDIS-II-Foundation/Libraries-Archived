
using Landis.Core;
using System.Collections;
using System.Collections.Generic;
using Landis.SpatialModeling;
using System;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// The age cohorts for a particular species at a site.
    /// </summary>
    public class SpeciesCohorts
        : ISpeciesCohorts
    {
        private ISpecies species;
        private List<ushort> ages;
        private bool isMaturePresent;

        //---------------------------------------------------------------------

        public int Count
        {
            get {
                return ages.Count;
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

        /// <summary>
        /// Initializes a new instance with one young cohort (age = 1).
        /// </summary>
        public SpeciesCohorts(ISpecies species)
        {
            this.species = species;
            this.ages = new List<ushort>();
            this.isMaturePresent = false;
            AddNewCohort();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with a list of ages.
        /// </summary>
        public SpeciesCohorts(ISpecies     species,
                              List<ushort> ages)
        {
            this.species = species;
            this.ages = new List<ushort>(ages);
            this.ages.Sort(Util.WhichIsOlder);
            this.isMaturePresent = false;
            for (int i = 0; i < this.ages.Count; i++) {
                if (this.ages[i] >= species.Maturity) {
                    this.isMaturePresent = true;
                    break;
                }
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance by copying a set of species cohorts.
        /// </summary>
        public SpeciesCohorts(ISpeciesCohorts cohorts)
        {
            this.species = cohorts.Species;
            this.ages = new List<ushort>(cohorts.Count);
            this.isMaturePresent = false;
            foreach (ICohort cohort in cohorts) {
                ushort age = cohort.Age;
                this.ages.Add(age);
                if (age >= species.Maturity)
                    this.isMaturePresent = true;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a new cohort.
        /// </summary>
        public void AddNewCohort()
        {
            //this.ages.Add(0);
            this.ages.Add(1);  //now adding with initial age 1
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Grows all the cohorts by advancing their ages.
        /// </summary>
        /// <param name="years">
        /// The number of years to advance each cohort's age.
        /// </param>
        /// <param name="successionTimestep">
        /// Indicates whether the current timestep is a succession timestep.
        /// If so, then all young cohorts (i.e., those whose ages are less than
        /// or equal to the succession timestep are combined into a single
        /// cohort whose age is the succession timestep.
        /// </param>
        /// <param name="site">
        /// The site where the cohorts are located.
        /// </param>
        public void Grow(ushort     years,
                         ActiveSite site,
                         int?       successionTimestep,
                         ICore      mCore)
        {
            //  Update ages
            for (int i = 0; i < ages.Count; i++) {
                if (successionTimestep.HasValue && (ages[i] < successionTimestep.Value))
                    // Young cohort
                    ages[i] = (ushort) successionTimestep.Value;
                else
                    ages[i] += years;
            }

            //  Combine young cohorts if succession timestep
            if (successionTimestep.HasValue) {
                //  Go backwards through list of ages, so the removal of an age
                //  doesn't mess up the loop.
                bool left1YoungCohort = false;
                for (int i = ages.Count - 1; i >= 0; i--) {
                    if (ages[i] == successionTimestep.Value) {
                        // Young cohort
                        if (left1YoungCohort)
                            ages.RemoveAt(i);
                        else
                            left1YoungCohort = true;
                    }
                }
            }

            //  Now go through ages and check for age-related mortality and
            //  senescence.  Again go backwards through the list, so the
            //  removal of an age doesn't mess up the loop.
            isMaturePresent = false;
            for (int i = ages.Count - 1; i >= 0; i--) {
                bool cohortDies = false;
                ushort age = ages[i];
                if (age > species.Longevity)
                    cohortDies = true;
                else if (age >= 0.8 * species.Longevity) {
                    //  Below is the annual age-related mortality probability,
                    //  so we need to check it for each year of growth.
                    double ageRelatedMortalityProb = (4 * ((double) age / species.Longevity) - 3) / 10;
                    for (int j = years; j > 0; j--) {
                        if (mCore.GenerateUniform() < ageRelatedMortalityProb)
                        {
                            cohortDies = true;
                            break;
                        }
                    }
                }
                if (cohortDies) {
                    ages.RemoveAt(i);
                    Cohort.Died(this, new Cohort(species, age), site, null);
                }
                else if (age >= species.Maturity)
                    isMaturePresent = true;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Removes the cohorts which are damaged by a disturbance.
        /// </summary>
        public void RemoveMarkedCohorts(ICohortDisturbance disturbance)
        {
            //  Go backwards through list of ages, so the removal of an age
            //  doesn't mess up the loop.
            isMaturePresent = false;
            for (int i = ages.Count - 1; i >= 0; i--) {
                ICohort cohort = new Cohort(species, ages[i]);
                //Console.WriteLine("Cohort={0}.", cohort.Species.Name);
                //Console.WriteLine("disturbance={0}.", disturbance.ToString());
                if (disturbance.MarkCohortForDeath(cohort))
                {
                    ages.RemoveAt(i);
                    Cohort.Died(this, cohort, disturbance.CurrentSite, disturbance.Type);
                }
                else if (ages[i] >= species.Maturity)
                    isMaturePresent = true;
            }
        }

        //---------------------------------------------------------------------

        private static SpeciesCohortBoolArray isSpeciesCohortDamaged;

        //---------------------------------------------------------------------

        static SpeciesCohorts()
        {
            isSpeciesCohortDamaged = new SpeciesCohortBoolArray();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Removes the cohorts which are damaged by a disturbance.
        /// </summary>
        public void RemoveCohorts(ISpeciesCohortsDisturbance disturbance)
        {
            isSpeciesCohortDamaged.SetAllFalse(Count);
            disturbance.MarkCohortsForDeath(this, isSpeciesCohortDamaged);

            //  Go backwards through list of ages, so the removal of an age
            //  doesn't mess up the loop.
            isMaturePresent = false;
            for (int i = ages.Count - 1; i >= 0; i--) {
                if (isSpeciesCohortDamaged[i]) {
                    ICohort cohort = new Cohort(species, ages[i]);
                    ages.RemoveAt(i);
                    Cohort.Died(this, cohort, disturbance.CurrentSite,
                                disturbance.Type);
                }
                else if (ages[i] >= species.Maturity)
                    isMaturePresent = true;
            } 
        }

        //---------------------------------------------------------------------

        public IEnumerator<ICohort> GetEnumerator()
        {
            foreach (ushort age in ages)
                yield return new Cohort(species, age);
        }
 
        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //---------------------------------------------------------------------
    }
}
