using Landis.Landscape;
using Landis.Species;

namespace Landis.Succession
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
        public static bool SufficientLight(ISpecies   species,
                                           ActiveSite site)
        {
            byte siteShade = SiteVars.Shade[site];
            bool sufficientLight;
            sufficientLight = (species.ShadeTolerance <= 4 && species.ShadeTolerance > siteShade) ||
                   (species.ShadeTolerance == 5 && siteShade > 1);
            //  pg 14, Model description, this ----------------^ may be 2?
            return sufficientLight;
        }
    }
}
