using Landis.Landscape;
using Landis.Species;

namespace Landis.Succession
{
	/// <summary>
	/// Seed disperal methods.
	/// </summary>
	public static class SeedDisperal
	{
		/// <summary>
		/// A seed dispersal method.
		/// </summary>
		/// <returns>
		/// The probability that the species at the source site will disperse
		/// seeds into the destination site.
		/// </returns>
		public delegate float Method(ISpecies   species,
		                             ActiveSite sourceSite,
		                             ActiveSite destinationSite);

		//---------------------------------------------------------------------

		/// <summary>
		/// Default seed dispersal method.
		/// </summary>
		public static float Default(ISpecies   species,
		                            ActiveSite sourceSite,
		                            ActiveSite destinationSite)
		{
			// TODO - Based on Brendan Ward's thesis
			return 0.0f;
		}
	}
}
