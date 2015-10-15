//using Landis.Species;
using Landis.Core;
using Wisc.Flel.GeospatialModeling.Landscapes;

namespace Landis.Library.Succession
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
            return Reproduction.SufficientResources(species, site) &&
                   Reproduction.Establish(species, site);
        }
    }
}
