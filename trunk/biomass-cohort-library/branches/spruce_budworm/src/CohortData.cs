namespace Landis.Library.BiomassCohorts
{
    /// <summary>
    /// Data for an individual cohort that is not shared with other cohorts.
    /// </summary>
    public struct CohortData
    {
        /// <summary>
        /// The cohort's age (years).
        /// </summary>
        public ushort Age;

        //---------------------------------------------------------------------

        /// <summary>
        /// The cohort's biomass (units?).
        /// </summary>
        public int Biomass;

        //---------------------------------------------------------------------

        /// The cohort's defoliaton history (% defol for last 10 years).
        /// </summary>
        public double[] DefoliationHistory;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="age">
        /// The cohort's age.
        /// </param>
        /// <param name="biomass">
        /// The cohort's biomass.
        /// </param>
        public CohortData(ushort age,
                          int biomass,
            double [] defoliationHistory)
        {
            this.Age = age;
            this.Biomass = biomass;
            this.DefoliationHistory = defoliationHistory;
        }
    }
}
