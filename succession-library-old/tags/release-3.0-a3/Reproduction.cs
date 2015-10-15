using Landis.Cohorts;
using Landis.Ecoregions;
using Landis.Landscape;
using Landis.Species;
using Landis.Util;

using System;
using System.Collections;
using System.Reflection;

using log4net;

namespace Landis.Succession
{
    public static class Reproduction
    {
        /// <summary>
        /// A method to add new young cohort for a particular species at a
        /// site.
        /// </summary>
        public delegate void AddNewCohortMethod(ISpecies   species,
                                                ActiveSite site);

        //---------------------------------------------------------------------

        private static double[,] establishProbabilities;
        private static Seeding seeding;
        private static AddNewCohortMethod addNewCohort;
        private static AgeCohort.ILandscapeCohorts cohorts;
        private static Species.IDataset speciesDataset;
        private static ISiteVar<BitArray> resprout;
        private static ISiteVar<BitArray> serotiny;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

        public static AddNewCohortMethod AddNewCohort
        {
            get {
                return addNewCohort;
            }
        }

        //---------------------------------------------------------------------

        public static void Initialize(double[,]          establishProbabilities,
                                      SeedingAlgorithm   seedingAlgorithm,
                                      AddNewCohortMethod addNewCohort)
        {
            Reproduction.establishProbabilities = establishProbabilities;
            seeding = new Seeding(seedingAlgorithm);
            Reproduction.addNewCohort = addNewCohort;

            cohorts = Model.Core.SuccessionCohorts as AgeCohort.ILandscapeCohorts;
            if (cohorts == null)
                throw new ApplicationException("The succession plug-in's cohorts don't support age-cohort interface");

            speciesDataset = Model.Core.Species;
            int speciesCount = speciesDataset.Count;

            resprout = Model.Core.Landscape.NewSiteVar<BitArray>();
            serotiny = Model.Core.Landscape.NewSiteVar<BitArray>();
            foreach (ActiveSite site in Model.Core.Landscape.ActiveSites) {
                resprout[site] = new BitArray(speciesCount);
                serotiny[site] = new BitArray(speciesCount);
            }
        }

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
        public static void CheckForResprouting(AgeCohort.ICohort cohort,
                                               ActiveSite        site)
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
        public static void CheckForPostFireRegen(AgeCohort.ICohort cohort,
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
        /// Does the appropriate forms of reproduction at a site.
        /// </summary>
        public static void Do(ActiveSite site)
        {
            bool serotinyOccurred = false;
            for (int index = 0; index < speciesDataset.Count; ++index) {
                if (serotiny[site].Get(index)) {
                    ISpecies species = speciesDataset[index];
                    if (SufficientLight(species, site) && Establish(species, site)) {
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
            serotiny[site].SetAll(false);

            bool speciesResprouted = false;
            if (! serotinyOccurred) {
                for (int index = 0; index < speciesDataset.Count; ++index) {
                    if (resprout[site].Get(index)) {
                        ISpecies species = speciesDataset[index];
                        if (SufficientLight(species, site) &&
                                (Util.Random.GenerateUniform() < species.VegReprodProb)) {
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

            if (! serotinyOccurred && ! speciesResprouted)
                seeding.Do(site);
        }

        //---------------------------------------------------------------------

        private static bool sufficientLight;

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if there is sufficient light at a site for a species to
        /// germinate/resprout.
        /// </summary>
        public static bool SufficientLight(ISpecies   species,
                                           ActiveSite site)
        {
            byte siteShade = SiteVars.Shade[site];
            double[] lightProbabilities = new double[6];
            if (species.ShadeTolerance == 1) lightProbabilities = new double[6]{1.0, 0.8, 0.2, 0.05, 0.01, 0.0};
            if (species.ShadeTolerance == 2) lightProbabilities = new double[6]{0.8, 0.7, 0.5, 0.05, 0.01, 0.0};
            if (species.ShadeTolerance == 3) lightProbabilities = new double[6]{0.3, 0.3, 0.5, 0.6, 0.3, 0.16};
            if (species.ShadeTolerance == 4) lightProbabilities = new double[6]{0.1, 0.2, 0.3, 0.4, 0.76, 0.4};
            if (species.ShadeTolerance == 5) lightProbabilities = new double[6]{0.0, 0.0, 0.16, 0.2, 0.8, 1.0};
            sufficientLight = Util.Random.GenerateUniform() < lightProbabilities[siteShade];
            return sufficientLight;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if a species can establish on a site.
        /// </summary>
        public static bool Establish(ISpecies   species,
                                     ActiveSite site)
        {
            double establishProbability = establishProbabilities[Model.Core.Ecoregion[site].Index, species.Index];
            return Util.Random.GenerateUniform() < establishProbability;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if a species has at least one mature cohort at a site.
        /// </summary>
        public static bool MaturePresent(ISpecies   species,
                                         Site       site)
        {
            return cohorts[site].IsMaturePresent(species);
        }
    }
}
