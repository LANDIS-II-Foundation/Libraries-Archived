// Copyright 2005 University of Wisconsin

using Landis.Library.AgeOnlyCohorts;
using System.Collections.Generic;

namespace Landis.Library.Harvest
{
    /// <summary>
    /// Selects specific ages and ranges of ages among a species' cohorts
    /// for harvesting.
    /// </summary>
    public class SpecificAgesCohortSelector
    {
        private AgesAndRanges agesAndRanges;

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public SpecificAgesCohortSelector(IList<ushort> ages,
                                          IList<AgeRange> ranges)
        {
            agesAndRanges = new AgesAndRanges(ages, ranges);
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
                AgeRange? notUsed;
    	        if (agesAndRanges.Contains(cohort.Age, out notUsed))
    	            isHarvested[i] = true;
    	        i++;
    	    }
    	}
    }
}
