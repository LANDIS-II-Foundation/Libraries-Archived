using Edu.Wisc.Forest.Flel.Util;
//using Landis.Cohorts;
//using Landis.Ecoregions;
//using Landis.Species;
//using Landis.Util;
using Landis.Core;
using log4net;
using System;
using System.Collections;
using System.Reflection;
using Wisc.Flel.GeospatialModeling.Landscapes;
using Landis.Library.BaseCohorts;

namespace Landis.Library.Succession
{
    public static class Reproduction
    {
        /// <summary>
        /// Various delegates associated with reproduction.
        /// </summary>
        public static class Delegates
        {
            /// <summary>
            /// A method to add new young cohort for a particular species at a
            /// site.
            /// </summary>
            public delegate void AddNewCohort(ISpecies   species,
                                              ActiveSite site);

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// A method to determine if there is sufficient light at a site for
            /// a species to reproduce.
            /// </summary>
            public delegate bool SufficientResources(ISpecies   species,
                                                     ActiveSite site);


            public delegate bool Establish(ISpecies species, ActiveSite site);
        }

        //---------------------------------------------------------------------

        //public static double[,] establishProbabilities;
        private static Seeding seeding;

        //private static Species.IDataset speciesDataset;
        private static ISpeciesDataset speciesDataset;
        private static ISiteVar<BitArray> resprout;
        private static ISiteVar<BitArray> serotiny;
        private static ISiteVar<bool> noEstablish;
        private static IPlanting planting;

        private static Delegates.AddNewCohort addNewCohort;
        private static Delegates.SufficientResources lightMethod = ReproductionDefaults.SufficientResources;
        private static Delegates.Establish estbMethod = ReproductionDefaults.Establish;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

        /// <summary>
        /// The method that this class uses to add new young cohort for a
        /// particular species at a site.
        /// </summary>
        public static Delegates.AddNewCohort AddNewCohort
        {
            get {
                return addNewCohort;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The method that this class uses to determine if there is sufficient
        /// light at a site for a species to reproduce.
        /// </summary>
        public static Delegates.SufficientResources SufficientResources
        {
            get {
                return lightMethod;
            }

            set {
                Require.ArgumentNotNull(value);
                lightMethod = value;
            }
        }

        //---------------------------------------------------------------------

        public static Delegates.Establish Establish
        {
            get
            {
                return estbMethod;
            }

            set
            {
                Require.ArgumentNotNull(value);
                estbMethod = value;
            }
        }

        //---------------------------------------------------------------------

        //public static void Initialize(double[,]              establishProbabilities,
        public static void Initialize(SeedingAlgorithm       seedingAlgorithm,
                                      Delegates.AddNewCohort addNewCohort)
        {
            // Reproduction.establishProbabilities = establishProbabilities;

            seeding = new Seeding(seedingAlgorithm);
            Reproduction.addNewCohort = addNewCohort;

            SiteVars.Cohorts = Model.Core.GetSiteVar<SiteCohorts>("Succession.Cohorts");

            speciesDataset = Model.Core.Species;
            int speciesCount = speciesDataset.Count;

            resprout = Model.Core.Landscape.NewSiteVar<BitArray>();
            serotiny = Model.Core.Landscape.NewSiteVar<BitArray>();
            noEstablish = Model.Core.Landscape.NewSiteVar<bool>();
            foreach (ActiveSite site in Model.Core.Landscape.ActiveSites) {
                resprout[site] = new BitArray(speciesCount);
                serotiny[site] = new BitArray(speciesCount);
            }

            noEstablish.ActiveSiteValues = false;
            planting = new Planting();
        }


        //---------------------------------------------------------------------

        /// <summary>
        /// Changes the table of establishment probabilities because of a
        /// change in climate.
        /// </summary>

        /*
        public static void ChangeEstablishProbabilities(double[,] establishProbabilities)
        {
            Reproduction.establishProbabilities = establishProbabilities;
        }
        */

        //---------------------------------------------------------------------

        /// <summary>
        /// Records a species that should be checked for resprouting during
        /// reproduction.
        /// </summary>
        /// <param name="cohort">
        /// The cohort whose death triggered the current call to this method.
        /// </param>
        /// <param name="site">
        /// The site where the cohort died.
        /// </param>
        /// <remarks>
        /// If the cohort's age is within the species' age range for
        /// resprouting, then the species will be have additional resprouting
        /// criteria (light, probability) checked during reproduction.
        /// </remarks>
        public static void CheckForResprouting(ICohort cohort, ActiveSite site)
        {
            ISpecies species = cohort.Species;
            if (species.MinSproutAge <= cohort.Age && cohort.Age <= species.MaxSproutAge)
                resprout[site].Set(species.Index, true);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Records a species that should be checked for post-fire regeneration
        /// during reproduction.
        /// </summary>
        /// <param name="cohort">
        /// The cohort whose death triggered the current call to this method.
        /// </param>
        /// <param name="site">
        /// The site where the cohort died.
        /// </param>
        /// <remarks>
        /// If the cohort's species has resprouting as its post-fire
        /// regeneration, then the CheckForResprouting method is called with
        /// the cohort.  If the species has serotiny as its post-fire
        /// regeneration, then the species will be checked for on-site seeding
        /// during reproduction.
        /// </remarks>
        public static void CheckForPostFireRegen(ICohort cohort,
                                                 ActiveSite        site)
        {
            ISpecies species = cohort.Species;
            switch (species.PostFireRegeneration) {
                case PostFireRegeneration.Resprout:
                    CheckForResprouting(cohort, site);
                    break;

                case PostFireRegeneration.Serotiny:
                    if (cohort.Age >= species.Maturity)
                        serotiny[site].Set(species.Index, true);
                    break;

                default:
                    //  Do nothing
                    break;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Schedules a list of species to be planted at a site.
        /// </summary>
        public static void ScheduleForPlanting(Planting.SpeciesList speciesToPlant,
                                               ActiveSite           site)
        {
            planting.Schedule(speciesToPlant, site);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Schedules a list of species to be planted at a site.
        /// </summary>
        public static void PreventEstablishment(ActiveSite site)
        {
            noEstablish[site] = true;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Does the appropriate forms of reproduction at a site.
        /// </summary>
        public static void Reproduce(ActiveSite site)
        {
            if(noEstablish[site])
                return;

            bool plantingOccurred = planting.TryAt(site);

            bool sufficientLight;

            bool serotinyOccurred = false;
            if (! plantingOccurred) {
                for (int index = 0; index < speciesDataset.Count; ++index) {
                    if (serotiny[site].Get(index)) {
                        ISpecies species = speciesDataset[index];
                        sufficientLight = SufficientResources(species, site);
                        if (sufficientLight && Establish(species, site)) {
                            AddNewCohort(species, site);
                            serotinyOccurred = true;
                            if (isDebugEnabled)
                                log.DebugFormat("site {0}: {1} post-fire regenerated",
                                                site.Location, species.Name);
                        }
                        else {
                            if (isDebugEnabled)
                                log.DebugFormat("site {0}: {1} post-fire regen failed: {2}",
                                                site.Location, species.Name,
                                                ! sufficientLight ? "insufficient light"
                                                                  : "didn't establish");
                        }
                    }
                }
            }
            serotiny[site].SetAll(false);

            bool speciesResprouted = false;
            if (! serotinyOccurred) {
                for (int index = 0; index < speciesDataset.Count; ++index) {
                    if (resprout[site].Get(index)) {
                        ISpecies species = speciesDataset[index];
                        sufficientLight = SufficientResources(species, site);
                        if (sufficientLight &&
                                (Model.Core.GenerateUniform() < species.VegReprodProb)) {
                            AddNewCohort(species, site);
                            speciesResprouted = true;
                            if (isDebugEnabled)
                                log.DebugFormat("site {0}: {1} resprouted",
                                                site.Location, species.Name);
                        }
                        else {
                            if (isDebugEnabled)
                                log.DebugFormat("site {0}: {1} resprouting failed: {2}",
                                                site.Location, species.Name,
                                                ! sufficientLight ? "insufficient light"
                                                                  : "random # >= probability");
                        }
                    }
                }
            }
            resprout[site].SetAll(false);

            if (! plantingOccurred && ! serotinyOccurred && ! speciesResprouted)
                seeding.Do(site);
        }

        //---------------------------------------------------------------------
        /*
        /// <summary>
        /// Gets the establishment probablity for a particular species at a
        /// site.
        /// </summary>
        public static double GetEstablishProbability(ISpecies   species,
                                                     ActiveSite site)
        {
            double duble = 0;
            return duble;
            //return establishProbabilities[Model.Core.Ecoregion[site].Index, species.Index];
        }
       */

        //---------------------------------------------------------------------
    }
}
