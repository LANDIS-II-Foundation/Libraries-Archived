using Landis.Species;
using Landis.Landscape;

namespace Landis.Succession
{
	/// <summary>
	/// Seeding algorithm: determines if a species seeds a site.
	/// <param name="species"></param>
	/// <param name="site">Site that may be seeded.</param>
	/// <returns>true if the species seeds the site.</returns>
	public delegate bool SeedingAlgorithm(ISpecies   species,
			                              ActiveSite site);
}
