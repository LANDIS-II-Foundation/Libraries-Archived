// Copyright 2005 University of Wisconsin
// Copyright 2014 University of Notre Dame

using System.Collections.Generic;

namespace Landis.Library.Harvest
{
    /// <summary>
    /// A set of specific ages and ranges of ages for one species' cohorts.
    /// </summary>
    public class AgesAndRanges
    {
        private List<ushort> ages;
        private List<AgeRange> ranges;

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AgesAndRanges(IList<ushort> ages,
                             IList<AgeRange> ranges)
        {
            this.ages = new List<ushort>(ages);
            this.ranges = new List<AgeRange>(ranges);
        }

        //---------------------------------------------------------------------

    	/// <summary>
        /// Is a particular age included among the set of specific ages and ranges?
        /// </summary>
        public bool Contains(ushort age, out AgeRange? containingRange)
        {
            containingRange = null;
            if (ages.Contains(age))
                return true;
            foreach (AgeRange range in ranges)
            {
                if (range.Contains(age))
                {
                    containingRange = range;
                    return true;
                }
            }
            return false;
        }
    }
}
