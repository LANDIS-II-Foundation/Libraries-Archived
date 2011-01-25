
using System;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// Utility methods for age cohorts.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Gets the maximum age among a species' cohorts.
        /// </summary>
        /// <returns>
        /// The age of the oldest cohort or 0 if there are no cohorts.
        /// </returns>
        public static ushort GetMaxAge(ISpeciesCohorts cohorts)
        {
            if (cohorts == null)
                return 0;
            ushort max = 0;
            foreach (ICohort cohort in cohorts) {
                //  First cohort is the oldest
                max = cohort.Age;
                break;
            }
            return max;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the maximum age among all cohorts at a site.
        /// </summary>
        /// <returns>
        /// The age of the oldest cohort or 0 if there are no cohorts.
        /// </returns>
        //public static ushort GetMaxAge(ISiteCohorts<ISpeciesCohorts<ICohort>> cohorts)
        public static ushort GetMaxAge(ISiteCohorts cohorts)
        {
            if (cohorts == null)
                return 0;
            ushort max = 0;
            foreach (ISpeciesCohorts speciesCohorts in cohorts)
            {
                ushort maxSpeciesAge = GetMaxAge(speciesCohorts);
                if (maxSpeciesAge > max)
                    max = maxSpeciesAge;
            }
            return max;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Compares the ages of two cohorts to determine which is older.
        /// </summary>
        /// <returns>
        /// <list type="">
        ///   <item>
        ///     A negative value if x is older than y.
        ///   </item>
        ///   <item>
        ///     0 if x and y are the same age.
        ///   </item>
        ///   <item>
        ///     A positive value if x is younger than y.
        ///   </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This method matches the signature for the System.Comparison
        /// delegate so it can be used to sort an arrary or list from oldest
        /// to youngest.  Sort methods require that the delegate return a
        /// negative value if x comes before y in the sort order, 0 if they are
        /// equivalent, and a positive value is x comes after y in the sort
        /// order.
        /// </remarks>
        public static int WhichIsOlderCohort(ICohort x,
                                             ICohort y)
        {
            return WhichIsOlder(x.Age, y.Age);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Compares two cohort ages to determine which is older.
        /// </summary>
        /// <returns>
        /// <list type="">
        ///   <item>
        ///     A negative value if x is older than y.
        ///   </item>
        ///   <item>
        ///     0 if x and y are the same age.
        ///   </item>
        ///   <item>
        ///     A positive value if x is younger than y.
        ///   </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This method matches the signature for the System.Comparison
        /// delegate so it can be used to sort an arrary or list of ages from
        /// oldest to youngest.  Sort methods require that the delegate return
        /// a negative value if x comes before y in the sort order, 0 if they
        /// are equivalent, and a positive value is x comes after y in the sort
        /// order.
        /// </remarks>
        public static int WhichIsOlder(ushort x,
                                       ushort y)
        {
            return y - x;
        }
    }
}
