
//using Landis.Library.AgeOnlyCohorts;
using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Library.LeafBiomassCohorts
{
    /// <summary>
    /// The biomass cohorts for a particular species at a site.
    /// </summary>
    public interface ISpeciesCohorts
        : Landis.Library.Cohorts.ISpeciesCohorts<Landis.Library.LeafBiomassCohorts.ICohort>
    {
        void RemoveCohort(Cohort cohort,
                              ActiveSite site,
                              ExtensionType disturbanceType);



        Cohort Get(int index);
    }
}
