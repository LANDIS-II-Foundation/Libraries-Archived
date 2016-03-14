using Landis.Landscape;

namespace Landis.Cohorts
{
	/// <summary>
	/// The cohorts for all the sites in the landscape.
	/// </summary>
	public interface ILandscapeCohorts<TCohort>
	{
		/// <summary>
		/// Gets the cohorts for a particular site.
		/// </summary>
		ISiteCohorts<TCohort> this[Site site]
		{
			get;
		}
	}
}
