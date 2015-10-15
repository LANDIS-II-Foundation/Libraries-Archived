using Landis.Landscape;
using Landis.Species;

using System.Reflection;
using log4net;

namespace Landis.Succession
{
    public class Seeding
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        private SeedingAlgorithm seedingAlgorithm;

        //---------------------------------------------------------------------

        public Seeding(SeedingAlgorithm seedingAlgorithm)
        {
            this.seedingAlgorithm = seedingAlgorithm;
        }

        //---------------------------------------------------------------------

        public void Do(ActiveSite site)
        {
            for (int i = 0; i < Model.Core.Species.Count; i++) {
                ISpecies species = Model.Core.Species[i];
                if (seedingAlgorithm(species, site)) {
                    Reproduction.AddNewCohort(species, site);
                    if (isDebugEnabled)
                        log.DebugFormat("site {0}: seeded {1}",
                                        site.Location, species.Name);
                }
            }
        }
    }
}
