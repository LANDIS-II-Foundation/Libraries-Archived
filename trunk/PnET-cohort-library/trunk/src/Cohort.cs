//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Library.BiomassCohortsPnET
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public class Cohort
        : ICohort, Landis.Library.BiomassCohorts.ICohort, Landis.Library.AgeOnlyCohorts.ICohort
    {
        private ISpecies species;
        ushort age;
        float fol;
        
        int maxbiomass;
        float wood;
        float root;
        float sep;
        float nsc;
        int year_of_birth;
        int canopylayer;
        float folshed;

        public float FolShed
        {
            get
            {
                return folshed;
            }
            set
            {
                folshed = value;
            }
        }
       
        public int Biomass
        {
            get
            {
                return (int)(wood + fol);
            }
        }
        public int MaxBiomass
        {
            get
            {
                return maxbiomass;
            }
            set
            {
                maxbiomass = value;
            }
        }
        public int CanopyLayer
        {
            get
            {
                return canopylayer;
            }
            set
            {
                canopylayer = value;
            }
        }
        public int YearOfBirth
        {
            get
            {
                return year_of_birth;
            }
            set
            {
                year_of_birth = value;
            }
        }
         
        public int ComputeNonWoodyBiomass(ActiveSite site)
        {
            return (int)(Fol + Root);
        }
       
        public ISpecies Species
        {
            get {
                return species;
            }
            set
            {
                species = value;
            }
        }
         
        public ushort Age
        {
            get {
                return age;
            }
            set
            {
                age = value;
            }
        }
         
        public float Fol
        {
            get
            {
                return fol;
            }
            set
            {
                fol = value;
            }
        }
        public float Wood
        {
            get {
                return wood;
            }
            set
            {
                wood = value;
            }
        }
        
        public float Root
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
            }
        }
        public float SEP
        {
            get
            {
                return sep;
            }
            set 
            {
                sep = value;
            }
        }
        public float NSC
        {
            get
            {
                return nsc;
            }
            set
            {
                nsc = value;
            }
        }
       
        //---------------------------------------------------------------------
        public Cohort()
        { 
        
        }
        

        //---------------------------------------------------------------------
 
         
        //---------------------------------------------------------------------

        /// <summary>
        /// Increments the cohort's age by one year.
        /// </summary>
        public void IncrementAge()
        {
            age += 1;
         
        }

        
        /// <summary>
        /// Occurs when a cohort dies either due to senescence or biomass
        /// disturbances.
        /// </summary>
        public static event Landis.Library.BiomassCohorts.DeathEventHandler<Landis.Library.BiomassCohorts.DeathEventArgs> DeathEvent;

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
                DeathEvent(sender, new Landis.Library.BiomassCohorts.DeathEventArgs(cohort, site, disturbanceType));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Occurs when a cohort is killed by an age-only disturbance.
        /// </summary>
        public static event Landis.Library.BiomassCohorts.DeathEventHandler<Landis.Library.BiomassCohorts.DeathEventArgs> AgeOnlyDeathEvent;

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
                AgeOnlyDeathEvent(sender, new Landis.Library.BiomassCohorts.DeathEventArgs(cohort, site, disturbanceType));
        }
    }
}
