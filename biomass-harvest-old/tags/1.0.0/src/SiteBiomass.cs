// This file is part of the Biomass Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/biomass-harvest/trunk/

using Landis.Core;
using Landis.Library.BiomassCohorts;
using log4net;
using System.Collections.Generic;

namespace Landis.Library.BiomassHarvest
{
    /// <summary>
    /// Records the biomass harvested for each species at the current site.
    /// </summary>
    public static class SiteBiomass
    {
        private static IDictionary<ISpecies, int> biomassHarvested;
        private static readonly ILog log = LogManager.GetLogger(typeof(SiteBiomass));
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

        /// <summary>
        /// The biomass harvested for each species at the site currently being
        /// cut by a disturbance extension.
        /// </summary>
        public static IDictionary<ISpecies, int> Harvested
        {
            get { return biomassHarvested; }
        }

        //---------------------------------------------------------------------

        static SiteBiomass()
        {
            biomassHarvested = new Dictionary<ISpecies, int>(Model.Core.Species.Count);
            ResetHarvestTotals();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Enable the recording of biomass harvested for the extension's
        /// execution during current timestep.
        /// </summary>
        public static void EnableRecordingForHarvest()
        {
            Cohort.AgeOnlyDeathEvent += CohortDied;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Disable the recording of biomass harvested at the end of the
        /// extension's execution during the current timestep.
        /// </summary>
        public static void DisableRecordingForHarvest()
        {
            Cohort.AgeOnlyDeathEvent -= CohortDied;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Event handler for when a cohort dies because of cutting (i.e., it's
        /// completely harvested).
        /// </summary>
        public static void CohortDied(object sender,
                                      DeathEventArgs eventArgs)
        {
            ICohort cohort = eventArgs.Cohort;
            if (isDebugEnabled)
                log.DebugFormat("    cohort died: {0}, age {1}, biomass {2}",
                                cohort.Species.Name,
                                cohort.Age,
                                cohort.Biomass);
            RecordHarvest(cohort.Species, cohort.Biomass);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Resets the harvest totals for all species.
        /// </summary>
        public static void ResetHarvestTotals()
        {
            foreach (ISpecies species in Model.Core.Species)
            {
                biomassHarvested[species] = 0;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Records an amount of biomass that has been cut for a species.
        /// </summary>
        public static void RecordHarvest(ISpecies species,
                                         int      biomass)
        {
            biomassHarvested[species] += biomass;
        }
    }
}
