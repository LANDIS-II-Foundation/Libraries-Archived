using Landis.Core;
using Landis.SpatialModeling;
using Seed_Dispersal;

namespace Landis.Library.Succession
{
    public class DemographicSeeding
    {
        private Seed_Dispersal.Map seedDispersalMap;

        public DemographicSeeding()
        {
            seedDispersalMap = new Seed_Dispersal.Map();
        }

        /// <summary>
        /// Seeding algorithm: determines if a species seeds a site.
        /// <param name="species"></param>
        /// <param name="site">Site that may be seeded.</param>
        /// <returns>true if the species seeds the site.</returns>
        public bool Algorithm(ISpecies   species,
                              ActiveSite site)
        {
            throw new System.NotImplementedException();
        }
    }
}
