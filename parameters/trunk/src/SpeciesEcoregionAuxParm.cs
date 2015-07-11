using Landis.Core;

namespace Landis.Library.Parameters
{
   public class SpeciesEcoregionAuxParm<T>
    {
       Parameters.Species.AuxParm<Parameters.Ecoregions.AuxParm<T>> values;
       ///<Summary>
       /// Gets a species and ecoregion specific value
       ///</Summary>
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
        ///<Summary>
        /// Initializes a species and ecoregion specific parameter
        ///</Summary>
        public SpeciesEcoregionAuxParm(ISpeciesDataset speciesDataset, IEcoregionDataset ecoregionDataset)
        {
            values = new Parameters.Species.AuxParm<Parameters.Ecoregions.AuxParm<T>>(speciesDataset);
            foreach (ISpecies species in speciesDataset)
            {
                values[species] = new Parameters.Ecoregions.AuxParm<T>(ecoregionDataset);
            }
        }
    }
}
