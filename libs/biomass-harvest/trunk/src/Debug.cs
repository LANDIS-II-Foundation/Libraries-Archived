// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;
using log4net;

namespace Landis.Extension.LandUse
{
    /// <summary>
    /// Helper methods for logging debug information
    /// </summary>
    public static class Debug
    {
        private static ISiteVar<ISiteCohorts> cohorts;

        /// <summary>
        /// Write the list of cohorts at a site to a log.
        /// </summary>
        public static void WriteSiteCohorts(ILog       log,
                                            ActiveSite site)
        {
            if (cohorts == null)
                cohorts = Model.Core.GetSiteVar<ISiteCohorts>("Succession.BiomassCohorts");

            int count = 0;  // # of species with cohorts
            foreach (ISpeciesCohorts speciesCohorts in cohorts[site])
            {
                string cohort_list = "";
                foreach (ICohort cohort in speciesCohorts)
                {
                    cohort_list += string.Format(", {0} yrs ({1})", cohort.Age, cohort.Biomass);
                }
                log.DebugFormat("      {0}{1}", speciesCohorts.Species.Name, cohort_list);
                count += 1;
            }
            if (count == 0)
                log.DebugFormat("      (no cohorts)");
        }
    }
}
