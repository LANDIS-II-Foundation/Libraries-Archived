using Landis.Species;

namespace Landis.AgeCohort
{
	/// <summary>
	/// A species cohort with only age information.
	/// </summary>
	public interface ICohort
	{
		/// <summary>
		/// The cohort's species.
		/// </summary>
		ISpecies Species
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The cohort's age (years).
		/// </summary>
		ushort Age
		{
			get;
		}
	}
}
