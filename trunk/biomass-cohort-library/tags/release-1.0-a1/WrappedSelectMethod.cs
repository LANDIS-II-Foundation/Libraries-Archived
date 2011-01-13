using Landis.Cohorts;

namespace Landis.Biomass
{
    /// <summary>
    /// A select method for age-only cohorts bundled so it can be called with
    /// biomass cohorts.
    /// </summary>
    public class WrappedSelectMethod
    {
        private SelectMethod<AgeCohort.ICohort> selectMethod;

        //---------------------------------------------------------------------

        public WrappedSelectMethod(SelectMethod<AgeCohort.ICohort> selectMethod)
        {
            this.selectMethod = selectMethod;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if a biomass cohort is selected.
        /// </summary>
        public bool Select(ICohort cohort)
        {
            return selectMethod(cohort);
        }
    }
}
