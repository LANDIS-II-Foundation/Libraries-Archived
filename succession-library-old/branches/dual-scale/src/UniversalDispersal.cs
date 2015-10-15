using Landis.Species;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Succession
{
    /// <summary>
    /// Seeding algorithm where every species can seed any site; a species does
    /// not even need to be present in any neighboring site.
    /// </summary>
    public static class UniversalDispersal
    {
        public static bool Algorithm(ISpecies   species,
                                     ActiveSite site)
        {
            return Reproduction.SufficientLight(species, site) &&
                   Reproduction.Establish(species, site);
        }
    }
}
