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
    public class Cohort : Landis.Library.AgeOnlyCohorts.ICohort, Landis.Library.BiomassCohorts.ICohort  
    {
        public System.Collections.Generic.List<float> Fwater;// { get; set; }
        public System.Collections.Generic.List<float> Frad;// { get; set; }
        public int SubLayerDominanceSum;// { get; set; }   

        public float Rootsenescence {get;set;}
        public float Woodsenescence { get; set; }
        public float Folalloc { get; set; }
        public float WoodAlloc { get; set; }
        public float RootAlloc { get; set; }
        public float FActiveBiom { get; set; }
        public float ReleasedNSC { get; set; }
        public float FolResp{ get; set; }
        public float Transpiration { get; set; }
        public float Netpsn { get; set; }
        public int MaxBiomass { get; set; }
        public float Fage { get; set; }
        public float Dominance { get; set; }
        public float MaintenanceRespiration { get; set; }
        public float Grosspsn{ get; set; }
        public float LAI{ get; set; }
        public ushort Age{ get; set; }
        public float Fol{ get; set; }
        public float Wood{ get; set; }
        public float Root{ get; set; }
        public float NSC{ get; set; }
        public float NSCfrac { get; set; }
        public bool IsAlive;// { get; set; }
        public int YearOfBirth { get; private set; }
        public ISpecies Species { get; private set; }
       
        
          
        public Cohort(ISpecies species, ActiveSite site, float NSC, int year_of_birth)
             
        {
            this.SubLayerDominanceSum = 0;
            this.Fwater = new System.Collections.Generic.List<float>();
            this.Frad = new System.Collections.Generic.List<float>();
            this.NSCfrac = 0.1F;
            this.Species = species;
            this.Age = 1;
            this.Wood = 10;
            this.NSC = NSC;
            //this.Root = Root;
            this.YearOfBirth = year_of_birth;
        }
        public Cohort(Cohort cohort)
        {
            this.SubLayerDominanceSum = cohort.SubLayerDominanceSum;
            this.Fwater = new System.Collections.Generic.List<float>(cohort.Fwater);
            this.Frad = new System.Collections.Generic.List<float>(cohort.Frad);

            this.NSCfrac = cohort.NSCfrac;
            this.Species = cohort.Species;
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
            this.Species = species;
            this.Age = age;
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
