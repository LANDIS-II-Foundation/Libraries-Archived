using System.Collections.Generic;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// An array of boolean values with one value for each cohort of a species'
    /// cohorts.
    /// </summary>
    public class SpeciesCohortBoolArray
        : List<bool>, ISpeciesCohortBoolArray
    {
        /// <summary>
        /// Initializes the array to a particular number of false values.
        /// </summary>
        public void SetAllFalse(int count)
        {
            Clear();
            for (int i = count; i > 0; i--)
                Add(false);
        }
    }
}
