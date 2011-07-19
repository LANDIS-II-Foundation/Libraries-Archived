using System.Collections.Generic;

namespace Landis.SiteInitialization
{
	/// <summary>
	/// The cohorts initially at a site.
	/// </summary>
	public interface ISiteCohorts
	{
		/// <summary>
		/// Accesses the site cohorts as a collection of species cohorts.
		/// </summary>
		IEnumerable<ISpeciesCohorts> BySpecies
		{
			get;
		}
	}
}
