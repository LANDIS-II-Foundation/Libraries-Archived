// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A stand ranking method based on economic ranks
    /// </summary>
    public class EconomicRank
        : StandRankingMethod
    {
        private EconomicRankTable rankTable;

        //---------------------------------------------------------------------

        public EconomicRank(EconomicRankTable rankTable)
        {
            this.rankTable = rankTable;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        protected override double ComputeRank(Stand stand, int i)
        {
            double standEconImportance = 0.0;
            //PlugIn.ModelCore.UI.WriteLine("Base Harvest: EconomicRank.cs: ComputeRank:  there are {0} sites in this stand.", stand.SiteCount);
            foreach (ActiveSite site in stand) {

                double siteEconImportance = 0.0;
                foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                {
                    EconomicRankParameters rankingParameters = rankTable[speciesCohorts.Species];
                    foreach (ICohort cohort in speciesCohorts) {
                        if (rankingParameters.MinimumAge > 0 &&
                            rankingParameters.MinimumAge <= cohort.Age)
                            siteEconImportance += (double) rankingParameters.Rank / rankingParameters.MinimumAge * cohort.Age;
                    }
                }
                standEconImportance += siteEconImportance;
            }
            standEconImportance /= stand.SiteCount;

            return standEconImportance;
        }
    }
}
