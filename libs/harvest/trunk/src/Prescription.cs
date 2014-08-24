// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using Landis.Core;
using Landis.Library.Succession;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A prescription describes how stands are ranked, how sites are selected,
    /// and which cohorts are harvested.
    /// </summary>
    public class Prescription
        : ISpeciesCohortsDisturbance
    {
        private static int nextNumber = 1;
        private int number;
        private string name;
        private IStandRankingMethod rankingMethod;
        private ISiteSelector siteSelector;
        private ICohortSelector cohortSelector;
        private Planting.SpeciesList speciesToPlant;
        private ActiveSite currentSite;
        private Stand currentStand;
        private int minTimeSinceDamage;
        private bool preventEstablishment;
        
        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription's number.
        /// </summary>
        /// <remarks>
        /// Each prescription's number is unique, and is generated and assigned
        /// when the prescription is initialized.
        /// </remarks>
        public int Number
        {
            get {
                return number;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The total number of prescription.
        /// </summary>
        public static int Count
        {
            get {
                return nextNumber;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription's name.
        /// </summary>
        public string Name
        {
            get {
                return name;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription's method for ranking stands.
        /// </summary>
        public IStandRankingMethod StandRankingMethod
        {
            get {
                return rankingMethod;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription's method for selecting sites in stands.
        /// </summary>
        public ISiteSelector SiteSelectionMethod
        {
            set {
                siteSelector = value;
            }
            get {
                return siteSelector;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Sets the cohorts that will be removed by the prescription.
        /// </summary>
        /// <remarks>
        /// The purpose of this property is to allow derived classes to change
        /// the cohort selector; for example, a single repeat-harvest switching
        /// between its two cohort selectors.
        /// </remarks>
        protected ICohortSelector CohortSelector
        {
            set {
                cohortSelector = value;
            }
        }

        protected ISiteSelector SiteSelector
        {
            set
            {
                siteSelector = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Sets the optional list of species that are planted at each site
        /// harvested by the prescription.
        /// </summary>
        /// <remarks>
        /// The purpose of this property is to allow derived classes to change
        /// the species list; for example, a single repeat-harvest switching
        /// between the lists for initial harvests and repeat harvests.
        /// </remarks>
        public Planting.SpeciesList SpeciesToPlant
        {
            set {
                speciesToPlant = value;
            }
            get {
                return speciesToPlant;
            }
        }
        

        //---------------------------------------------------------------------

        ExtensionType IDisturbance.Type
        {
            get {
                return PlugIn.ExtType;
            }
        }

        //---------------------------------------------------------------------

        ActiveSite IDisturbance.CurrentSite
        {
            get {
                return currentSite;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription's time limit for harvesting damaged sites. If
        /// a site has been damaged less than this, it should not be harvested.
        /// </summary>
        public int MinTimeSinceDamage {
            get {
                return minTimeSinceDamage;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription's boolean variable for preventing establishment
        /// </summary>
        public bool PreventEstablishment {
            get {
                return preventEstablishment;
            }
        }

        //---------------------------------------------------------------------

        public Prescription(string               name,
                            IStandRankingMethod  rankingMethod,
                            ISiteSelector        siteSelector,
                            ICohortSelector      cohortSelector,
                            Planting.SpeciesList speciesToPlant,
                            int                  minTimeSinceDamage,
                            bool                 preventEstablishment)
        {
            this.number = nextNumber;
            nextNumber++;

            this.name = name;
            this.rankingMethod = rankingMethod;
            this.siteSelector = siteSelector;
            this.cohortSelector = cohortSelector;
            this.speciesToPlant = speciesToPlant;
            this.minTimeSinceDamage = minTimeSinceDamage;
            this.preventEstablishment = preventEstablishment;
            
        }

        //---------------------------------------------------------------------
        
        /// <summary>
        /// Harvests a stand (and possibly its neighbors) according to the
        /// prescription's site-selection method.
        /// </summary>
        /// <returns>
        /// The area that was harvested (units: hectares).
        /// </returns>
        // This is called by AppliedPrescription
        public virtual void Harvest(Stand stand)
        {
            //set prescription name for stand
            stand.PrescriptionName = this.Name;
            stand.HarvestedRank = PlugIn.CurrentRank;
            stand.LastPrescription = this;
            
            stand.MinTimeSinceDamage = this.minTimeSinceDamage;
            
            //set current stand
            currentStand = stand;
            currentStand.ClearDamageTable();

            // SelectSites(stand) is where either complete, complete stand spreading, or partial stand
            // spreading are activated.
            // tjs - This is what gets the sites that will be harvested
           

            foreach (ActiveSite site in siteSelector.SelectSites(stand)) {
                currentSite = site;

                SiteVars.Cohorts[site].RemoveMarkedCohorts(this);         
                
                if (SiteVars.CohortsDamaged[site] > 0)
                {
                    stand.LastAreaHarvested += PlugIn.ModelCore.CellArea;
                    SiteVars.Prescription[site] = this;
                }    

                if (speciesToPlant != null)
                    Reproduction.ScheduleForPlanting(speciesToPlant, site);
            
            } 
            return; 
        } 

        //---------------------------------------------------------------------
        void ISpeciesCohortsDisturbance.MarkCohortsForDeath(ISpeciesCohorts cohorts,
                                                         ISpeciesCohortBoolArray isDamaged)
        {
            cohortSelector.Harvest(cohorts, isDamaged);

            int cohortsDamaged = 0;
            for (int i = 0; i < isDamaged.Count; i++) {
                if (isDamaged[i]) {
                    
                    //if this cohort is killed, update the damage table (for the stand of this site) with this species name
                    SiteVars.Stand[currentSite].UpdateDamageTable(cohorts.Species.Name);
                    //PlugIn.ModelCore.UI.WriteLine("Damaged:  {0}.", cohorts.Species.Name);
                    
                    //and increment the cohortsDamaged
                    cohortsDamaged++;
                }
            }
            SiteVars.CohortsDamaged[currentSite] += cohortsDamaged;
        }
    }
}