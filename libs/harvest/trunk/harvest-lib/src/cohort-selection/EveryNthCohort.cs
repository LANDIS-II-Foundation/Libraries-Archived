using Landis.AgeCohort;

namespace Landis.Harvest
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
