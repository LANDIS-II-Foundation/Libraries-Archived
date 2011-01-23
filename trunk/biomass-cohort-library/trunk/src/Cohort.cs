//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Library.BiomassCohorts
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public class Cohort
        : ICohort//, Landis.Cohorts.TypeIndependent.ICohort
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

        public int Biomass
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

        /*public static readonly CohortAttribute AgeAttribute = new CohortAttribute("Age");
        public static readonly CohortAttribute BiomassAttribute = new CohortAttribute("Biomass");
        public static readonly CohortAttribute[] Attributes = new CohortAttribute[]{ AgeAttribute, BiomassAttribute };

        //---------------------------------------------------------------------

        object Landis.Cohorts.TypeIndependent.ICohort.this[CohortAttribute attribute]
        {
            get {
                if (attribute == AgeAttribute)
                    return data.Age;
                if (attribute == BiomassAttribute)
                    return data.Biomass;
                return null;
            }
        }*/

        //---------------------------------------------------------------------

        public Cohort(ISpecies species,
                      ushort   age,
                      int   biomass)
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
            data.Biomass = System.Math.Max(0, newBiomass);
        }

        //---------------------------------------------------------------------

        public int ComputeNonWoodyBiomass(ActiveSite site)
        {
            Percentage nonWoodyPercentage = Cohorts.BiomassCalculator.ComputeNonWoodyPercentage(this, site);
            return (int) (data.Biomass * nonWoodyPercentage);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Occurs when a cohort dies either due to senescence or biomass
        /// disturbances.
        /// </summary>
        public static event DeathEventHandler<DeathEventArgs> DeathEvent;

        //---------------------------------------------------------------------

        /// <summary>
        /// Raises a Cohort.DeathEvent.
        /// </summary>
        public static void Died(object     sender,
                                ICohort    cohort,
                                ActiveSite site,
                                ExtensionType disturbanceType)
        {
            if (DeathEvent != null)
                DeathEvent(sender, new DeathEventArgs(cohort, site, disturbanceType));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Occurs when a cohort is killed by an age-only disturbance.
        /// </summary>
        public static event DeathEventHandler<DeathEventArgs> AgeOnlyDeathEvent;

        //---------------------------------------------------------------------

        /// <summary>
        /// Raises a Cohort.AgeOnlyDeathEvent.
        /// </summary>
        public static void KilledByAgeOnlyDisturbance(object     sender,
                                                      ICohort    cohort,
                                                      ActiveSite site,
                                                      ExtensionType disturbanceType)
        {
            if (AgeOnlyDeathEvent != null)
                AgeOnlyDeathEvent(sender, new DeathEventArgs(cohort, site, disturbanceType));
        }
    }
}
