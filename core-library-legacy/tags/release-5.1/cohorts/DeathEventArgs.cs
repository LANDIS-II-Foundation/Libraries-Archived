using Landis.Landscape;

namespace Landis.Cohorts
{
    /// <summary>
    /// Information about a cohort's death.
    /// </summary>
    public class DeathEventArgs<TCohort, TDisturbanceType>
        : System.EventArgs
    {
        private TCohort cohort;
        private ActiveSite site;
        private TDisturbanceType disturbanceType;

        //---------------------------------------------------------------------

        /// <summary>
        /// The cohort that died.
        /// </summary>
        public TCohort Cohort
        {
            get {
                return cohort;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The site where the cohort died.
        /// </summary>
        public ActiveSite Site
        {
            get {
                return site;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The type of disturbance that killed the cohort.
        /// </summary>
        /// <remarks>
        /// null if the cohort died during the growth phase of succession.
        /// </remarks>
        public TDisturbanceType DisturbanceType
        {
            get {
                return disturbanceType;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public DeathEventArgs(TCohort          cohort,
                              ActiveSite       site,
                              TDisturbanceType disturbanceType)
        {
            this.cohort = cohort;
            this.site = site;
            this.disturbanceType = disturbanceType;
        }
    }
}
