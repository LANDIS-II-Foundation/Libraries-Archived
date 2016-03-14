using Landis.Species;
using Landis.Landscape;

namespace Landis.Succession
{
	/// <summary>
	/// Seeding algorithm
	/// </summary>
	public interface ISeeding
	{
		/// <summary>
		/// Determines if a species seeds into a site.
		/// </summary>
		/// <param name="species"></param>
		/// <param name="site">Site that may be seeded.</param>
		/// <returns>true if the species seeds the site.</returns>
		bool Seeds(ISpecies   species,
		           ActiveSite site);
	}
}
