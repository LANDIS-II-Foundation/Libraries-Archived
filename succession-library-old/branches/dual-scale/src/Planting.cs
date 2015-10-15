using Landis.Species;
using System.Collections;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Succession
{
    /// <summary>
    /// The form of reproduction that represents the planting of trees after
    /// a harvest.
    /// </summary>
    public class Planting
        : FormOfReproduction, IPlanting
    {
        /// <summary>
        /// A list of species to be planted
        /// </summary>
        public class SpeciesList
        {
            private BitArray bitArray;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public SpeciesList(IEnumerable<ISpecies> speciesList,
                               Species.IDataset      speciesDataset)
            {
                bitArray = new BitArray(speciesDataset.Count);
                if (speciesList != null) {
                    foreach (ISpecies species in speciesList) {
                        bitArray.Set(species.Index, true);
                    }
                }
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            internal BitArray AsBitArray
            {
                get {
                    return bitArray;
                }
            }
        }

        //---------------------------------------------------------------------

        public Planting()
            : base()
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Are the conditions necessary for planting a species at site
        /// satified?
        /// </summary>
        protected override bool PreconditionsSatisfied(ISpecies   species,
                                                       ActiveSite site)
        {
            return Reproduction.GetEstablishProbability(species, site) > 0;
        }

        //---------------------------------------------------------------------

        void IPlanting.Schedule(SpeciesList speciesList,
                                ActiveSite  site)
        {
            SelectedSpecies[site].Or(speciesList.AsBitArray);
        }
    }
}
