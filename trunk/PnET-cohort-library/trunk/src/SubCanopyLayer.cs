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
        

        public float LAI;
       
        public float WoodAlloc;
        public float MaintenanceRespiration;
        public float Fol;




        public float Radiation;


        public float NetPsn;



        public float FolResp;



        public float GrossPsn;



        public float Transpiration;


        public float Folalloc;



        public float RootAlloc;



        public float CumCohortBiomass;




        public float ReleasedNSC;
        

        
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
