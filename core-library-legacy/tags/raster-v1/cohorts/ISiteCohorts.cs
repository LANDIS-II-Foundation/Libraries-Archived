using Landis.Species;
using System.Collections.Generic;

namespace Landis
{
	/// <summary>
	/// All the cohorts initially at a site.  T is basic cohort interface.
	/// </summary>
	public interface ISiteCohorts<T>
		: ISiteCollection<T>
	{
		/// <summary>
		/// Accesses the site cohorts as a collection of species cohorts.
		/// </summary>
		IEnumerable<ISpeciesCohorts<T>> BySpecies
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Accesses as a collection of species present on the site.
		/// </summary>
		IEnumerable<ISpecies> SpeciesPresent
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Adds a new cohort for a particular species.
		/// </summary>
		void AddNewCohort(ISpecies species);
	}
}
