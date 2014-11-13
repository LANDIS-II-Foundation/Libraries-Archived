using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Linq;
 

namespace Landis.Library.BiomassCohortsPnET
{
    public class SubCanopyLayer
    {
        static float log2 = (float)System.Math.Log(2);
        public float Fol;
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
        public int CanopyLayer;

        public float CumCohortBiomass;
        public float ReleasedNSC;
        public float RootPlusWoodAlloc;
        public Cohort cohort;
       
        public ISpecies Species
        {
            get
            {
                return cohort.Species;
            }
        }

        public SubCanopyLayer(Cohort cohort, int LayerIndex)
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
