using Landis.Species;
using System.Collections.Generic;

namespace Landis.SiteInitialization
{
	/// <summary>
	/// The cohorts for a particular species.
	/// </summary>
	public interface ISpeciesCohorts
	{
		/// <summary>
		/// The species of the cohorts.
		/// </summary>
		ISpecies Species
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Accesses the species' cohorts as a collection of ages.
		/// </summary>
		IEnumerable<int> Ages
		{
			get;
		}
	}
}
