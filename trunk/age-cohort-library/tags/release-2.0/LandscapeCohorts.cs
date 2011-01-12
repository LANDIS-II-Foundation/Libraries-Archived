using Landis.Cohorts;
using TypeIndependent = Landis.Cohorts.TypeIndependent;
using Landis.Landscape;

namespace Landis.AgeCohort
{
    /// <summary>
    /// The cohorts for all the sites in the landscape.
    /// </summary>
    public class LandscapeCohorts
        : ILandscapeCohorts, TypeIndependent.ILandscapeCohorts
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
