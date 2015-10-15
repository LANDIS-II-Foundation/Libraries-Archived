//using Landis.Species;
using Landis.Core;
using Wisc.Flel.GeospatialModeling.Landscapes;

namespace Landis.Library.Succession
{
    /// <summary>
    /// Default implementations of some of the reproduction delegates.
    /// </summary>
    public static class ReproductionDefaults
    {
        /// <summary>
        /// The default method for determining if there is sufficient light at
        /// a site for a species to germinate/resprout.
        /// </summary>
        public static bool SufficientResources(ISpecies   species,
                                               ActiveSite site)
        {
            byte siteShade = SiteVars.Shade[site];
            bool sufficientLight;
            sufficientLight = (species.ShadeTolerance <= 4 && species.ShadeTolerance > siteShade) ||
                   (species.ShadeTolerance == 5 && siteShade > 1);
            //  pg 14, Model description, this ----------------^ may be 2?
            return sufficientLight;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if a species can establish on a site.
        /// </summary>
        public static bool Establish(ISpecies species, ActiveSite site)
        //public static bool Establish(double[,] establishment)
        {
            double establishProbability = 0; // Reproduction.GetEstablishProbability(species, site);

            //return Landis.Model.GenerateUniform() < establishment;

            return Model.Core.GenerateUniform() < establishProbability;
        }

        //---------------------------------------------------------------------
    }
}
