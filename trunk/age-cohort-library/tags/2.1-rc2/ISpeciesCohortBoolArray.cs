namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// An array of boolean values with one value for each cohort of a species'
    /// cohorts.
    /// </summary>
    public interface ISpeciesCohortBoolArray
    {
        /// <summary>
        /// The number of values in the array.
        /// </summary>
        /// <remarks>
        /// Same as the number of species' cohorts.
        /// </remarks>
        int Count
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The boolean value at a particular index in the array.
        /// </summary>
        bool this[int index]
        {
            get;
            set;
        }
    }
}
