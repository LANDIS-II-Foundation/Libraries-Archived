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
		/// Gets the cohorts for a particular species.
		/// </summary>
		ISpeciesCohorts<T> this[ISpecies species]
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Adds a new cohort for a particular species.
		/// </summary>
		void AddNewCohort(ISpecies species);

		//---------------------------------------------------------------------

		/// <summary>
		/// Is at least one sexually mature cohort present for a particular
		/// species?
		/// </summary>
		bool IsMaturePresent(ISpecies species);
	}
}
