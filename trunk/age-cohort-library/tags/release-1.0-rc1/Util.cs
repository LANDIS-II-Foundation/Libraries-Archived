using Landis.Landscape;
using Landis.Species;

namespace Landis.AgeCohort
{
	/// <summary>
	/// Utility methods for age-only cohorts.
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Gets the maximum age among a species' cohorts.
		/// </summary>
		/// <returns>
		/// The age of the oldest cohort or 0 if there are no cohorts.
		/// </returns>
		public static ushort GetMaxAge(ISpeciesCohorts<ICohort> cohorts)
		{
			if (cohorts == null)
				return 0;
			ushort max = 0;
			foreach (ushort age in cohorts.Ages)
				if (age > max)
					max = age;
			return max;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets the maximum age among all cohorts at a site.
		/// </summary>
		/// <returns>
		/// The age of the oldest cohort or 0 if there are no cohorts.
		/// </returns>
		public static ushort GetMaxAge(ISiteCohorts<ICohort> cohorts)
		{
			if (cohorts == null)
				return 0;
			ushort max = 0;
			foreach (ISpeciesCohorts<ICohort> speciesCohorts in cohorts.BySpecies) {
				ushort maxSpeciesAge = GetMaxAge(speciesCohorts);
				if (maxSpeciesAge > max)
					max = maxSpeciesAge;
			}
			return max;
		}
	}
}
