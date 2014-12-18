//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace Landis.Library.BiomassCohortsPnET
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public class Cohort : Landis.Library.AgeOnlyCohorts.ICohort  , Landis.Library.BiomassCohorts.ICohort  
    {
        LocalOutput cohortoutput;

        public delegate void AllocateLitters(Landis.Library.BiomassCohortsPnET.Cohort cohort, ActiveSite site, ExtensionType disturbanceType);

        public static event AllocateLitters allocatelitters;

        public delegate float Fage(Landis.Library.BiomassCohortsPnET.Cohort cohort);

        public static event Fage calculate_fage;

        public delegate float FActiveBiom(Landis.Library.BiomassCohortsPnET.Cohort cohort);

        public static event FActiveBiom calculate_factivebiom;

        public string outputfilename
        {
            get
            {
                return cohortoutput.FileName;
            }
        }
        public ushort Age { get; set; }
        public bool IsAlive;
        List<SubCanopyLayer> SubCanopyLayers;
        public ISpecies species { get; private set; }
        ushort maxbiomass;
        public ushort Root;
        public ushort Wood;
        public ushort Fol;  
        
        public ushort NSC;
        public ushort MaintenanceRespiration;

        public SubCanopyLayer this[int ly]
        {
            get
            {
                return SubCanopyLayers[ly];
            }
        }

        public float NSCfrac
        {
            get
            {
                return NSC / (calculate_factivebiom(this) * (Wood + Root) + Fol);
            }
        }
         
       
        public float MaxBiomass
        {
            get
            {
                return (float)maxbiomass;
            }
            set
            {
                Debug.Assert(value < ushort.MaxValue && value >= 0, "MaxBiomass out of range" + value);
                maxbiomass = (ushort)value;
            }
        }
        public int Layer
        {
            get
            {
                return SubCanopyLayers.Max(o => o.CanopyLayer);
            }
        }

        public float Radiation
        {
            get
            {
                return (float)SubCanopyLayers.Max(o => o.Radiation);
            }

        }
        public  float Fwater
        {
            get
            {
                return SubCanopyLayers.Average(o => o.Fwater);
            }
        }
        public  float Frad
        {
            get
            {
                return SubCanopyLayers.Average(o => o.Frad);
            }
        }
        
        public float LAI
        {
            get
            {
                return SubCanopyLayers.Sum(o => o.LAI);
            }
        }
       
       
        
        public float FolResp
        {
            get
            {
                return SubCanopyLayers.Sum(o => o.FolResp);
            }
        }
        public float Transpiration
        {
            get
            {
                return SubCanopyLayers.Sum(o => o.Transpiration);
            }
        }
        public float Netpsn
        {
            get
            {
                return SubCanopyLayers.Sum(o => o.NetPsn);
            }
        }
        public float Grosspsn
        {
            get
            {
                return Netpsn + FolResp;
            }
        }
       
        public Landis.Core.ISpecies Species 
        {
            get
            {
                return species;
            }
        }

        public float WaterUseEfficiency 
        {
            get
            {
                if (Transpiration > 0) return Netpsn / Transpiration;
                return 0;
            }
            
        }
        public void ClearSubLayers()
        {
            SubCanopyLayers = null;
        }
        public void AddSubLayers(int NrOfSublayers)
        {
            SubCanopyLayers = new List<SubCanopyLayer>();
            for (byte i = 0; i < NrOfSublayers; i++)
            {
                SubCanopyLayers.Add(new SubCanopyLayer(this, i));
            }
        }

        public Cohort(ISpecies species, ushort year_of_birth, byte IMAX,float  InitialNSC, float DNSC)
             
        {
            this.species = species;
            this.Age = 0;
            this.NSC = (ushort) InitialNSC;
            this.Wood = (ushort)(1F / DNSC * (ushort) InitialNSC);
            this.MaxBiomass = this.Biomass;
            this.IsAlive = true;
        }
        public Cohort(Cohort cohort, int IMAX)
        {
            SubCanopyLayers = new List<SubCanopyLayer>();
            this.MaxBiomass = cohort.MaxBiomass;
            this.species = cohort.species;
            this.Age = cohort.Age;
            this.Wood = cohort.Wood;
            this.NSC = cohort.NSC;
            this.Root = cohort.Root;
            this.Fol = cohort.Fol;
            this.MaxBiomass = cohort.MaxBiomass;
            this.IsAlive = true;
         
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
        
        public void InitializeOutput(string SiteName, ushort YearOfBirth)
        {
            cohortoutput = new LocalOutput(SiteName, "Cohort_" + Species.Name + "_" + YearOfBirth + ".csv", OutputHeader);
        }

        public void UpdateCohortData(DateTime date, ActiveSite site, float FTempPSN, float FTempResp, bool Leaf_On)
        {
            string s = date.Year + "," + date.Month + "," + date.ToString("yyyy/MM") + "," + Age + "," + Layer + "," + LAI + "," + Grosspsn + "," +
                       FolResp + "," + MaintenanceRespiration + "," + Netpsn  + "," +  WaterUseEfficiency + "," + Fol + "," + Root + "," + Wood + "," + NSC + "," +
                       NSCfrac + "," + Fwater + "," + Radiation + "," + Frad + "," + FTempPSN + "," + FTempResp + "," + calculate_fage(this) + "," + Leaf_On + "," +
                       calculate_factivebiom(this);

            cohortoutput.Add(s);
        }

        public string OutputHeader
        {
            get
            {
                string hdr = OutputHeaders.Year + "," + OutputHeaders.Month + "," + OutputHeaders.date + "," + OutputHeaders.Age + "," + OutputHeaders.Layer + "," + OutputHeaders.LAI + "," +
                OutputHeaders.GrossPsn + "," + OutputHeaders.FolResp + "," + OutputHeaders.MaintResp + "," + OutputHeaders.NetPsn + "," +  OutputHeaders.WUE + "," 
                + OutputHeaders.Fol + "," + OutputHeaders.Root + "," + OutputHeaders.Wood + "," +
                OutputHeaders.NSC + "," + OutputHeaders.NSCfrac + "," + OutputHeaders.fWater + "," + OutputHeaders.Radiation + "," + OutputHeaders.fRad + "," + OutputHeaders.fTemp_psn + "," +
                OutputHeaders.fTemp_resp + "," + OutputHeaders.fage + "," + OutputHeaders.LeafOn + "," + OutputHeaders.FActiveBiom + ",";

                return hdr;
            }
        }
        public void WriteCohortData()
        {
            cohortoutput.Write();
        }

        public static event Landis.Library.AgeOnlyCohorts.DeathEventHandler<Landis.Library.AgeOnlyCohorts.DeathEventArgs> DeathEvent;

        /// <summary>
        /// Raises a Cohort.DeathEvent.
        /// </summary>
        public static void Died(object sender,
                                Landis.Library.BiomassCohortsPnET.Cohort cohort,
                                ActiveSite site,
                                ExtensionType disturbanceType)
        {

            if (cohort.cohortoutput != null)
            {
                cohort.cohortoutput.Write();
            }

            if (DeathEvent != null)
            {
                DeathEvent(sender, new Landis.Library.AgeOnlyCohorts.DeathEventArgs(cohort, site, disturbanceType));
            }
            if (allocatelitters != null)
            {
                allocatelitters(cohort, site, disturbanceType);
            }
        }
        
        
        
      
         
    }
}
