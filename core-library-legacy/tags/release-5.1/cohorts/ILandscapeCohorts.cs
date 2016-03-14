using Landis.Landscape;

namespace Landis.Cohorts
{
	/// <summary>
	/// The cohorts for all the sites in the landscape.
	/// </summary>
	public interface ILandscapeCohorts<TSiteCohorts>
	{
		/// <summary>
		/// Gets the cohorts for a particular site.
		/// </summary>
		TSiteCohorts this[Site site]
		{
			get;
		}
	}
}
