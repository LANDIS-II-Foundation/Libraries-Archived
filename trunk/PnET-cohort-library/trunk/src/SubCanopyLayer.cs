using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Linq;
 

namespace Landis.Library.BiomassCohortsPnET
{
    public class SubCanopyLayer
    {
        static float log2 = (float)System.Math.Log(2);
        public float Fwater; 
        public float Frad;
        public float MaintenanceRespiration;
        public float LAI;
        public float Radiation;
        public float NetPsn;
        public float FolResp;
        public float GrossPsn;
        public float Transpiration;
        public float Folalloc;
        public float RootAlloc;
        public float WoodAlloc;
        public int LayerIndex;
        public float CumCohortBiomass;
        public float ReleasedNSC;

        Cohort cohort;

        public Species Species
        {
            get
            {
                return cohort.species;
            }
        }

        public SubCanopyLayer(Cohort cohort, int LayerIndex)
        {
            this.cohort = cohort;
            this.LayerIndex = LayerIndex;
        }
        static float GetFWater(Species species, float pressurehead)
        {
            if (pressurehead < 0 || pressurehead > species.H4) return 0;
            else if (pressurehead > species.H3) return 1 - ((pressurehead - species.H3) / (species.H4 - species.H3));
            else if (pressurehead < species.H2) return pressurehead / species.H2;
            else return 1;
        }
        
    }
}
