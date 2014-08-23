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
    public class Cohort : Landis.Library.AgeOnlyCohorts.ICohort, Landis.Library.BiomassCohorts.ICohort  
    {
        
        float fol;
        float wood;
        float root;
        float nsc;
        float folshed;
        ushort age;
        ISpecies species;

        /// <summary>
        /// Occurs when a cohort dies either due to senescence or disturbances.
        /// </summary>
        public static event Landis.Library.AgeOnlyCohorts.DeathEventHandler<Landis.Library.AgeOnlyCohorts.DeathEventArgs> DeathEvent;

        //---------------------------------------------------------------------

        /// <summary>
        /// Raises a Cohort.DeathEvent.
        /// </summary>
        public static void Died(object sender,
                                Landis.Library.AgeOnlyCohorts.ICohort cohort,
                                ActiveSite site,
                                ExtensionType disturbanceType)
        {
            if (DeathEvent != null)
                DeathEvent(sender, new Landis.Library.AgeOnlyCohorts.DeathEventArgs(cohort, site, disturbanceType));
        }

        public Cohort(ISpecies species, ushort age) 
        {
            this.species = species;
            this.age = age;
        }
        public ISpecies Species
        {
            get
            {
                return species;
            }
        }
        public ushort Age
        {
            set
            {
                age = value;
            }
            get
            {
                return age;
            }
        }
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
            get
            {
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
         
        public int ComputeNonWoodyBiomass(ActiveSite site)
        {
            return (int)(Fol + Root);
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
        
      
         
    }
}
