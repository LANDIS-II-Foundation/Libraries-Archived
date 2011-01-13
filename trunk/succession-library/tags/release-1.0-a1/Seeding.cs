using Landis.Landscape;
using Landis.Species;

namespace Landis.Succession
{
	public class Seeding
	{
		private SeedingAlgorithm seedingAlgorithm;
		private ILandscapeCohorts<AgeOnly.ICohort> cohorts;

		//---------------------------------------------------------------------

		public Seeding(SeedingAlgorithm seedingAlgorithm)
		{
			this.seedingAlgorithm = seedingAlgorithm;
			this.cohorts = Model.GetSuccession<AgeOnly.ICohort>().Cohorts;
		}

		//---------------------------------------------------------------------

		public void Do(ActiveSite site)
		{
			for (int i = 0; i < Model.Species.Count; i++) {
				ISpecies species = Model.Species[i];
				if (seedingAlgorithm(species, site))
					cohorts[site].AddNewCohort(species);
			}
		}
	}
}
