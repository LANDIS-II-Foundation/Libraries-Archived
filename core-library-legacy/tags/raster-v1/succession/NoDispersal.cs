using Landis.Species;
using Landis.Landscape;

namespace Landis.Succession
{
	/// <summary>
	/// Seeding algorithm where no species can seed a neighboring site.
	/// </summary>
	public class NoDispersal
		: ISeeding
	{
		public bool Seeds(ISpecies   species,
		           		  ActiveSite site)
		{
			return false;
		}
	}
}
