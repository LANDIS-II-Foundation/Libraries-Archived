using Landis.AgeCohort;

namespace Landis.Harvest
{
    /// <summary>
    /// Various methods for selecting which of a species' cohorts to harvest.
    /// </summary>
    public static class SelectCohorts
    {
        /// <summary>
        /// A method for selecting which of a species' cohorts to harvest.
        /// </summary>
        public delegate void Method(ISpeciesCohorts         cohorts,
                                    ISpeciesCohortBoolArray isHarvested);

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects all of a species' cohorts for harvesting.
        /// </summary>
        public static void All(ISpeciesCohorts         cohorts,
                               ISpeciesCohortBoolArray isHarvested)
        {
            //loop through all cohorts and mark as harvested
            for (int i = 0; i < isHarvested.Count; i++)
                isHarvested[i] = true;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects the oldest of a species' cohorts for harvesting.
        /// </summary>
        public static void Oldest(ISpeciesCohorts         cohorts,
                                  ISpeciesCohortBoolArray isHarvested)
        {
            //  Oldest is first.
            isHarvested[0] = true;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects the youngest of a species' cohorts for harvesting.
        /// </summary>
        public static void Youngest(ISpeciesCohorts         cohorts,
                                    ISpeciesCohortBoolArray isHarvested)
        {
            //  Youngest is last.
            isHarvested[isHarvested.Count - 1] = true;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects all of a species' cohorts for harvesting except the oldest.
        /// </summary>
        public static void AllExceptOldest(ISpeciesCohorts         cohorts,
                                           ISpeciesCohortBoolArray isHarvested)
        {
            //  Oldest is first (so start at i = 1 instead of i = 0)
            for (int i = 1; i < isHarvested.Count; i++)
                isHarvested[i] = true;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects all of a species' cohorts for harvesting except the
        /// youngest.
        /// </summary>
        public static void AllExceptYoungest(ISpeciesCohorts         cohorts,
                                             ISpeciesCohortBoolArray isHarvested)
        {
            //  Youngest is last.
            int youngestIndex = isHarvested.Count - 1;
            for (int i = 0; i < youngestIndex; i++)
                isHarvested[i] = true;
        }
    }
}
