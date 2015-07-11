using Landis.Species;
using System.Collections.Generic;

namespace Landis.Cohorts
{
	/// <summary>
	/// The cohorts for a particular species at a site.
	/// </summary>
	public interface ISpeciesCohorts<TCohort>
		: IEnumerable<TCohort>
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
		/// Removes the cohorts which satisfy a certain selection criteria.
		/// </summary>
		/// <param name="selectMethod">
		/// A selection method that returns true if the cohort passed to it as
		/// a parameter should be removed from the site.
		/// </param>
		void Remove(SelectMethod<TCohort> selectMethod);
	}
}
