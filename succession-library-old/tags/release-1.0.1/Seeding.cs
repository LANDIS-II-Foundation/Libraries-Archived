using Landis.Cohorts;
using Landis.Landscape;
using Landis.Species;

namespace Landis.Succession
{
	public class Seeding
	{
		private SeedingAlgorithm seedingAlgorithm;
		private ILandscapeCohorts<AgeCohort.ICohort> cohorts;

		//---------------------------------------------------------------------

		public Seeding(SeedingAlgorithm seedingAlgorithm)
		{
			this.seedingAlgorithm = seedingAlgorithm;
			this.cohorts = Model.GetSuccession<AgeCohort.ICohort>().Cohorts;
		}

		//---------------------------------------------------------------------

		public void Do(ActiveSite site)
		{
			for (int i = 0; i < Model.Species.Count; i++) {
				ISpecies species = Model.Species[i];
				if (seedingAlgorithm(species, site))
					Reproduction.AddNewCohort(species, site);
			}
		}
	}
}
