using Landis.InitialCommunities;
using Landis.Landscape;
using Landis.Species;
using Landis.Raster;
using Landis.Util;
using System.Collections.Generic;

namespace Landis.Succession
{
	/// <summary>
	/// Base component upon which a succession plug-in can built.
	/// </summary>
	public abstract class BaseComponent
	{
		private int timestep;
		private int nextTimeToRun;
		private IReproduction defaultReproduction;

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
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes the instance and its associated site variables.
		/// </summary>
		protected void Initialize(int       timestep,
		                          double[,] establishProbabilities)
		{
			this.timestep = timestep;
			this.nextTimeToRun = timestep;

			SiteVars.Initialize();
			defaultReproduction = new Seeding(WardSeedDispersal.Algorithm);
			SiteVars.Reproduction.ActiveSiteValues = defaultReproduction;
			Reproduction.SetEstablishProbabilities(establishProbabilities);
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
			InitialCommunities.DatasetParser parser = new InitialCommunities.DatasetParser();
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
					byte mapCode = pixel.Band0;
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
		public void AgeCohorts(IEnumerable<ActiveSite> sites,
		                       int                     currentTime)
		{
			int? succTimestep = null;
			if (currentTime == nextTimeToRun)
				succTimestep = timestep;
			foreach (ActiveSite site in sites) {
				ushort deltaTime = (ushort) (currentTime - SiteVars.TimeOfLast[site]);
				AgeCohorts(site, deltaTime, succTimestep);
				SiteVars.TimeOfLast[site] = currentTime;
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
		public void ComputeShade(IEnumerable<ActiveSite> sites)
		{
			foreach (ActiveSite site in sites)
				SiteVars.Shade[site] = ComputeShade(site);
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
		public void ReproducePlants(IEnumerable<ActiveSite> sites)
		{
			foreach (ActiveSite site in sites)
				SiteVars.Reproduction[site].Do(site);

			SiteVars.Reproduction.ActiveSiteValues = defaultReproduction;
			UpdateNextTimeToRun();
		}
	}
}
