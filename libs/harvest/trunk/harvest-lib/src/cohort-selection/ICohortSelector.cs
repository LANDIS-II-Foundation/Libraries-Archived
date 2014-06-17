using Landis.AgeCohort;

namespace Landis.Harvest
{
    /// <summary>
    /// Selects which cohorts at a site are removed by a harvest event.
    /// </summary>
    public interface ICohortSelector
    {
    	/// <summary>
    	/// Selects which of a species' cohorts are harvested.
    	/// </summary>
    	void Harvest(ISpeciesCohorts         cohorts,
                     ISpeciesCohortBoolArray isHarvested);
    }
}
