// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.Library.SiteHarvest;
using Landis.Library.Succession;
using System.Collections.Generic;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A repeat harvest is a variation of a prescription that harvests stands
    /// and then sets them aside for additional harvests.
    /// </summary>
    public class RepeatHarvest
        : Prescription
    {
        private int interval;
        private StandSpreading spreadingSiteSelector;
        private List<Stand> harvestedStands;
        private ISiteSelector additionalSiteSelector;

        //---------------------------------------------------------------------

        /// <summary>
        /// The time interval between a stand's initial harvest and each of
        /// its additional harvests.
        /// </summary>
        /// <remarks>
        /// Units: years.
        /// </remarks>
        public int Interval
        {
            get {
                return interval;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The list of stands that were harvested during the most recent call
        /// to the Harvest method.
        /// </summary>
        public List<Stand> HarvestedStands
        {
            get {
                return harvestedStands;
            }
        }

        //---------------------------------------------------------------------

        public RepeatHarvest(string               name,
                             IStandRankingMethod  rankingMethod,
                             ISiteSelector        siteSelector,
                             ICohortCutter        cohortCutter,
                             Planting.SpeciesList speciesToPlant,
                             ISiteSelector        additionalSiteSelector,
                             int                  minTimeSinceDamage,
                             bool                 preventEstablishment,
                             int                  interval)
            : base(name, rankingMethod, siteSelector, cohortCutter, speciesToPlant, minTimeSinceDamage, preventEstablishment)
        {
            this.interval = interval;
            this.spreadingSiteSelector = siteSelector as StandSpreading;
            this.additionalSiteSelector = additionalSiteSelector;
            this.harvestedStands = new List<Stand>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvests a stand (and possibly its neighbors) according to the
        /// repeat-harvest's site-selection method.
        /// </summary>
        /// <returns>
        /// The area that was harvested (units: hectares).
        /// </returns>
        public override void Harvest(Stand stand)
        {
            if (stand.IsSetAside)
            {
                SiteSelector = additionalSiteSelector;
            }
            
            base.Harvest(stand);
            
            harvestedStands.Clear();

            //  Add stand to list only if actually harvested
            if(stand.LastAreaHarvested > 0)
                harvestedStands.Add(stand); 
            
            if (spreadingSiteSelector != null)
                harvestedStands.AddRange(spreadingSiteSelector.HarvestedNeighbors);
            
            return; // areaHarvested;
        }
    }
}
