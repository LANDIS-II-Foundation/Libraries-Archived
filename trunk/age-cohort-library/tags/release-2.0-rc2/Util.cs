namespace Landis.AgeCohort
{
	/// <summary>
	/// Utility methods for age cohorts.
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Gets the maximum age among a species' cohorts.
		/// </summary>
		/// <returns>
		/// The age of the oldest cohort or 0 if there are no cohorts.
		/// </returns>
		public static ushort GetMaxAge(ISpeciesCohorts cohorts)
		{
			if (cohorts == null)
				return 0;
			ushort max = 0;
			foreach (ICohort cohort in cohorts)
				if (cohort.Age > max)
					max = cohort.Age;
			return max;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets the maximum age among all cohorts at a site.
		/// </summary>
		/// <returns>
		/// The age of the oldest cohort or 0 if there are no cohorts.
		/// </returns>
		public static ushort GetMaxAge(ISiteCohorts cohorts)
		{
			if (cohorts == null)
				return 0;
			ushort max = 0;
			foreach (ISpeciesCohorts speciesCohorts in cohorts) {
				ushort maxSpeciesAge = GetMaxAge(speciesCohorts);
				if (maxSpeciesAge > max)
					max = maxSpeciesAge;
			}
			return max;
		}
	}
}
