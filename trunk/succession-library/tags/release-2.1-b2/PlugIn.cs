using Edu.Wisc.Forest.Flel.Util;

using Landis.InitialCommunities;
using Landis.Landscape;
using Landis.Species;
using Landis.RasterIO;
using Landis.Util;

using System;
using System.Collections.Generic;

using log4net;

namespace Landis.Succession
{
    /// <summary>
    /// Base component upon which a succession plug-in can built.
    /// </summary>
    public abstract class PlugIn
        : PlugIns.SuccessionPlugIn
    {
        private DisturbedSiteEnumerator disturbedSites;
        public bool ShowProgress;
        private uint? prevSiteDataIndex;

        //---------------------------------------------------------------------

        private static ILog logger;

        //---------------------------------------------------------------------

        static PlugIn()
        {
            logger = LogManager.GetLogger(typeof(PlugIn));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public PlugIn(string name)
            : base(name)
        {
            this.ShowProgress = true;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Site variable that indicates if a site has been disturbed.
        /// </summary>
        protected ISiteVar<bool> Disturbed
        {
            get {
                return SiteVars.Disturbed;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the instance and its associated site variables.
        /// </summary>
        protected void Initialize(PlugIns.ICore                   modelCore,
                                  double[,]                       establishProbabilities,
                                  SeedingAlgorithms               seedAlg,
                                  Reproduction.AddNewCohortMethod addNewCohort)
        {
            Model.Core = modelCore;
            SiteVars.Initialize();
            disturbedSites = new DisturbedSiteEnumerator(Model.Core.Landscape,
                                                         SiteVars.Disturbed);

            SeedingAlgorithm algorithm;
            switch (seedAlg) {
                case SeedingAlgorithms.NoDispersal:
                    algorithm = NoDispersal.Algorithm;
                    break;

                case SeedingAlgorithms.UniversalDispersal:
                    algorithm = UniversalDispersal.Algorithm;
                    break;

                case SeedingAlgorithms.WardSeedDispersal:
                    algorithm = WardSeedDispersal.Algorithm;
                    break;

                default:
                    throw new ArgumentException(string.Format("Unknown seeding algorithm: {0}", seedAlg));
            }
            Reproduction.Initialize(establishProbabilities, algorithm,
                                    addNewCohort == null ? null : new Reproduction.Delegates.AddNewCohort(addNewCohort));
        }

        //---------------------------------------------------------------------

        public override void InitializeSites(string initialCommunities,
                                             string initialCommunitiesMap)
        {
            UI.WriteLine("Loading initial communities from file \"{0}\" ...", initialCommunities);
            InitialCommunities.DatasetParser parser = new InitialCommunities.DatasetParser(Timestep, Model.Core.Species);
            InitialCommunities.IDataset communities = Data.Load<InitialCommunities.IDataset>(initialCommunities, parser);

            UI.WriteLine("Reading initial communities map \"{0}\" ...", initialCommunitiesMap);
            IInputRaster<InitialCommunities.Pixel> map;
            map = Model.Core.OpenRaster<InitialCommunities.Pixel>(initialCommunitiesMap);
            using (map) {
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    InitialCommunities.Pixel pixel = map.ReadPixel();
                    ActiveSite activeSite = site as ActiveSite;
                    if (activeSite == null)
                        continue;
                    ushort mapCode = pixel.Band0;
                    ICommunity community = communities.Find(mapCode);
                    if (community == null) {
                        throw new PixelException(activeSite.Location,
                                                 "Unknown map code for initial community: {0}",
                                                 mapCode);
                    }
                    InitializeSite(activeSite, community);
                }
            }

            base.Cohorts = Model.Core.SuccessionCohorts;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes an active site's cohorts using a specific initial
        /// community.
        /// </summary>
        protected abstract void InitializeSite(ActiveSite site,
                                               ICommunity initialCommunity);

        //---------------------------------------------------------------------

        public override void Run()
        {
            bool isSuccessionTimestep = (Model.Core.CurrentTime % Timestep == 0);
            IEnumerable<MutableActiveSite> sites;
            if (isSuccessionTimestep)
                sites = Model.Core.Landscape.ActiveSites;
            else
                sites = disturbedSites;

            AgeCohorts(sites, isSuccessionTimestep);
            ComputeShade(sites);
            ReproduceCohorts(sites);

            if (! isSuccessionTimestep)
                SiteVars.Disturbed.ActiveSiteValues = false;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Advances the age of all the cohorts at certain specified sites.
        /// </summary>
        /// <param name="sites">
        /// The sites whose cohorts are to be aged.
        /// </param>
        public void AgeCohorts(IEnumerable<MutableActiveSite> sites,
                               bool                           isSuccessionTimestep)
        {
            int? succTimestep = null;
            if (isSuccessionTimestep) {
                succTimestep = Timestep;
                ShowProgress = true;
            }
            else {
                ShowProgress = false;
            }

            ProgressBar progressBar = null;
            if (ShowProgress) {
                System.Console.WriteLine("Ageing cohorts ...");
                progressBar = NewProgressBar();
            }

            foreach (ActiveSite site in sites) {
                ushort deltaTime = (ushort) (Model.Core.CurrentTime - SiteVars.TimeOfLast[site]);
                AgeCohorts(site, deltaTime, succTimestep);
                SiteVars.TimeOfLast[site] = Model.Core.CurrentTime;

                if (ShowProgress)
                    Update(progressBar, site.DataIndex);
            }
            if (ShowProgress)
                CleanUp(progressBar);
        }

        //---------------------------------------------------------------------

        private ProgressBar NewProgressBar()
        {
            prevSiteDataIndex = null;
            return new ProgressBar((uint) Model.Core.Landscape.ActiveSiteCount,
                                   System.Console.Out);
        }

        //---------------------------------------------------------------------

        private void Update(ProgressBar progressBar,
                            uint        currentSiteDataIndex)
        {
            uint increment = (uint) (prevSiteDataIndex.HasValue
                                        ? (currentSiteDataIndex - prevSiteDataIndex.Value)
                                        : currentSiteDataIndex);
            progressBar.IncrementWorkDone(increment);
            prevSiteDataIndex = currentSiteDataIndex;
        }


        //---------------------------------------------------------------------

        private void CleanUp(ProgressBar progressBar)
        {
            if (! prevSiteDataIndex.HasValue) {
                //    Then no sites were processed; the site iterator was a
                //    disturbed-sites iterator, and there were no disturbed
                //    sites.  So increment the progress bar to 100%.
                progressBar.IncrementWorkDone((uint) Model.Core.Landscape.ActiveSiteCount);
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Advances the age of all the cohorts at a site.
        /// </summary>
        /// <param name="site">
        /// The site whose cohorts are to be aged.
        /// </param>
        /// <param name="years">
        /// The number of years to advance the cohorts' ages by.
        /// </param>
        /// <param name="successionTimestep">
        /// The succession timestep (years).  If this parameter has a value,
        /// then the ageing is part of a succession timestep; therefore, all
        /// young cohorts (whose age is less than succession timestep) are
        /// combined into a single cohort whose age is the succession timestep.
        /// </param>
        protected abstract void AgeCohorts(ActiveSite site,
                                           ushort     years,
                                           int?       successionTimestep);

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the shade at certain specified sites.
        /// </summary>
        /// <param name="sites">
        /// The sites where shade is to be computed.
        /// </param>
        public void ComputeShade(IEnumerable<MutableActiveSite> sites)
        {
            ProgressBar progressBar = null;
            if (ShowProgress) {
                System.Console.WriteLine("Computing shade ...");
                progressBar = NewProgressBar();
            }

            foreach (ActiveSite site in sites) {
                SiteVars.Shade[site] = ComputeShade(site);

                if (ShowProgress)
                    Update(progressBar, site.DataIndex);
            }
            if (ShowProgress)
                CleanUp(progressBar);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the shade at a site.
        /// </summary>
        public abstract byte ComputeShade(ActiveSite site);

        //---------------------------------------------------------------------

        /// <summary>
        /// Does cohort reproduction at certain specified sites.
        /// </summary>
        /// <param name="sites">
        /// The sites where cohort reproduction should be done.
        /// </param>
        /// <remarks>
        /// Because this is the last stage of succession during a timestep,
        /// the NextTimeToRun is updated after all the sites are processed.
        /// </remarks>
        public void ReproduceCohorts(IEnumerable<MutableActiveSite> sites)
        {
            logger.Debug(string.Format("{0:G}", DateTime.Now));
            int maxGeneration = GC.MaxGeneration;
            for (int generation = 0; generation <= maxGeneration; generation++)
                logger.Debug(string.Format("  gen {0}: {1}", generation,
                                           GC.CollectionCount(generation)));

            ProgressBar progressBar = null;
            if (ShowProgress) {
                System.Console.WriteLine("Cohort reproduction ...");
                progressBar = NewProgressBar();
            }

            foreach (ActiveSite site in sites) {
                Reproduction.Do(site);

#if WATCH_CHANGING_GC
                bool collectionCountChanged = false;
                for (int generation = 0; generation <= maxGeneration; generation++) {
                    int currentCount = GC.CollectionCount(generation);
                    if (collectionCounts[generation] != currentCount) {
                        collectionCountChanged = true;
                        collectionCounts[generation] = currentCount;
                    }
                }
                if (collectionCountChanged) {
                    logger.Info(string.Format("Site {0}, index = {1}", site.Location, site.DataIndex));
                    for (int generation = 0; generation <= maxGeneration; generation++)
                        logger.Info(string.Format("  gen {0}: {1}", generation, collectionCounts[generation]));
                }
#endif
                if (ShowProgress)
                    Update(progressBar, site.DataIndex);
            }
            if (ShowProgress)
                CleanUp(progressBar);

            logger.Debug(string.Format("{0:G}", DateTime.Now));
            for (int generation = 0; generation <= maxGeneration; generation++)
                logger.Debug(string.Format("  gen {0}: {1}", generation,
                                          GC.CollectionCount(generation)));
        }
    }
}
