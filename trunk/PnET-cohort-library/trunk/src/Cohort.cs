//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Linq;
namespace Landis.Library.BiomassCohortsPnET
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public class Cohort : Landis.Library.AgeOnlyCohorts.ICohort  , Landis.Library.BiomassCohorts.ICohort  
    {
        public System.Collections.Generic.List<float> Fwater;// { get; set; }
        public System.Collections.Generic.List<float> Frad;// { get; set; }
        public int SubLayerDominanceSum; 

        public float Rootsenescence;  
        public float Woodsenescence;
        public float Folalloc ;
        public float WoodAlloc ; 
        public float RootAlloc ;
        public float FActiveBiom;  
        public float ReleasedNSC ; 
        public float FolResp ;
        public float Transpiration;  
        public float Netpsn ;
        public int MaxBiomass;  
        public float Fage  ;
        public float Dominance;  
        public float MaintenanceRespiration  ;
        public float Grosspsn ;
        public float LAI ;
        public float Fol ;
        public float Wood ;
        public float Root ;
        public float NSC ;
        public float NSCfrac;  
        public bool IsAlive;
        public ushort Age  { get; set; }
        public int YearOfBirth { get; private set; }
        public Species species { get; private set; }

        public Species Species 
        {
            get
            {
                return species;
            }
        }



        public Cohort(Species species, int year_of_birth)
             
        {
            this.FActiveBiom = 1;
            this.SubLayerDominanceSum = 0;
            this.Fwater = new System.Collections.Generic.List<float>();
            this.Frad = new System.Collections.Generic.List<float>();
            this.NSCfrac = 0.1F;
            this.species = species;
            this.Age = 0;
            this.Fage = 1;
            this.Wood = 10;
            this.NSC = species.InitialNSC;
            this.YearOfBirth = year_of_birth;
            this.MaxBiomass = this.Biomass;
        }
        public Cohort(Cohort cohort)
        {
            this.FActiveBiom = cohort.FActiveBiom;
            this.SubLayerDominanceSum = cohort.SubLayerDominanceSum;
            this.Fwater = new System.Collections.Generic.List<float>(cohort.Fwater);
            this.Frad = new System.Collections.Generic.List<float>(cohort.Frad);
            this.MaxBiomass = cohort.MaxBiomass;
            this.NSCfrac = cohort.NSCfrac;
            this.species = cohort.species;
            this.Age = cohort.Age;
            this.Wood = cohort.Wood;
            this.NSC = cohort.NSC;
            this.Root = cohort.Root;
            this.Fol = cohort.Fol;
            this.YearOfBirth = cohort.YearOfBirth;//
            this.MaxBiomass = cohort.MaxBiomass;
            this.LAI = cohort.LAI;
            this.Fage = cohort.Fage;
            
        }
        /*
        public Cohort(ISpecies species, ushort age)
        {
            this.Species = species;
            this.Age = age;
        }
        */
 
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

        
        public int Biomass
        {
            get
            {
                return (int)(Wood + Fol);
            }
        }
        public int ComputeNonWoodyBiomass(ActiveSite site)
        {
            return (int)(Fol);
        }
        public static Percentage ComputeNonWoodyPercentage(Landis.Library.BiomassCohortsPnET.Cohort cohort, ActiveSite site)
        {
            return new Percentage(cohort.Fol / (cohort.Wood + cohort.Fol));
        }
        
        
        
        
      
         
    }
}
