using Landis.Species;
using System.Collections.Generic;

namespace Landis
{
	/// <summary>
	/// The cohorts for a particular species at a site.  T is cohort type.
	/// </summary>
	public interface ISpeciesCohorts<T>
		: ISiteCollection<T>
	{
		/// <summary>
		/// The number of cohorts in the collection.
		/// </summary>
		int Count
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The species of the cohorts.
		/// </summary>
		ISpecies Species
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Accesses the cohorts as a collection of ages.
		/// </summary>
		IEnumerable<ushort> Ages
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Adds a new cohort.
		/// </summary>
		void AddNewCohort();
	}
}
