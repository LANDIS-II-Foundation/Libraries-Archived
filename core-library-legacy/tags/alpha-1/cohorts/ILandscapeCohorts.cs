using Landis.Landscape;

namespace Landis
{
	/// <summary>
	/// The cohorts for all the sites in the landscape.  T is cohort type.
	/// </summary>
	public interface ILandscapeCohorts<T>
	{
		/// <summary>
		/// Gets the cohorts for a particular site.
		/// </summary>
		ISiteCohorts<T> this[Site site]
		{
			get;
		}
	}
}
