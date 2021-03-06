using Landis;
using Landis.Ecoregions;
using Landis.Species;
using Landis.Landscape;
using Landis.Util;

namespace Landis.Succession
{
	public static class Reproduction
	{
		private static double[,] establishProbabilities;

		//---------------------------------------------------------------------

		public static void SetEstablishProbabilities(double[,] probabilities)
		{
			establishProbabilities = probabilities;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Determines if a species resprouts when one of its cohorts dies.
		/// </summary>
		public static bool Resprout(AgeOnly.ICohort cohort,
		                            ActiveSite      site)
		{
			ISpecies species = cohort.Species;
			return (species.MinSproutAge <= cohort.Age) &&
			       (cohort.Age <= species.MaxSproutAge) &&
				   SufficientLight(species, site) &&
				   Establish(species, site);
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
		/// Determines if a species itself on a site.
		/// </summary>
		public static bool Establish(ISpecies   species,
		                             ActiveSite site)
		{
			double establishProbability = establishProbabilities[Model.SiteVars.Ecoregion[site].Index, species.Index];
			return Random.GenerateUniform() < establishProbability;
		}
	}
}
