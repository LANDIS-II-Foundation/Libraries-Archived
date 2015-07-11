// Copyright 2005 University of Wisconsin

using Landis.Library.AgeOnlyCohorts;

namespace Landis.Library.Harvest
{
    /// <summary>
    /// Removes all the cohorts at a site.
    /// </summary>
    public class ClearCut
        : ICohortSelector
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ClearCut()
        {
        }

        //---------------------------------------------------------------------

    	/// <summary>
    	/// Selects which of a species' cohorts are harvested.
    	/// </summary>
    	public void Harvest(ISpeciesCohorts         cohorts,
                            ISpeciesCohortBoolArray isHarvested)
    	{
    	    for (int i = 0; i < isHarvested.Count; i++)
    	        isHarvested[i] = true;
    	}
    }
}
