using Landis.Landscape;
using Landis.Species;

namespace Landis.Succession
{
	public class Seeding
		: IReproduction
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
			foreach (ISpecies species in Model.Species) {
				if (seedingAlgorithm(species, site))
					cohorts[site].AddNewCohort(species);
			}
		}
	}
}
