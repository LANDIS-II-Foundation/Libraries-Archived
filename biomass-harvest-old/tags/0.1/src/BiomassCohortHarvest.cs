// This file is part of the Biomass Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/biomass-harvest/trunk/

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using Landis.Library.BiomassCohorts;
using Landis.Library.Harvest;
using Landis.SpatialModeling;

namespace Landis.Library.BiomassHarvest
{
    /// <summary>
    /// A disturbance that harvests biomass cohorts at a site.
    /// </summary>
    /// <remarks>
    /// It is derived from its counterpart, AgeCohortHarvest, in the harvest
    /// library.  The base class is used to handle selectors for species that
    /// harvest whole cohorts (i.e., no partial harvesting).  This class
    /// handles the cohort selectors for those species that are partially
    /// removed (i.e., a percentage was specified for at least one age or age
    /// range).
    /// </remarks>
    public abstract class BiomassCohortHarvest
        : AgeCohortHarvest, IDisturbance
    {
        private PartialCohortSelectors partialCohortSelectors;

        //---------------------------------------------------------------------

        public BiomassCohortHarvest(ICohortSelector wholeCohortSelector,
                                    PartialCohortSelectors partialCohortSelectors)
            : base(wholeCohortSelector)
        {
            this.partialCohortSelectors = new PartialCohortSelectors(partialCohortSelectors);
        }

        //---------------------------------------------------------------------

        // Interface method for biomass disturbances

        int IDisturbance.ReduceOrKillMarkedCohort(ICohort cohort)
        {
            int reduction = 0;
            SpecificAgesCohortSelector specificAgeCohortSelector;
            if (partialCohortSelectors.TryGetValue(cohort.Species, out specificAgeCohortSelector))
            {
                Percentage percentage;
                if (specificAgeCohortSelector.Selects(cohort, out percentage))
                    reduction = (int)(percentage * cohort.Biomass);
            }
            Record(reduction, cohort);
            return reduction;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Record a reduction that's been computed for a particular cohort.
        /// </summary>
        /// <remarks>
        /// Derived classes will override this method in order to collect
        /// statistics about biomass (for example, the amount of biomass
        /// harvest per species at a site).
        /// </remarks>
        protected abstract void Record(int     reduction,
                                       ICohort cohort);

        //---------------------------------------------------------------------

        /// <summary>
        /// Cut the trees at a site.
        /// </summary>
        public override void Cut(ActiveSite site)
        {
            // Use age-only cohort selectors to harvest whole cohorts
            base.Cut(site);

            // Then do any partial harvesting with the partial cohort selectors.
            SiteVars.Cohorts[site].ReduceOrKillBiomassCohorts(this);
        }
    }
}
