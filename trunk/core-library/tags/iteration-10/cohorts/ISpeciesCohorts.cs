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
		/// Is at least one sexually mature cohort present?
		/// </summary>
		bool IsMaturePresent
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The age at a particular index in the collection.
		/// </summary>
		/// <exception cref="System.IndexOutOfRangeException">
		/// index is negative, or it is = or > Count.
		/// </exception>
		ushort this[int index]
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
