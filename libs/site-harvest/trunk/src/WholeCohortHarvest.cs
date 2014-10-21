// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.Library.Harvest;
using Landis.SpatialModeling;
using log4net;

namespace Landis.Extension.LandUse.LandCover
{
    /// <summary>
    /// A harvest where each selected cohort is completely cut.
    /// </summary>
    class WholeCohortHarvest
        : AgeCohortHarvest, ICohortHarvest
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WholeCohortHarvest));
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

        public WholeCohortHarvest(ICohortSelector cohortSelector)
            : base(cohortSelector)
        {
            base.Type = Main.ExtType;
        }

        //---------------------------------------------------------------------

        void ICohortHarvest.Cut(ActiveSite site)
        {
            if (isDebugEnabled)
                log.DebugFormat("    {0} is cutting site {1}:",
                                GetType().Name,
                                site.Location);
            CurrentSite = site;
            base.Cut(site);
        }
    }
}
