using Landis.Cohorts;
using Landis.Cohorts.TypeIndependent;
using TypeIndependent = Landis.Cohorts.TypeIndependent;
using Landis.Landscape;
using Landis.Species;

namespace Landis.Biomass
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public class Cohort
        : ICohort, TypeIndependent.ICohort
    {
        private ISpecies species;
        private CohortData data;

        //---------------------------------------------------------------------

        public ISpecies Species
        {
            get {
                return species;
            }
        }

        //---------------------------------------------------------------------

        public ushort Age
        {
            get {
                return data.Age;
            }
        }

        //---------------------------------------------------------------------

        public ushort Biomass
        {
            get {
                return data.Biomass;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The cohort's age and biomass data.
        /// </summary>
        public CohortData Data
        {
            get {
                return data;
            }
        }

        //---------------------------------------------------------------------

        public static readonly CohortAttribute AgeAttribute = new CohortAttribute("Age");
        public static readonly CohortAttribute BiomassAttribute = new CohortAttribute("Biomass");
        public static readonly CohortAttribute[] Attributes = new CohortAttribute[]{ AgeAttribute, BiomassAttribute };

        //---------------------------------------------------------------------

        object TypeIndependent.ICohort.this[CohortAttribute attribute]
        {
            get {
                if (attribute == AgeAttribute)
                    return data.Age;
                if (attribute == BiomassAttribute)
                    return data.Biomass;
                return null;
            }
        }

        //---------------------------------------------------------------------

        public Cohort(ISpecies species,
                      ushort   age,
                      ushort   biomass)
        {
            this.species = species;
            this.data.Age = age;
            this.data.Biomass = biomass;
        }

        //---------------------------------------------------------------------

        public Cohort(ISpecies   species,
                      CohortData cohortData)
        {
            this.species = species;
            this.data = cohortData;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Increments the cohort's age by one year.
        /// </summary>
        public void IncrementAge()
        {
            data.Age += 1;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Changes the cohort's biomass.
        /// </summary>
        public void ChangeBiomass(int delta)
        {
            int newBiomass = data.Biomass + delta;
            data.Biomass = (ushort) System.Math.Max(0, newBiomass);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Occurs when a cohort dies either due to senescence or disturbances.
        /// </summary>
        public static event CohortDiedEventHandler DiedEvent;

        //---------------------------------------------------------------------

        /// <summary>
        /// Raises a Cohort.Died event.
        /// </summary>
        public static void Died(ICohort    cohort,
                                ActiveSite site)
        {
            if (DiedEvent != null)
                DiedEvent(cohort, site);
        }
    }
}
