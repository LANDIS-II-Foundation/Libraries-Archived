// Copyright 2005 University of Wisconsin

using Landis.Library.AgeOnlyCohorts;

namespace Landis.Library.SiteHarvest
{
    /// <summary>
    /// Selects every Nth cohort among a species' cohorts for harvesting.
    /// </summary>
    /// <remarks>
    /// The cohorts are traversed from youngest to oldest.
    /// </remarks>
    public class EveryNthCohort
    {
        private int N;

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public EveryNthCohort(int N)
        {
            this.N = N;
        }

        //---------------------------------------------------------------------

    	/// <summary>
    	/// Selects which of a species' cohorts are harvested.
    	/// </summary>
    	public void SelectCohorts(ISpeciesCohorts         cohorts,
                                  ISpeciesCohortBoolArray isHarvested)
    	{
    	    for (int i = isHarvested.Count - N; i >= 0; i -= N)
    	        isHarvested[i] = true;
    	}
    }
}
