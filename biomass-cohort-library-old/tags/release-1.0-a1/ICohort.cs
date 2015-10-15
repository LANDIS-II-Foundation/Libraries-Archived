namespace Landis.Biomass
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public interface ICohort
        : AgeCohort.ICohort
    {
        /// <summary>
        /// The cohort's biomass (units?).
        /// </summary>
        ushort Biomass
        {
            get;
        }
    }
}
