using Landis.Cohorts;
using TypeIndependent = Landis.Cohorts.TypeIndependent;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Biomass.Succession
{
    public class LandscapeCohorts
        : Biomass.ILandscapeCohorts,
          AgeCohort.ILandscapeCohorts,
          TypeIndependent.ILandscapeCohorts
    {
        private ISiteVar<SiteCohorts> cohorts;

        //---------------------------------------------------------------------

        public LandscapeCohorts(ISiteVar<SiteCohorts> cohorts)
        {
            this.cohorts = cohorts;
        }

        //---------------------------------------------------------------------

        public ISiteCohorts this[Site site]
        {
            get {
                return cohorts[site];
            }
        }

        //---------------------------------------------------------------------

        AgeCohort.ISiteCohorts ILandscapeCohorts<AgeCohort.ISiteCohorts>.this[Site site]
        {
            get {
                return cohorts[site];
            }
        }

        //---------------------------------------------------------------------

        TypeIndependent.ISiteCohorts TypeIndependent.ILandscapeCohorts.this[Site site]
        {
            get {
                return cohorts[site];
            }
        }

        //---------------------------------------------------------------------

        TypeIndependent.CohortAttribute[] TypeIndependent.ILandscapeCohorts.CohortAttributes
        {
            get {
                return Cohort.Attributes;
            }
        }
    }
}
