// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.SpatialModeling;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A stand ranking method based on economic ranks
    /// </summary>
    public class FireRiskRank
        : StandRankingMethod
    {
        private FireRiskTable rankTable;

        //---------------------------------------------------------------------

        public FireRiskRank(FireRiskTable rankTable)
        {
            this.rankTable = rankTable;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        protected override double ComputeRank(Stand stand, int i)
        {

            SiteVars.ReInitialize();
            //if (SiteVars.CFSFuelType == null)
            //    throw new System.ApplicationException("Error: CFS Fuel Type NOT Initialized.  Fuel extension MUST be active.");

            double standFireRisk = 0.0;
            //Model.Core.UI.WriteLine("Base Harvest: EconomicRank.cs: ComputeRank:  there are {0} sites in this stand.", stand.SiteCount);
            foreach (ActiveSite site in stand) {

                //double siteFireRisk = 0.0;
                int fuelType = SiteVars.CFSFuelType[site];
                //Model.Core.UI.WriteLine("Base Harvest: ComputeRank:  FuelType = {0}.", fuelType);
                FireRiskParameters rankingParameters = rankTable[fuelType];
                standFireRisk = (double)rankingParameters.Rank;

                //foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                //{
                //    FireRiskParameters rankingParameters = rankTable[speciesCohorts.Species];
                //    foreach (ICohort cohort in speciesCohorts) {
                //        if (rankingParameters.MinimumAge > 0 &&
                //            rankingParameters.MinimumAge <= cohort.Age)
                //            siteEconImportance += (double) rankingParameters.Rank / rankingParameters.MinimumAge * cohort.Age;
                //    }
                //}
                //standEconImportance += siteEconImportance;
            }
            standFireRisk /= stand.SiteCount;

            return standFireRisk;
        }
    }
}
