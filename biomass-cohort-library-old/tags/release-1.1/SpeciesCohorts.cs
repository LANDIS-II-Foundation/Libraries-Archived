using Edu.Wisc.Forest.Flel.Util;

using Landis.Cohorts;
using TypeIndependent = Landis.Cohorts.TypeIndependent;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using log4net;

namespace Landis.Biomass
{
    /// <summary>
    /// The cohorts for a particular species at a site.
    /// </summary>
    public class SpeciesCohorts
        : ISpeciesCohorts,
          AgeCohort.ISpeciesCohorts,
          TypeIndependent.ISpeciesCohorts
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

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

        public ICohort this[int index]
        {
            get {
                return new Cohort(species, cohortData[index]);
            }
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
        /// Creates a copy of a species' cohorts.
        /// </summary>
        public SpeciesCohorts Clone()
        {
            SpeciesCohorts clone = new SpeciesCohorts(this.species);
            clone.cohortData = new List<CohortData>(this.cohortData);
            clone.isMaturePresent = this.isMaturePresent;
            return clone;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with no cohorts.
        /// </summary>
        /// <remarks>
        /// Private constructor used by Clone method.
        /// </remarks>
        private SpeciesCohorts(ISpecies species)
        {
            this.species = species;
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
        /// Gets the age of a cohort at a specific index.
        /// </summary>
        /// <exception cref="System.IndexOutOfRangeException">
        /// </exception>
        public int GetAge(int index)
        {
            return cohortData[index].Age;
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
            Debug.Assert(cohort.Biomass <= siteBiomass);

            if (isDebugEnabled)
                log.DebugFormat("  grow cohort: {0}, {1} yrs, {2} Mg/ha",
                                cohort.Species.Name, cohort.Age, cohort.Biomass);

            //  Check for senescence
            if (cohort.Age >= species.Longevity) {
                siteBiomass -= cohort.Biomass;
                cohortMortality = cohort.Biomass;
                RemoveCohort(index, cohort, site, null);
                return index;
            }

            cohort.IncrementAge();
            int biomassChange = Cohorts.BiomassCalculator.ComputeChange(cohort, site, siteBiomass, prevYearSiteMortality);
            Debug.Assert(-(cohort.Biomass) <= biomassChange);  // Cohort can't loss more biomass than it has

            cohort.ChangeBiomass(biomassChange);
            siteBiomass += biomassChange;

            if (isDebugEnabled)
                log.DebugFormat("    biomass: change = {0}, cohort = {1}, site = {2}",
                                biomassChange, cohort.Biomass, siteBiomass);

            cohortMortality = Cohorts.BiomassCalculator.MortalityWithoutLeafLitter;
            if (cohort.Biomass > 0) {
                cohortData[index] = cohort.Data;
                return index + 1;
            }
            else {
                RemoveCohort(index, cohort, site, null);
                return index;
            }
        }

        //---------------------------------------------------------------------

        private void RemoveCohort(int        index,
                                  ICohort    cohort,
                                  ActiveSite site,
                                  PlugInType disturbanceType)
        {
            if (isDebugEnabled)
                log.DebugFormat("  cohort removed: {0}, {1} yrs, {2} Mg/ha ({3})",
                                cohort.Species.Name, cohort.Age, cohort.Biomass,
                                disturbanceType != null
                                    ? disturbanceType.Name
                                    : cohort.Age >= species.Longevity
                                        ? "senescence"
                                        : cohort.Biomass == 0
                                            ? "attrition"
                                            : "UNKNOWN");

            cohortData.RemoveAt(index);
            Cohort.Died(this, cohort, site, disturbanceType);
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

        /// <summary>
        /// Computes how much a disturbance damages the cohorts by reducing
        /// their biomass.
        /// </summary>
        /// <returns>
        /// The total of all the cohorts' biomass reductions.
        /// </returns>
        public int DamageBy(IDisturbance disturbance)
        {
            //  Go backwards through list of cohort data, so the removal of an
            //  item doesn't mess up the loop.
            isMaturePresent = false;
            int totalReduction = 0;
            for (int i = cohortData.Count - 1; i >= 0; i--) {
                Cohort cohort = new Cohort(species, cohortData[i]);
                ushort reduction = disturbance.Damage(cohort);
                if (reduction > 0) {
                    totalReduction += reduction;
                    if (reduction < cohort.Biomass) {
                        cohort.ChangeBiomass(-reduction);
                        cohortData[i] = cohort.Data;
                    }
                    else {
                        RemoveCohort(i, cohort, disturbance.CurrentSite,
                                     disturbance.Type);
                        cohort = null;
                    }
                }
                if (cohort != null && cohort.Age >= species.Maturity)
                    isMaturePresent = true;
            }
            return totalReduction;
        }

        //---------------------------------------------------------------------

        private static AgeCohort.SpeciesCohortBoolArray isSpeciesCohortDamaged;

        //---------------------------------------------------------------------

        static SpeciesCohorts()
        {
            isSpeciesCohortDamaged = new AgeCohort.SpeciesCohortBoolArray();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Removes the cohorts that are damaged by an age-only disturbance.
        /// </summary>
        /// <returns>
        /// The total biomass of all the cohorts damaged by the disturbance.
        /// </returns>
        public int DamageBy(AgeCohort.ISpeciesCohortsDisturbance disturbance)
        {
            isSpeciesCohortDamaged.SetAllFalse(Count);
            disturbance.Damage(this, isSpeciesCohortDamaged);

            //  Go backwards through list of cohort data, so the removal of an
            //  item doesn't mess up the loop.
            isMaturePresent = false;
            int totalReduction = 0;
            for (int i = cohortData.Count - 1; i >= 0; i--) {
                if (isSpeciesCohortDamaged[i]) {
                    Cohort cohort = new Cohort(species, cohortData[i]);
                    totalReduction += cohort.Biomass;
                    RemoveCohort(i, cohort, disturbance.CurrentSite,
                                 disturbance.Type);
                    cohort = null;
                }
                else if (cohortData[i].Age >= species.Maturity)
                    isMaturePresent = true;
            }
            return totalReduction;
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

        IEnumerator<TypeIndependent.ICohort> IEnumerable<TypeIndependent.ICohort>.GetEnumerator()
        {
            foreach (CohortData data in cohortData)
                yield return new Cohort(species, data);
        }
    }
}
