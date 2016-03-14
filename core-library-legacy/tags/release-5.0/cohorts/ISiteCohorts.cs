using Landis.Species;
using System.Collections.Generic;

namespace Landis.Cohorts
{
	/// <summary>
	/// All the cohorts initially at a site.
	/// </summary>
	public interface ISiteCohorts<TCohort>
		: IEnumerable<ISpeciesCohorts<TCohort>>
	{
		/// <summary>
		/// Gets the cohorts for a particular species.
		/// </summary>
		ISpeciesCohorts<TCohort> this[ISpecies species]
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Is at least one sexually mature cohort present for a particular
		/// species?
		/// </summary>
		bool IsMaturePresent(ISpecies species);

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
