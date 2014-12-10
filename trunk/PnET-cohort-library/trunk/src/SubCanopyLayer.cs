using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Linq;
 

namespace Landis.Library.BiomassCohortsPnET
{
    public class SubCanopyLayer
    {
        static float log2 = (float)System.Math.Log(2);
        byte fwater;
        public float Fwater
        {
            get
            {
                return 1F/100F * fwater;
            }

            set
            {
                fwater = (byte)(100F * value);
            }
        }
        byte frad;
        public float Frad
        {
            get
            {
                return 1F / 100F * frad;
            }

            set
            {
                frad = (byte)(100F * value);
            }
        }
        public byte lai;
        public float LAI
        {
            get
            {
                return 1F / 100F * lai;
            }

            set
            {
                lai = (byte)(100F * value);
            }
        }
        ushort woodAlloc;
        public float WoodAlloc
        {
            get
            {
                return 1F / 100F * woodAlloc;
            }

            set
            {
                woodAlloc = (ushort)(100F * value);
            }
        }

        public ushort maintenancerespiration;
        public float MaintenanceRespiration
        {
            get
            {
                return 1F / 100F * maintenancerespiration;
            }

            set
            {
                maintenancerespiration = (ushort)(100F * value);
            }
        }
        public ushort fol;
        public float Fol
        {
            get
            {
                return 1F / 100F * fol;
            }

            set
            {
                fol = (ushort)(100F * value);
            }
        }


        public ushort radiation;
        public float Radiation
        {
            get
            {
                return 1F / 100F * radiation;
            }

            set
            {
                radiation = (ushort)(100F * value);
            }
        }
        public ushort netPsn;
        public float NetPsn
        {
            get
            {
                return 1F / 100F * netPsn;
            }

            set
            {
                netPsn = (ushort)(100F * value);
            }
        }

        public ushort folResp;
        public float FolResp
        {
            get
            {
                return 1F / 100F * folResp;
            }

            set
            {
                folResp = (ushort)(100F * value);
            }
        }


        public ushort grossPsn;
        public float GrossPsn
        {
            get
            {
                return 1F / 100F * grossPsn;
            }

            set
            {
                grossPsn = (ushort)(100F * value);
            }
        }

        public ushort transpiration;
        public float Transpiration
        {
            get
            {
                return 1F / 100F * transpiration;
            }

            set
            {
                transpiration = (ushort)(100F * value);
            }
        }

        public ushort folalloc;
        public float Folalloc
        {
            get
            {
                return 1F / 100F * folalloc;
            }

            set
            {
                folalloc = (ushort)(100F * value);
            }
        }


        public ushort rootAlloc;
        public float RootAlloc
        {
            get
            {
                return 1F / 100F * rootAlloc;
            }

            set
            {
                rootAlloc = (ushort)(100F * value);
            }
        }

        public ushort cumCohortBiomass;
        public float CumCohortBiomass
        {
            get
            {
                return 1F / 100F * cumCohortBiomass;
            }

            set
            {
                cumCohortBiomass = (ushort)(100F * value);
            }
        }


        public ushort releasedNSC;
        public float ReleasedNSC
        {
            get
            {
                return 1F / 100F * releasedNSC;
            }

            set
            {
                releasedNSC = (ushort)(100F * value);
            }
        }


        
        public byte LayerIndex;
        public byte CanopyLayer;
        public Cohort cohort;
       
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
        public void Reset()
        {
            Transpiration = 0;
            GrossPsn = 0;
            NetPsn = 0;
            Folalloc = 0;
            MaintenanceRespiration = 0;
            WoodAlloc = 0;
            FolResp = 0;
        }
        
    }
}
