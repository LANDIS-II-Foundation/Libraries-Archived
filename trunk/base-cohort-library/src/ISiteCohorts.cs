using Landis.Core;
using System.Collections.Generic;

namespace Landis.Library.Cohorts
{
	/// <summary>
	/// All the cohorts at a site.
	/// </summary>
	public interface ISiteCohorts<TSpeciesCohorts>
		: IEnumerable<TSpeciesCohorts>
	{
		/// <summary>
		/// Gets the cohorts for a particular species.
		/// </summary>
		TSpeciesCohorts this[ISpecies species]
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Is at least one sexually mature cohort present for a particular
		/// species?
		/// </summary>
		bool IsMaturePresent(ISpecies species);

	}
}
