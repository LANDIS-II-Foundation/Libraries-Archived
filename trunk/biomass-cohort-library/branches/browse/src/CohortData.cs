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
        /// The cohort's biomass (g/m2).
        /// </summary>
        public int Biomass;

        //---------------------------------------------------------------------
        /// <summary>
        /// The cohort's annual NPP (g/m2).
        /// </summary>
        public int ANPP;
        //---------------------------------------------------------------------
        /// <summary>
        /// The cohort's potential forage (g/m2).
        /// </summary>
        public int Forage;
        //---------------------------------------------------------------------
        /// <summary>
        /// The cohort's forage within browse reach (g/m2).
        /// </summary>
        public int ForageInReach;
        //---------------------------------------------------------------------
        /// <summary>
        /// The cohort's last proportion browsed.
        /// </summary>
        public double LastBrowseProp;
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
                          int biomass)
        {
            this.Age = age;
            this.Biomass = biomass;
            this.ANPP = biomass;
            this.Forage = 0;
            this.ForageInReach = 0;
            this.LastBrowseProp = 0.0;
        }
    }
}
