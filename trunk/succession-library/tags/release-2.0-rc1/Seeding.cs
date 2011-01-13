using Landis.Landscape;
using Landis.Species;

namespace Landis.Succession
{
	public class Seeding
	{
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
				if (seedingAlgorithm(species, site))
					Reproduction.AddNewCohort(species, site);
			}
		}
	}
}
