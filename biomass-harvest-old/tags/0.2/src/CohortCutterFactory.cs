// This file is part of the Biomass Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/biomass-harvest/trunk/

using Landis.Core;
using Landis.Library.SiteHarvest;

namespace Landis.Library.BiomassHarvest
{
    /// <summary>
    /// A factory for making instances of cohort cutters (WholeCohortCutter
    /// and PartialCohortCutter).
    /// </summary>
    public static class CohortCutterFactory
    {
        /// <summary>
        /// Creates a cohort cutter instance.
        /// </summary>
        /// <returns>
        /// An instance of WholeCohortCutter if no species is partially thinned
        /// by the cohort selector.  If the selector has a percentage for at
        /// least one species, then an instance of PartialCohortCutter is
        /// returned.
        /// </returns>
        public static ICohortCutter CreateCutter(ICohortSelector cohortSelector,
                                                 ExtensionType   extensionType)
        {
            ICohortCutter cohortCutter;
            if (PartialThinning.CohortSelectors.Count == 0)
                cohortCutter = new WholeCohortCutter(cohortSelector, extensionType);
            else
                cohortCutter = new PartialCohortCutter(cohortSelector,
                                                       PartialThinning.CohortSelectors,
                                                       extensionType);
            return cohortCutter;
        }
    }
}
