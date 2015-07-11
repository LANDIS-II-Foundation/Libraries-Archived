using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Linq;
using System.Diagnostics;

namespace Landis.Library.BiomassCohortsPnET
{
    public class SubCanopyLayer
    {
        public Cohort cohort;

        public byte LayerIndex;
        public byte CanopyLayer;

        public ushort CumCohortBiomass;
        public ushort  Radiation;
        public float Fwater;
        public float Frad;
        public float LAI;
        public float NetPsn;
        public float FolResp;
        public float Transpiration;
        
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
