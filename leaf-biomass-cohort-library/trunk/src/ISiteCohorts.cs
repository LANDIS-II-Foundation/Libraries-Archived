using Landis.Core;
//using Landis.Cohorts;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Library.LeafBiomassCohorts
{
    /// <summary>
    /// All the biomass cohorts at a site.
    /// </summary>
    public interface ISiteCohorts
        //: Landis.Library.AgeOnlyCohorts.ISiteCohorts
        : Landis.Library.Cohorts.ISiteCohorts<LeafBiomassCohorts.ISpeciesCohorts>
        //: Landis.Cohorts.ISiteCohorts<ISpeciesCohorts>
    {
        int ReduceOrKillBiomassCohorts(IDisturbance disturbance);
        void AddNewCohort(ISpecies species, ushort age, float initialWood, float initialLeaf);
        void Grow(ActiveSite site, bool isSuccessionTimestep, bool annualTimestep);

    }
}
