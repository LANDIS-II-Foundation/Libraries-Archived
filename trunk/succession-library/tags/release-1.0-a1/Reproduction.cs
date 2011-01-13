using Landis;
using Landis.Ecoregions;
using Landis.Landscape;
using Landis.Species;
using Landis.Util;
using System.Collections;

namespace Landis.Succession
{
	public static class Reproduction
	{
		private static double[,] establishProbabilities;
		private static Seeding seeding;
		private static ILandscapeCohorts<AgeOnly.ICohort> cohorts;
		private static Species.IDataset speciesDataset;
		private static ISiteVar<BitArray> resprout;

		//---------------------------------------------------------------------

		public static void Initialize(double[,]        establishProbabilities,
		                              SeedingAlgorithm seedingAlgorithm)
		{
			Reproduction.establishProbabilities = establishProbabilities;
			seeding = new Seeding(seedingAlgorithm);
			cohorts = Model.GetSuccession<AgeOnly.ICohort>().Cohorts;

			speciesDataset = Model.Species;
			int speciesCount = speciesDataset.Count;
			resprout = Model.Landscape.NewSiteVar<BitArray>();
			foreach (ActiveSite site in Model.Landscape.ActiveSites)
				resprout[site] = new BitArray(speciesCount);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Called when a cohort dies at a site (for example, when a
		/// disturbance kills a cohort).
		/// </summary>
		/// <remarks>
		/// The cohort is checked to see if its age permits resprouting.  If
		/// its age is within the resprouting age-range for the species, the
		/// species is recorded to be checked for sufficient light and
		/// establishment during reproduction stage of succession.
		/// </remarks>
		public static void CohortDeath(AgeOnly.ICohort cohort,
		                               ActiveSite      site)
		{
			ISpecies species = cohort.Species;
			if (species.MinSproutAge <= cohort.Age && cohort.Age <= species.MaxSproutAge)
				resprout[site].Set(species.Index, true);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Does the appropriate forms of reproduction at a site.
		/// </summary>
		public static void Do(ActiveSite site)
		{
			bool speciesResprouted = false;
			for (int index = 0; index < speciesDataset.Count; ++index) {
				if (resprout[site].Get(index)) {
					ISpecies species = speciesDataset[index];
					if (SufficientLight(species, site) && Establish(species, site)) {
						cohorts[site].AddNewCohort(species);
						speciesResprouted = true;
					}
				}
			}
			resprout[site].SetAll(false);

			if (! speciesResprouted)
				seeding.Do(site);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Determines if there is sufficient light at a site for a species to
		/// germinate/resprout.
		/// </summary>
		public static bool SufficientLight(ISpecies   species,
		                                   ActiveSite site)
		{
			byte siteShade = SiteVars.Shade[site];
			return (species.ShadeTolerance <= 4 && species.ShadeTolerance > siteShade) ||
				   (species.ShadeTolerance == 5 && siteShade > 1);
			//  pg 14, Model description, this ----------------^ may be 2?
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Determines if a species can establish on a site.
		/// </summary>
		public static bool Establish(ISpecies   species,
		                             ActiveSite site)
		{
			double establishProbability = establishProbabilities[Model.SiteVars.Ecoregion[site].Index, species.Index];
			return Random.GenerateUniform() < establishProbability;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Determines if a species has at least one mature cohort at a site.
		/// </summary>
		public static bool MaturePresent(ISpecies   species,
		                                 Site       site)
		{
			return cohorts[site].IsMaturePresent(species);
		}
	}
}
