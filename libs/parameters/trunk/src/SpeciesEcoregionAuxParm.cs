using Landis.Core;

namespace Landis.Library.Parameters
{
    ///<Summary>
    /// Creates a species/ecoregion specific parameter
    ///</Summary>
   public class SpeciesEcoregionAuxParm<T>
    {
        
       Parameters.Species.AuxParm<Parameters.Ecoregions.AuxParm<T>> values;

       ///<Summary>
       /// Gets a template-defined value specific to species and ecoregion
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
        /// Initialized a species and ecoregion specific parameter
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
