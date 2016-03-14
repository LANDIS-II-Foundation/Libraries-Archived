using Landis.Landscape;
using Landis.Species;
using System.Collections.Generic;

namespace Landis
{
	/// <summary>
	/// A collection of cohorts at a site.  T is basic cohort interface.
	/// </summary>
	public interface ISiteCollection<T>
		: IEnumerable<T>
	{
		/// <summary>
		/// Grows the cohorts for a period of time.
		/// </summary>
		/// <param name="years">
		/// The amount of time the cohorts grow.
		/// </param>
		/// <param name="currentSite">
		/// The site where the cohorts are located.
		/// </param>
		/// <param name="successionTimestep">
		/// The succession timestep (years).  If this parameter has a value,
		/// then the ageing is part of a succession timestep; therefore, all
		/// young cohorts (whose age is less than succession timestep) are
		/// combined into a single cohort whose age is the succession timestep.
		/// </param>
		void Grow(ushort     years,
		          ActiveSite currentSite,
		          int?       successionTimestep);

		//---------------------------------------------------------------------

		/// <summary>
		/// Removes the cohorts which satisfy a certain selection criteria.
		/// </summary>
		/// <param name="selectMethod">
		/// A selection method that returns true if the cohort passed to it as
		/// a parameter should be removed from the site.
		/// </param>
		/// <param name="currentSite">
		/// The site where the cohorts are located.
		/// </param>
		void Remove(SelectMethod<T> selectMethod,
		            ActiveSite      site);
	}
}
