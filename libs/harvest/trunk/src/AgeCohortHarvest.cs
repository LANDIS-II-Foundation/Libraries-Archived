// This file is part of the Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest/trunk/

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Library.Harvest
{
    /// <summary>
    /// A disturbance that harvests age-only cohorts at a site.
    /// </summary>
    public class AgeCohortHarvest
        : ISpeciesCohortsDisturbance
    {
        #region ISpeciesCohortDisturbance members

        /// <summary>
        /// What type of disturbance is this.
        /// </summary>
        public ExtensionType Type { get; protected set; }

        /// <summary>
        /// The site currently that the disturbance is impacting.
        /// </summary>
        public ActiveSite CurrentSite { get; protected set; }
        #endregion

        /// <summary>
        /// The object responsible for selecting cohorts for harvest at a site.
        /// </summary>
        protected ICohortSelector cohortSelector;

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AgeCohortHarvest(ICohortSelector cohortSelector)
        {
            this.cohortSelector = cohortSelector;
        }

        //---------------------------------------------------------------------

        void ISpeciesCohortsDisturbance.MarkCohortsForDeath(ISpeciesCohorts cohorts,
                                                            ISpeciesCohortBoolArray isKilled)
        {
            cohortSelector.Harvest(cohorts, isKilled);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Cut the selected trees at a site.
        /// </summary>
        public virtual void Cut(ActiveSite site)
        {
            SiteVars.Cohorts[site].RemoveMarkedCohorts(this);
        }
    }
}
