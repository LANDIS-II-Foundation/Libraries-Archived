namespace Landis.Cohorts
{
	/// <summary>
	/// Interface to a succession component's cohorts.  T is cohort type.
	/// </summary>
	public interface ISuccession<T>
	{
		/// <summary>
		/// Gets the cohorts for all the sites on the landscape.
		/// </summary>
		ILandscapeCohorts<T> Cohorts
		{
			get;
		}
	}
}
