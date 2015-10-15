using Landis.Core;
using System.Collections;
using Landis.SpatialModeling;

namespace Landis.Library.Succession
{
    /// <summary>
    /// A base class for forms of reproduction.
    /// </summary>
    public abstract class FormOfReproduction
        : IFormOfReproduction
    {
        //private static Species.IDataset speciesDataset;
        private static ISpeciesDataset speciesDataset;

        //---------------------------------------------------------------------

        static FormOfReproduction()
        {
            speciesDataset = Model.Core.Species;
        }

        //---------------------------------------------------------------------

        private ISiteVar<BitArray> selectedSpecies;

        //---------------------------------------------------------------------

        /// <summary>
        /// The species that have been selected for this form of reproduction
        /// at each active site.
        /// </summary>
        protected ISiteVar<BitArray> SelectedSpecies
        {
            get {
                return selectedSpecies;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// By default, if a form of reproduciton succeeds at a site, it
        /// precludes trying any other forms that haven't been tried yet.
        /// </summary>
        public virtual bool PrecludeRemainingForms
        {
            get {
                return true;
            }
        }

        //---------------------------------------------------------------------

        protected FormOfReproduction()
        {
            int speciesCount = speciesDataset.Count;
            selectedSpecies = Model.Core.Landscape.NewSiteVar<BitArray>();
            foreach (ActiveSite site in Model.Core.Landscape.ActiveSites) {
                selectedSpecies[site] = new BitArray(speciesCount);
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Are the preconditions for this form of reproduction satisified at
        /// a site for a particular species?
        /// </summary>
        protected abstract bool PreconditionsSatisfied(ISpecies   species, ActiveSite site);

        //---------------------------------------------------------------------

        bool IFormOfReproduction.TryAt(ActiveSite site)
        {
            bool success = false;
            BitArray selectedSpeciesAtSite = selectedSpecies[site];

            for (int index = 0; index < speciesDataset.Count; ++index) {
                if (selectedSpeciesAtSite.Get(index)) {
                    ISpecies species = speciesDataset[index];
                    if (PreconditionsSatisfied(species, site)) {
                        Reproduction.AddNewCohort(species, site);
                        success = true;
                    }
                }
            }

            return success;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Clears the list of selected species at a site.
        /// </summary>
        protected void ClearSpeciesAt(ActiveSite site)
        {
            selectedSpecies[site].SetAll(false);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Resets the form of reproduction at a site because it will not be
        /// tried.
        /// </summary>
        /// <param name="site">
        /// The site where the form of reproduction will not be tried.
        /// </param>
        void IFormOfReproduction.NotTriedAt(ActiveSite site)
        {
            ClearSpeciesAt(site);
        }
    }
}
