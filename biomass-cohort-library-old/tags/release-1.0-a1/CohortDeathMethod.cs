using Landis.Landscape;

namespace Landis.Biomass
{
    /// <summary>
    /// A method that is called when a cohort dies.
    /// </summary>
    public delegate void CohortDeathMethod(ICohort    cohort,
                                           ActiveSite site);
}
