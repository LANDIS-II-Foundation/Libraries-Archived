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
	public abstract class BaseComponent
	{
		private int timestep;
		private int nextTimeToRun;
		private bool updateNextTimeToRun;

		public bool ShowProgress;
		private uint? prevSiteDataIndex;

		//---------------------------------------------------------------------

		private static ILog logger;

		//---------------------------------------------------------------------

		static BaseComponent()
		{
			logger = LogManager.GetLogger(typeof(BaseComponent));
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The plug-in's timestep (years).
		/// </summary>
		public int Timestep
		{
			get {
				return timestep;
			}
		}

		//---------------------------------------------------------------------

		public int NextTimeToRun
		{
			get {
				return nextTimeToRun;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public BaseComponent()
		{
			this.timestep = 0;
			this.nextTimeToRun = 0;
			this.updateNextTimeToRun = false;

			this.ShowProgress = true;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes the instance and its associated site variables.
		/// </summary>
		protected void Initialize(int               timestep,
		                          double[,]         establishProbabilities,
		                          int               startTime,
		                          SeedingAlgorithms seedAlg)
		{
			this.timestep = timestep;
			this.nextTimeToRun = startTime - 1 + timestep;
			this.updateNextTimeToRun = false;

			SiteVars.Initialize();

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
			Reproduction.Initialize(establishProbabilities, algorithm);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Updates the plug-in's NextTimeToRun to the next timestep.
		/// </summary>
		protected virtual void UpdateNextTimeToRun()
		{
			nextTimeToRun += Timestep;
		}
	
		//---------------------------------------------------------------------

		public void InitializeSites(string initialCommunities,
		                            string initialCommunitiesMap)
		{
			Log.Info("Loading initial communities from file \"{0}\" ...", initialCommunities);
			InitialCommunities.DatasetParser parser = new InitialCommunities.DatasetParser(Timestep);
			InitialCommunities.IDataset communities = Data.Load<InitialCommunities.IDataset>(initialCommunities, parser);

			Log.Info("Reading initial communities map \"{0}\" ...", initialCommunitiesMap);
			IInputRaster<InitialCommunities.Pixel> map;
			map = Util.Raster.Open<InitialCommunities.Pixel>(initialCommunitiesMap);
			using (map) {
				foreach (Site site in Model.Landscape.AllSites) {
					InitialCommunities.Pixel pixel = map.ReadPixel();
					ActiveSite activeSite = site as ActiveSite;
					if (activeSite == null)
						continue;
					ushort mapCode = pixel.Band0;
					ICommunity community = communities.Find(mapCode);
					if (community == null) {
						throw Util.Raster.PixelException(activeSite.Location,
						                                 "Unknown map code for initial community: {0}",
						                                 mapCode);
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

		/// <summary>
		/// Advances the age of all the cohorts at certain specified sites.
		/// </summary>
		/// <param name="sites">
		/// The sites whose cohorts are to be aged.
		/// </param>
		/// <param name="currentTime">
		/// The current model timestep.
		/// </param>
		public void AgeCohorts(IEnumerable<MutableActiveSite> sites,
		                       int                            currentTime)
		{
			int? succTimestep = null;
			if (currentTime == nextTimeToRun) {
				succTimestep = timestep;
				updateNextTimeToRun = true;
				ShowProgress = true;
			}
			else {
				updateNextTimeToRun = false;
				ShowProgress = false;
			}

			ProgressBar progressBar = null;
			if (ShowProgress) {
				System.Console.WriteLine("Ageing cohorts ...");
				progressBar = NewProgressBar();
			}

			foreach (ActiveSite site in sites) {
				ushort deltaTime = (ushort) (currentTime - SiteVars.TimeOfLast[site]);
				AgeCohorts(site, deltaTime, succTimestep);
				SiteVars.TimeOfLast[site] = currentTime;

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
			return new ProgressBar((uint) Model.Landscape.ActiveSiteCount,
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
				//	Then no sites were processed; the site iterator was a
				//	disturbed-sites iterator, and there were no disturbed
				//	sites.  So increment the progress bar to 100%.
				progressBar.IncrementWorkDone((uint) Model.Landscape.ActiveSiteCount);
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
		/// Reproduces plants (trees) at certain specified sites.
		/// </summary>
		/// <param name="sites">
		/// The sites where plant reproduction should be done.
		/// </param>
		/// <remarks>
		/// Because this is the last stage of succession during a timestep,
		/// the NextTimeToRun is updated after all the sites are processed.
		/// </remarks>
		public void ReproducePlants(IEnumerable<MutableActiveSite> sites)
		{
			logger.Debug(string.Format("{0:G}", DateTime.Now));
			int maxGeneration = GC.MaxGeneration;
			for (int generation = 0; generation <= maxGeneration; generation++)
				logger.Debug(string.Format("  gen {0}: {1}", generation,
				                           GC.CollectionCount(generation)));

			ProgressBar progressBar = null;
			if (ShowProgress) {
				System.Console.WriteLine("Plant reproduction ...");
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

			if (updateNextTimeToRun)
				UpdateNextTimeToRun();
		}
	}
}
