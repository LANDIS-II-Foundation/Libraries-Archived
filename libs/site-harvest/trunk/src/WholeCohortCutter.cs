// This file is part of the Site Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/site-harvest/trunk/

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using log4net;

namespace Landis.Library.SiteHarvest
{
    /// <summary>
    /// A disturbance where each selected cohort is completely cut.
    /// </summary>
    public class WholeCohortCutter
        : ISpeciesCohortsDisturbance, ICohortCutter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WholeCohortCutter));
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

        #region IDisturbance members

        /// <summary>
        /// What type of disturbance is this.
        /// </summary>
        public ExtensionType Type { get; protected set; }

        /// <summary>
        /// The site currently that the disturbance is impacting.
        /// </summary>
        public ActiveSite CurrentSite { get; protected set; }

        #endregion

        //---------------------------------------------------------------------

        #region ISpeciesCohortsDisturbance members

        /// <summary>
        /// Mark which cohorts for a species are to be cut (harvested).
        /// </summary>
        public void MarkCohortsForDeath(ISpeciesCohorts cohorts,
                                        ISpeciesCohortBoolArray isKilled)
        {
            CohortSelector.Harvest(cohorts, isKilled);
        }

        #endregion

        //---------------------------------------------------------------------

        #region ICohortCutter members

        /// <summary>
        /// The object responsible for selecting which cohorts to be cut.
        /// </summary>
        public ICohortSelector CohortSelector { get; protected set; }

        /// <summary>
        /// Cut cohorts at an individual site.
        /// </summary>
        public virtual void Cut(ActiveSite site)
        {
            if (isDebugEnabled)
                log.DebugFormat("    {0} is cutting site {1}:",
                                GetType().Name,
                                site.Location);
            CurrentSite = site;
            SiteVars.Cohorts[site].RemoveMarkedCohorts(this);
        }

        #endregion

        //---------------------------------------------------------------------

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public WholeCohortCutter(ICohortSelector cohortSelector,
                                 ExtensionType   extensionType)
        {
            Type = extensionType;
            CohortSelector = cohortSelector;
        }
    }
}
