using Landis.Landscape;
using Landis.Species;

namespace Landis.Biomass
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public class Cohort
        : ICohort
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
    }
}
