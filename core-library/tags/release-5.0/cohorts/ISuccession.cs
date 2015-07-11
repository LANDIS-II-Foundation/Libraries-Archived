using Landis.Landscape;

namespace Landis.Cohorts
{
	/// <summary>
	/// Interface to the cohorts and their reproduction in a succession plug-in.
	/// </summary>
	public interface ISuccession<TCohort>
	{
		/// <summary>
		/// The cohorts for all the sites on the landscape.
		/// </summary>
		ILandscapeCohorts<TCohort> Cohorts
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Records a species that should be checked for resprouting during
		/// reproduction.
		/// </summary>
		/// <param name="cohort">
		/// The cohort whose death triggered the current call to this method.
		/// </param>
		/// <param name="site">
		/// The site where the cohort died.
		/// </param>
		/// <remarks>
		/// If the cohort's age is within the species' age range for
		/// resprouting, then the species will be have additional resprouting
		/// criteria (light, probability) checked during reproduction.
		/// </remarks>
		void CheckForResprouting(TCohort    cohort,
		                         ActiveSite site);

		//---------------------------------------------------------------------

		/// <summary>
		/// Records a species that should be checked for post-fire regeneration
		/// during reproduction.
		/// </summary>
		/// <param name="cohort">
		/// The cohort whose death triggered the current call to this method.
		/// </param>
		/// <param name="site">
		/// The site where the cohort died.
		/// </param>
		/// <remarks>
		/// If the cohort's species has resprouting as its post-fire
		/// regeneration, then the CheckForResprouting method is called with
		/// the cohort.  If the species has serotiny as its post-fire
		/// regeneration, then the species will be checked for on-site seeding
		/// during reproduction.
		/// </remarks>
		void CheckForPostFireRegen(TCohort    cohort,
		                           ActiveSite site);
	}
}
