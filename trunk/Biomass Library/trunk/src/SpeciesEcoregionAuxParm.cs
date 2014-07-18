using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.SpatialModeling;
using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Library.Biomass
{
   public class SpeciesEcoregionAuxParm<T>
    {
       Landis.Library.Biomass.Species.AuxParm<Landis.Library.Biomass.Ecoregions.AuxParm<T>> values;

        /*public Landis.Library.Biomass.Species.AuxParm<Landis.Library.Biomass.Ecoregions.AuxParm<T>> Parm
        {
            get
            {
                return values;
            }
            set
            {
                values = value;
            }
        }
         * */

        public T this[ISpecies species, IEcoregion ecoregion]
        {
            get
            {
                return values[species][ecoregion];
            }

            set
            {
                values[species][ecoregion] = value;
            }
        }

        public SpeciesEcoregionAuxParm(ISpeciesDataset speciesDataset, IEcoregionDataset ecoregionDataset)
        {
            values = new Landis.Library.Biomass.Species.AuxParm<Landis.Library.Biomass.Ecoregions.AuxParm<T>>(speciesDataset);
            foreach (ISpecies species in speciesDataset)
            {
                values[species] = new Landis.Library.Biomass.Ecoregions.AuxParm<T>(ecoregionDataset);
            }
        }
        //---------------------------------------------------------------------
    }
}
