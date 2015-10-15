using Landis.Species;
using Landis.Landscape;

namespace Landis.Succession
{
    /// <summary>
    /// Seeding algorithm where every species can seed any site; a species does
    /// not even need to be present in any neighboring site.
    /// </summary>
    public static class UniversalDispersal
    {
        public static void Algorithm(//ISpecies   species,
                                     ActiveSite site)
        {
            foreach(ISpecies species in Model.Core.Species)
                    if(Reproduction.SufficientLight(species, site) && Reproduction.Establish(species, site))
                        Reproduction.AddNewCohort(species, site);
            //return Reproduction.SufficientLight(species, site) &&
            //       Reproduction.Establish(species, site);
        }
    }
}
