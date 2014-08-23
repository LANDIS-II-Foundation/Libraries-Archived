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
        public float FolCnt;

        int year_of_birth;

         
        private ActiveSite site;

        public ActiveSite Site
        {
            get
            {
                return site;
            }
        }
        
        private float rootsenescence;
        public float Rootsenescence
        {
            get
            {
                return rootsenescence;
            }
            set
            {
                rootsenescence = value;
            }
        }


        private float woodsenescence;
        public float Woodsenescence
        {
            get
            {
                return woodsenescence;
            }
            set
            {
                woodsenescence = value;
            }
        }

        System.Collections.Generic.List<float> fwater;
        public System.Collections.Generic.List<float> Fwater
        {
            get
            {
                return fwater;
            }
            set
            {
                fwater = value;
            }
        }
        private float folalloc;
        public float Folalloc
        {
            get
            {
                return folalloc;
            }
            set
            {
                folalloc = value;
            }
        }
        private float rootalloc;
        public float Rootalloc
        {
            get
            {
                return rootalloc;
            }
            set
            {
                rootalloc = value;
            }
        }

        private float woodalloc;
        public float Woodalloc
        {
            get
            {
                return woodalloc;
            }
            set
            {
                woodalloc = value;
            }
        }
        public float Maintenancerespiration
        {
            get
            {
                return maintenancerespiration;
            }
            set
            {
                maintenancerespiration = value;
            }
        }
        
        private float wue;
        public float WUE 
        { 
            get 
            { 
                return wue; 
            }
            set
            {
                wue = value;
            }
        }
        public float AverageFwater()
        {
            if (fwater.Count == 0) return 1;
            return fwater.Average();
        }
        public int Index;
        private float folresp;

        private float transpiration;

        private float abovecohortradiation;

        float bottomfrad;
        public float Bottomfrad
        {
            get
            {
                return bottomfrad;
            }
            set
            {
                bottomfrad = value;
            }
        }
        float upperfrad;
        public float UpperFrad
        {
            get
            {
                return upperfrad;
            }
            set
            {
                upperfrad = value;
            }
        }
        private float releasednsc;
        public float ReleasedNSC 
        { 
            get 
            { 
                return releasednsc; 
            } 
            set 
            {
                releasednsc = value;
            } 
        }
        public float WoodAlloc 
        { 
            get 
            { 
                return woodalloc; 
            }
            set 
            {
                woodalloc = value;
            }
        }
        public float RootAlloc 
        { 
            get 
            { 
                return rootalloc; 
            }
            set
            {
                rootalloc = value;
            }
        }

        public float FolResp
        {
            get
            {
                return folresp;
            }
            set
            {
                folresp = value;
            }
        }
        public float Transpiration
        {
            get
            {
                return transpiration;
            }
            set
            {
                transpiration = value;
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

        public Cohort(ISpecies species, ActiveSite site, float NSC, int year_of_birth)
             
        {
            this.species = species;
            this.age = 1;
            this.site = site;
            this.Wood = Wood;
            this.NSC = NSC;
            this.Root = Root;
            this.YearOfBirth = year_of_birth;

        }
        public Cohort(Cohort cohort)
        {
            this.species = cohort.species;
            this.age = cohort.Age;
            this.site = cohort.site;
            this.Wood = cohort.Wood;
            this.NSC = cohort.NSC;
            this.Root = cohort.Root;
            this.Fol = cohort.Fol;
            this.YearOfBirth = cohort.YearOfBirth;//
            this.FolShed = cohort.FolShed;
            this.MaxBiomass = cohort.MaxBiomass;
            this.LAI = cohort.LAI;
            this.Fage = cohort.Fage;
        }

        public float AboveCohortRadiation
        {
            get
            {
                return abovecohortradiation;
            }
            set
            {
                abovecohortradiation = value;
            }
        }
       
        
        float fol;
        float wood;
        float root;
        float nsc;
        float folshed;
        ushort age;
        float lai;
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
        private float netpsn;
        public float Netpsn
        {
            get
            {
                return netpsn;
            }
            set
            {
                netpsn = value;
            }
        }
        int maxbiomass;
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
        private float fage;
        public float Fage 
        { 
            get 
            { 
                return fage; 
            }
            set
            {
                fage = value;
            }
        }
        int canopylayer;
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
        private float maintenancerespiration;
        public float MaintenanceRespiration 
        { 
            get 
            { 
                return maintenancerespiration; 
            }
            set
            {
                maintenancerespiration = value;
            }
        }
        
        private float grosspsn;
        public float Grosspsn
        {
            get
            {
                return grosspsn;
            }
            set
            {
                grosspsn = value;
            }
        }
        public float LAI
        {
            get
            {
                return lai;
            }
            set
            {
                lai = value;
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
            return (int)(Fol);
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
