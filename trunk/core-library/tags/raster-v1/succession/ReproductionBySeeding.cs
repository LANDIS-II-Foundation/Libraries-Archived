using Landis.Landscape;
using Landis.Species;

namespace Landis.Succession
{
	public class ReproductionBySeeding
		: IReproduction
	{
		private ISeeding seedingAlgorithm;
		private ILandscapeCohorts<AgeOnly.ICohort> cohorts;

		//---------------------------------------------------------------------

		public ReproductionBySeeding(ISeeding seedingAlgorithm)
		{
			this.seedingAlgorithm = seedingAlgorithm;
			this.cohorts = Model.GetSuccession<AgeOnly.ICohort>().Cohorts;
		}

		//---------------------------------------------------------------------

		public void Do(ActiveSite site)
		{
			foreach (ISpecies species in Model.Species) {
				if (seedingAlgorithm.Seeds(species, site))
					cohorts[site].AddNewCohort(species);
			}
		}
	}
}
