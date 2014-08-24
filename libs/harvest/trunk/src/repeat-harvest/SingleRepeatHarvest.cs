// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Landis.Library.Succession;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A single repeat-harvest harvests stands and then sets them aside for
    /// just one additional harvest.  The additional harvest can remove a
    /// different set of cohorts than the initial harvest.
    /// </summary>
    public class SingleRepeatHarvest
        : RepeatHarvest
    {
        private ICohortSelector initialCohortSelector;
        private Planting.SpeciesList initialSpeciesToPlant;

        private ICohortSelector additionalCohortSelector;
        private Planting.SpeciesList additionalSpeciesToPlant;
        private ISiteSelector additionalSiteSelector;

        //---------------------------------------------------------------------

        public SingleRepeatHarvest(string               name,
                                   IStandRankingMethod  rankingMethod,
                                   ISiteSelector        siteSelector,
                                   ICohortSelector      cohortSelector,
                                   Planting.SpeciesList speciesToPlant,
                                   ICohortSelector      additionalCohortSelector,
                                   Planting.SpeciesList additionalSpeciesToPlant,
                                   ISiteSelector        additionalSiteSelector,
                                   int                  minTimeSinceDamage,
                                   bool                 preventEstablishment,
                                   int                  interval)
            : base(name, rankingMethod, siteSelector, cohortSelector, speciesToPlant,additionalSiteSelector, minTimeSinceDamage, preventEstablishment, interval)
        {
            this.initialCohortSelector = cohortSelector;
            this.initialSpeciesToPlant = speciesToPlant;

            this.additionalCohortSelector = additionalCohortSelector;
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
                CohortSelector = additionalCohortSelector;
                SpeciesToPlant = additionalSpeciesToPlant;
                SiteSelector = additionalSiteSelector; // new CompleteStand();
                //
                //if(this.SiteSelectionMethod.GetType() == Landis.Extension.BiomassHarvest.PartialStandSpreading)
                //  SiteSelector = BiomassHarvest.WrapSiteSelector(SiteSelector);
                
            }
            else {
                CohortSelector = initialCohortSelector;
                SpeciesToPlant = initialSpeciesToPlant;
            }
            base.Harvest(stand);

            return; 


        }
    }
}
