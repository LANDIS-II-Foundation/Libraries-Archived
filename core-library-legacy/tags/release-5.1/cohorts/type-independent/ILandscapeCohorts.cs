using Landis.Landscape;

namespace Landis.Cohorts.TypeIndependent
{
	/// <summary>
	/// The cohorts for all the sites in the landscape.
	/// </summary>
	public interface ILandscapeCohorts
	{
	    /// <summary>
	    /// The attributes that each cohort has.
	    /// </summary>
	    CohortAttribute[] CohortAttributes
	    {
	        get;
	    }

	    //---------------------------------------------------------------------

	    /// <summary>
		/// Gets the cohorts for a particular site.
		/// </summary>
		ISiteCohorts this[Site site]
		{
			get;
		}
	}
}
