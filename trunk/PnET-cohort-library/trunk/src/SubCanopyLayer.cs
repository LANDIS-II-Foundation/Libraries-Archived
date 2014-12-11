using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Linq;
using System.Diagnostics;

namespace Landis.Library.BiomassCohortsPnET
{
    

    public class SubCanopyLayer
    {
        
        static float log2 = (float)System.Math.Log(2);
        
        byte fwater;
        byte frad;
        public byte lai;
        public ushort maintenancerespiration;
        public byte LayerIndex;
        public byte CanopyLayer;
        public Cohort cohort;
        ushort netPsn;
        ushort radiation;
        ushort folResp;
        ushort grossPsn;
        ushort transpiration;
        ushort cumCohortBiomass;

        public float Radiation
        {
            get
            {
                return 0.1F * (float)radiation;
            }
            set
            {
                Debug.Assert(10 * value < ushort.MaxValue && value >= 0, "Radiation out of range" + value);
                radiation = (ushort)(10 * value);
            }
        }


        public float Fwater
        {
            get
            {
                return 0.01F * fwater;
            }

            set
            {
                Debug.Assert(100 * value < byte.MaxValue && value >= 0, "Fwater out of range " + value);
                fwater = (byte)(100F * value);
            }
        }
        
        public float Frad
        {
            get
            {
                return 0.01F * frad;
            }

            set
            {
                Debug.Assert(100 * value < byte.MaxValue && value >= 0, "Frad out of range " + value);
                frad = (byte)(100F * value);
            }
        }
        
        public float LAI
        {
            get
            {
                return 0.01F * lai;
            }

            set
            {
                Debug.Assert(100 * value < byte.MaxValue && value >= 0, "LAI out of range " + value);
                lai = (byte)(100F * value);
            }
        }
        public float MaintenanceRespiration
        {
            get
            {
                return 0.01F * maintenancerespiration;
            }

            set
            {
                Debug.Assert(100 * value < ushort.MaxValue && value >= 0, "MaintenanceRespiration out of range " + value);
                maintenancerespiration = (ushort)(100F * value);
            }
        }

        
        public float NetPsn
        {
            get
            {
                return 0.01F * netPsn;
            }

            set
            {
                Debug.Assert(100 * value < ushort.MaxValue && value >= 0, "NetPsn out of range " + value);
                netPsn = (ushort)(100F * value);
            }
        }

       
        public float FolResp
        {
            get
            {
                return 0.01F * folResp;
            }

            set
            {
                Debug.Assert(100 * value < ushort.MaxValue && value >= 0, "FolResp out of range " + value);
                folResp = (ushort)(100F * value);
            }
        }


       
        public float GrossPsn
        {
            get
            {
                return 0.01F * grossPsn;
            }

            set
            {
                Debug.Assert(100 * value < ushort.MaxValue && value >= 0, "GrossPsn out of range " + value);
                grossPsn = (ushort)(100F * value);
            }
        }

        
        public float Transpiration
        {
            get
            {
                return 0.01F * transpiration;
            }

            set
            {
                Debug.Assert(100 * value < ushort.MaxValue && value >= 0, "GrossPsn out of range " + value);
                transpiration = (ushort)(100F * value);
            }
        }
         
        public float CumCohortBiomass
        {
            get
            {
                return cumCohortBiomass;
            }

            set
            {
                Debug.Assert(value < ushort.MaxValue && value >= 0, "CumCohortBiomass out of range " + value);
                cumCohortBiomass = (ushort)(value);
            }
        }

        public ISpecies Species
        {
            get
            {
                return cohort.Species;
            }
        }

        public SubCanopyLayer(Cohort cohort, byte LayerIndex)
        {
            this.cohort = cohort;
            this.LayerIndex = LayerIndex;
        }
         
        
    }
}
