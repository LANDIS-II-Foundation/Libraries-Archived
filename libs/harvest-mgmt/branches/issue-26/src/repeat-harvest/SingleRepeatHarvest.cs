// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.Library.SiteHarvest;
using Landis.Library.Succession;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A single repeat-harvest harvests stands and then sets them aside for
    /// just one additional harvest.  The additional harvest can remove a
    /// different set of cohorts than the initial harvest.
    /// </summary>
    public class SingleRepeatHarvest
        : RepeatHarvest
    {
        private ICohortCutter initialCohortSelector;
        private Planting.SpeciesList initialSpeciesToPlant;

        private ICohortCutter additionalCohortCutter;
        private Planting.SpeciesList additionalSpeciesToPlant;
        private ISiteSelector additionalSiteSelector;

        //---------------------------------------------------------------------

        public SingleRepeatHarvest(string               name,
                                   IStandRankingMethod  rankingMethod,
                                   ISiteSelector        siteSelector,
                                   ICohortCutter        cohortCutter,
                                   Planting.SpeciesList speciesToPlant,
                                   ICohortCutter        additionalCohortCutter,
                                   Planting.SpeciesList additionalSpeciesToPlant,
                                   ISiteSelector        additionalSiteSelector,
                                   int                  minTimeSinceDamage,
                                   bool                 preventEstablishment,
                                   int                  interval)
            : base(name, rankingMethod, siteSelector, cohortCutter, speciesToPlant,
                   additionalSiteSelector, minTimeSinceDamage, preventEstablishment,
                   interval)
        {
            this.initialCohortSelector = cohortCutter;
            this.initialSpeciesToPlant = speciesToPlant;

            this.additionalCohortCutter = additionalCohortCutter;
            this.additionalSpeciesToPlant = additionalSpeciesToPlant;
            this.additionalSiteSelector = additionalSiteSelector;
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
            if (stand.IsSetAside) {
                CohortCutter = additionalCohortCutter;
                SpeciesToPlant = additionalSpeciesToPlant;
                SiteSelector = additionalSiteSelector; // new CompleteStand();
                //
                //if(this.SiteSelectionMethod.GetType() == Landis.Extension.BiomassHarvest.PartialStandSpreading)
                //  SiteSelector = BiomassHarvest.WrapSiteSelector(SiteSelector);
                
            }
            else {
                CohortCutter = initialCohortSelector;
                SpeciesToPlant = initialSpeciesToPlant;
            }
            base.Harvest(stand);

            return; 
        }
    }
}
