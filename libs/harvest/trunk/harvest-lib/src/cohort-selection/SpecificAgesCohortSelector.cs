using Landis.AgeCohort;
using Landis.Species;

using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// Selects specific ages and ranges of ages among a species' cohorts
    /// for harvesting.
    /// </summary>
    public class SpecificAgesCohortSelector
    {
        private List<ushort> ages;
        private List<AgeRange> ranges;

        //---------------------------------------------------------------------

        public SpecificAgesCohortSelector(List<ushort>   ages,
                                          List<AgeRange> ranges)
        {
            this.ages = new List<ushort>(ages);
            this.ranges = new List<AgeRange>(ranges);
        }

        //---------------------------------------------------------------------

    	/// <summary>
    	/// Selects which of a species' cohorts are harvested.
    	/// </summary>
    	public void SelectCohorts(ISpeciesCohorts         cohorts,
                                  ISpeciesCohortBoolArray isHarvested)
    	{
    	    int i = 0;
    	    foreach (ICohort cohort in cohorts) {
    	        if (ages.Contains(cohort.Age))
    	            isHarvested[i] = true;
    	        else {
    	            foreach (AgeRange range in ranges) {
    	                if (range.Contains(cohort.Age)) {
    	                    isHarvested[i] = true;
    	                    break;
    	                }
    	            }
    	        }
    	        i++;
    	    }
    	}
    }
}
