// Copyright 2005 University of Wisconsin

using Landis.Library.AgeOnlyCohorts;

namespace Landis.Library.SiteHarvest
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
