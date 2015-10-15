namespace Landis.Biomass
{
	/// <summary>
	/// All the biomass cohorts at a site.
	/// </summary>
	public interface ISiteCohorts
		: Landis.Cohorts.ISiteCohorts<ISpeciesCohorts>
	{
        /// <summary>
        /// Computes who much a disturbance damages the cohorts by reducing
        /// their biomass.
        /// </summary>
        /// <returns>
        /// The total of all the cohorts' biomass reductions.
        /// </returns>
        int DamageBy(IDisturbance disturbance);
	}
}
