using Landis.Species;

namespace Landis.AgeCohort
{
	/// <summary>
	/// A species cohort with only age information.
	/// </summary>
	public interface ICohort
	    : Cohorts.ICohort
	{
		/// <summary>
		/// The cohort's age (years).
		/// </summary>
		ushort Age
		{
			get;
		}
	}
}
