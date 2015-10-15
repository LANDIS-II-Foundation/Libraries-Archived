using Landis.Landscape;

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

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes how much of the cohort's biomass is non-woody.
        /// </summary>
        /// <param name="site">
        /// The site where the cohort is located.
        /// </param>
        ushort ComputeNonWoodyBiomass(ActiveSite site);
        
    }
}
