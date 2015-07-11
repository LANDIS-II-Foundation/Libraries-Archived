// This file is part of the Biomass Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/biomass-harvest/trunk/

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using Landis.Library.BiomassCohorts;
using Landis.Library.SiteHarvest;
using Landis.SpatialModeling;
using log4net;

namespace Landis.Library.BiomassHarvest
{
    /// <summary>
    /// A disturbance where at least one species is partially thinned (i.e.,
    /// a percentage of one or more cohorts are harvested).
    /// </summary>
    /// <remarks>
    /// It is based on its counterpart, WholeCohortCutter, in the Site Harvest
    /// library.  The base class is used to handle selectors for species that
    /// harvest whole cohorts (i.e., no partial harvesting).  This class
    /// handles the cohort selectors for those species that are partially
    /// removed (i.e., a percentage was specified for at least one age or age
    /// range).
    /// </remarks>
    public class PartialCohortCutter
        : WholeCohortCutter, BiomassCohorts.IDisturbance
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PartialCohortCutter));
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        private PartialCohortSelectors partialCohortSelectors;
        private CohortCounts cohortCounts;

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PartialCohortCutter(Landis.Library.SiteHarvest.ICohortSelector cohortSelector,
                                   PartialCohortSelectors                     partialCohortSelectors,
                                   ExtensionType                              extensionType)
            : base(cohortSelector, extensionType)
        {
            this.partialCohortSelectors = new PartialCohortSelectors(partialCohortSelectors);
        }

        //---------------------------------------------------------------------

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
            if (reduction > 0)
                cohortCounts.IncrementCount(cohort.Species);
            Record(reduction, cohort);
            return reduction;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Cuts the cohorts at a site.
        /// </summary>
        public override void Cut(ActiveSite   site,
                                 CohortCounts cohortCounts)
        {
            if (isDebugEnabled)
            {
                log.DebugFormat("    {0} is cutting site {1}; cohorts are:",
                                GetType().Name,
                                site.Location);
                Debug.WriteSiteCohorts(log, site);
            }

            // Use age-only cohort selectors to harvest whole cohorts
            // Note: the base method sets the CurrentSite property, and resets
            // the counts to 0 before cutting.
            base.Cut(site, cohortCounts);

            // Then do any partial harvesting with the partial cohort selectors.
            this.cohortCounts = cohortCounts;
            SiteVars.Cohorts[site].ReduceOrKillBiomassCohorts(this);

            if (isDebugEnabled)
            {
                log.DebugFormat("    Cohorts after cutting site {0}:",
                                site.Location);
                Debug.WriteSiteCohorts(log, site);
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Records the amount a cohort's biomass was cut (reduced).
        /// </summary>
        protected void Record(int     reduction,
                              ICohort cohort)
        {
            SiteBiomass.RecordHarvest(cohort.Species, reduction);
            if (isDebugEnabled)
                log.DebugFormat("    {0}, age {1}, biomass {2} : reduction = {3}",
                                cohort.Species.Name,
                                cohort.Age,
                                cohort.Biomass,
                                reduction);
        }
    }
}
