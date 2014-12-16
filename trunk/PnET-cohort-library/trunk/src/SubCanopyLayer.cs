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
        
      
       
        
        public byte LayerIndex;
        public byte CanopyLayer;
        public Cohort cohort;
  
        ushort cumCohortBiomass;

        public float Radiation;
         


        public float Fwater;
         
        public float Frad;
         

        public float LAI;

        public float MaintenanceRespiration;


        public float NetPsn;



        public float FolResp;



        public float GrossPsn;



        public float Transpiration;
         
         
        public float CumCohortBiomass
        {
            get
            {
                return cumCohortBiomass;
            }

            set
            {
                //Debug.Assert(value < ushort.MaxValue && value >= 0, "CumCohortBiomass out of range " + value);
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
