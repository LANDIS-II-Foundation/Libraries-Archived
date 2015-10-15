using Landis.Landscape;

namespace Landis.Biomass
{
    /// <summary>
    /// A wrapped age-cohort disturbance so it works with biomass cohorts.
    /// </summary>
    public class WrappedDisturbance
        : IDisturbance
    {
        private AgeCohort.IDisturbance ageCohortDisturbance;

        //---------------------------------------------------------------------

        public WrappedDisturbance(AgeCohort.IDisturbance ageCohortDisturbance)
        {
            this.ageCohortDisturbance = ageCohortDisturbance;
        }

        //---------------------------------------------------------------------

        public ActiveSite CurrentSite
        {
            get {
                return ageCohortDisturbance.CurrentSite;
            }
        }

        //---------------------------------------------------------------------

        public ushort Damage(ICohort cohort)
        {
            if (ageCohortDisturbance.Damage(cohort))
                return cohort.Biomass;
            else
                return 0;
        }
    }
}
