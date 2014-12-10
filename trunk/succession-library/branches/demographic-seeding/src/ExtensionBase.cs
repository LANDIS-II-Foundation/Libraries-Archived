using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.InitialCommunities;
using Landis.Core;

using log4net;
using System;
using System.Collections.Generic;
using Landis.SpatialModeling;

namespace Landis.Library.Succession
{
    /// <summary>
    /// Base component upon which a succession plug-in can built.
    /// </summary>
    public abstract class ExtensionBase
        : SuccessionMain
    {
        private DisturbedSiteEnumerator disturbedSites;
        public bool ShowProgress;
        private uint? prevSiteDataIndex;

        //---------------------------------------------------------------------

        private static ILog logger;

        //---------------------------------------------------------------------

        static ExtensionBase()
        {
            logger = LogManager.GetLogger(typeof(ExtensionBase));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ExtensionBase(string name)
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
            get
            {
                return SiteVars.Disturbed;
            }
        }

        //---------------------------------------------------------------------


        /// <summary>
        /// Initializes the instance and its associated site variables.
        /// </summary>
        protected void Initialize(ICore modelCore,
                                  SeedingAlgorithms seedAlg)//,
                                  //Reproduction.Delegates.AddNewCohort addNewCohort)
        {
            Model.Core = modelCore;
            SiteVars.Initialize();
            Seeding.InitializeMaxSeedNeighborhood();

            disturbedSites = new DisturbedSiteEnumerator(Model.Core.Landscape,
                                                         SiteVars.Disturbed);

            SeedingAlgorithm algorithm = SeedingAlgorithmsUtil.GetAlgorithm(seedAlg);
            Reproduction.Initialize(algorithm);
        }

        //---------------------------------------------------------------------

        public override void InitializeSites(string initialCommunities,
                                             string initialCommunitiesMap,
                                             ICore modelCore)
        {
            Model.Core.UI.WriteLine("   Loading initial communities from file \"{0}\" ...", initialCommunities);
            InitialCommunities.DatasetParser parser = new InitialCommunities.DatasetParser(Timestep, Model.Core.Species);
            InitialCommunities.IDataset communities = Landis.Data.Load<InitialCommunities.IDataset>(initialCommunities, parser);

            Model.Core.UI.WriteLine("   Reading initial communities map \"{0}\" ...", initialCommunitiesMap);
            IInputRaster<uintPixel> map;
            map = Model.Core.OpenRaster<uintPixel>(initialCommunitiesMap);
            using (map)
            {
                uintPixel pixel = map.BufferPixel;
                foreach (Site site in Model.Core.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    uint mapCode = pixel.MapCode.Value;
                    if (!site.IsActive)
                        continue;

                    //if (!modelCore.Ecoregion[site].Active)
                    //    continue;

                    //modelCore.Log.WriteLine("ecoregion = {0}.", modelCore.Ecoregion[site]);

                    ActiveSite activeSite = (ActiveSite) site;
                    ICommunity community = communities.Find(mapCode);
                    if (community == null)
                    {
                        throw new ApplicationException(string.Format("Unknown map code for initial community: {0}", mapCode));
                    }

                    InitializeSite(activeSite, community);
                }
            }
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
            IEnumerable<ActiveSite> sites;
            if (isSuccessionTimestep)
                sites = Model.Core.Landscape.ActiveSites;
            else
                sites = disturbedSites;

            AgeCohorts(sites, isSuccessionTimestep);
            ComputeShade(sites);
            ReproduceCohorts(sites);

            if (!isSuccessionTimestep)
                SiteVars.Disturbed.ActiveSiteValues = false;
        }

        //---------------------------------------------------------------------

        public void RunReproductionFirst()
        {
            bool isSuccessionTimestep = (Model.Core.CurrentTime % Timestep == 0);
            IEnumerable<ActiveSite> sites;
            if (isSuccessionTimestep)
                sites = Model.Core.Landscape.ActiveSites;
            else
                sites = disturbedSites;

            ComputeShade(sites);
            ReproduceCohorts(sites);
            AgeCohorts(sites, isSuccessionTimestep);

            if (!isSuccessionTimestep)
                SiteVars.Disturbed.ActiveSiteValues = false;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Advances the age of all the cohorts at certain specified sites.
        /// </summary>
        /// <param name="sites">
        /// The sites whose cohorts are to be aged.
        /// </param>
        public void AgeCohorts(IEnumerable<ActiveSite> sites,
                               bool isSuccessionTimestep)
        {
            int? succTimestep = null;
            if (isSuccessionTimestep)
            {
                succTimestep = Timestep;
                ShowProgress = true;
            }
            else
            {
                ShowProgress = false;
            }

            ProgressBar progressBar = null;
            if (ShowProgress)
            {
                System.Console.WriteLine("Ageing cohorts ...");
                prevSiteDataIndex = null;
                progressBar = Model.Core.UI.CreateProgressMeter(Model.Core.Landscape.ActiveSiteCount); // NewProgressBar();
            }

            foreach (ActiveSite site in sites)
            {
                ushort deltaTime = (ushort)(Model.Core.CurrentTime - SiteVars.TimeOfLast[site]);
                AgeCohorts(site, deltaTime, succTimestep);
                SiteVars.TimeOfLast[site] = Model.Core.CurrentTime;

                if (ShowProgress)
                    Update(progressBar, site.DataIndex);
            }
            if (ShowProgress)
                CleanUp(progressBar);
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
                                           ushort years,
                                           int? successionTimestep);

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the shade at certain specified sites.
        /// </summary>
        /// <param name="sites">
        /// The sites where shade is to be computed.
        /// </param>
        public void ComputeShade(IEnumerable<ActiveSite> sites)
        {
            ProgressBar progressBar = null;
            if (ShowProgress)
            {
                System.Console.WriteLine("Computing shade ...");
                prevSiteDataIndex = null;
                progressBar = Model.Core.UI.CreateProgressMeter(Model.Core.Landscape.ActiveSiteCount); //NewProgressBar();
            }

            foreach (ActiveSite site in sites)
            {
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
        public void ReproduceCohorts(IEnumerable<ActiveSite> sites)
        {
            logger.Debug(string.Format("{0:G}", DateTime.Now));
            int maxGeneration = GC.MaxGeneration;
            for (int generation = 0; generation <= maxGeneration; generation++)
                logger.Debug(string.Format("  gen {0}: {1}", generation,
                                           GC.CollectionCount(generation)));

            ProgressBar progressBar = null;
            if (ShowProgress)
            {
                System.Console.WriteLine("Cohort reproduction ...");
                prevSiteDataIndex = null;
                progressBar = Model.Core.UI.CreateProgressMeter(Model.Core.Landscape.ActiveSiteCount); // NewProgressBar();
            }

            foreach (ActiveSite site in sites)
            {
                Reproduction.Reproduce(site);

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

        //---------------------------------------------------------------------

        private void Update(ProgressBar progressBar,
                            uint currentSiteDataIndex)
        {
            uint increment = (uint)(prevSiteDataIndex.HasValue
                                        ? (currentSiteDataIndex - prevSiteDataIndex.Value)
                                        : currentSiteDataIndex);
            progressBar.IncrementWorkDone(increment);
            prevSiteDataIndex = currentSiteDataIndex;
        }


        //---------------------------------------------------------------------

        private void CleanUp(ProgressBar progressBar)
        {
            if (!prevSiteDataIndex.HasValue)
            {
                //    Then no sites were processed; the site iterator was a
                //    disturbed-sites iterator, and there were no disturbed
                //    sites.  So increment the progress bar to 100%.
                progressBar.IncrementWorkDone((uint)Model.Core.Landscape.ActiveSiteCount);
            }
        }

    }

}