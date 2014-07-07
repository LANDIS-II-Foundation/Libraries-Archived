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
        public ExtensionType Type { get; protected set; }
        public ActiveSite CurrentSite { get; protected set; }
        protected ICohortSelector cohortSelector;

        //---------------------------------------------------------------------

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

        public virtual void Cut(ActiveSite site)
        {
            SiteVars.Cohorts[site].RemoveMarkedCohorts(this);
        }
    }
}
