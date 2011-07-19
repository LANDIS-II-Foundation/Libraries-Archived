using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using Landis.PlugIns;
using System.Collections.Generic;

namespace Landis
{
	public static class Model
	{
		private static Species.IDataset species;
		private static Ecoregions.IDataset ecoregions;
		private static Landscape.Landscape landscape;
		private static Raster.Dimensions landscapeRasterDims;
		private static float cellSizeHectares;
		private static ISuccession succession;

		//---------------------------------------------------------------------

		public static Species.IDataset Species
		{
			get {
				return species;
			}
		}

		//---------------------------------------------------------------------

		public static Ecoregions.IDataset Ecoregions
		{
			get {
				return ecoregions;
			}
		}

		//---------------------------------------------------------------------

		public static Landscape.ILandscape Landscape
		{
			get {
				return landscape;
			}
		}

		//---------------------------------------------------------------------

		public static Raster.Dimensions LandscapeRasterDimensions
		{
			get {
				return landscapeRasterDims;
			}
		}

		//---------------------------------------------------------------------

		public static float CellSizeHectares
		{
			get {
				return cellSizeHectares;
			}
		}

		//---------------------------------------------------------------------

		public static Cohorts.ISuccession<T> GetSuccession<T>()
		{
			return succession as Cohorts.ISuccession<T>;
		}

		//---------------------------------------------------------------------

		public static class SiteVars
		{
			private static Landscape.ISiteVar<Ecoregions.IEcoregion> ecoregion;
			private static Landscape.ISiteVar<bool> disturbed;

			//-----------------------------------------------------------------

			public static Landscape.ISiteVar<Ecoregions.IEcoregion> Ecoregion
			{
				get {
					return ecoregion;
				}
			}

			//-----------------------------------------------------------------

			public static Landscape.ISiteVar<bool> Disturbed
			{
				get {
					return disturbed;
				}
			}

			//-----------------------------------------------------------------

			internal static void Initialize(Ecoregions.Map ecoregionMap)
			{
				ecoregion = ecoregionMap.CreateSiteVar(Model.Landscape);
				disturbed = Model.Landscape.NewSiteVar<bool>();
			}
		}

		//---------------------------------------------------------------------
	
		/// <summary>
		/// Runs a model scenario.
		/// </summary>
		public static void Run(string scenarioPath)
		{
			//  Initialize plug-ins manager with the default plug-ins
			//	database in the folder where application resides.
			PlugIns.Manager.Initialize(PlugIns.Database.DefaultPath);

			IScenario scenario = LoadScenario(scenarioPath);

			LoadSpecies(scenario.Species);
			LoadEcoregions(scenario.Ecoregions);

			Log.Info("Initializing landscape from ecoregions map \"{0}\" ...", scenario.EcoregionsMap);
			Ecoregions.Map ecoregionsMap = new Ecoregions.Map(scenario.EcoregionsMap,
			                                                  Model.Ecoregions);
			using (Landscape.IInputGrid<bool> grid = ecoregionsMap.OpenAsInputGrid()) {
				landscape = new Landscape.Landscape(grid);
			}
			SiteVars.Initialize(ecoregionsMap);

			//  Load and initialize plug-ins.

			Log.Info("Loading {0} plug-in ...", scenario.Succession.Info.Name);
			succession = PlugIns.Manager.Load<ISuccession>(scenario.Succession.Info);
			succession.Initialize(scenario.Succession.InitFile);
			succession.InitializeSites(scenario.InitialCommunities,
			                           scenario.InitialCommunitiesMap);

			IDisturbance[] disturbancePlugIns = LoadPlugIns<IDisturbance>(scenario.Disturbances);
			IOutput[] outputPlugIns = LoadPlugIns<IOutput>(scenario.Outputs);

			//  Run those output plug-ins whose next time to run is 0.
			foreach (IOutput outPlugIn in GetPlugInsToRun<IOutput>(outputPlugIns, 0))
				outPlugIn.Run(0);

			//******************// for Rob
			//  Main time loop  //
			//******************//
			// currentTime (years)
			for (int currentTime = 1; currentTime <= scenario.Duration; ++currentTime) {
				List<IDisturbance> distPlugInsToRun;
				distPlugInsToRun = GetPlugInsToRun<IDisturbance>(disturbancePlugIns, currentTime);
				bool isDistTimestep = distPlugInsToRun.Count > 0;

				List<IOutput> outPlugInsToRun;
				outPlugInsToRun = GetPlugInsToRun<IOutput>(outputPlugIns, currentTime);
				bool isOutTimestep = outPlugInsToRun.Count > 0;

				bool isSuccTimestep = succession.NextTimeToRun == currentTime;

				//  If not a succession timestep, a disturance timestep or
				//  an output timestep, then go to the next timestep.
				if (! (isSuccTimestep || isDistTimestep || isOutTimestep))
					continue;

				Log.Info("Current time: {0}", currentTime);

				if (isDistTimestep) {
					if (scenario.DisturbancesRandomOrder)
						distPlugInsToRun = Shuffle(distPlugInsToRun);
					foreach (IDisturbance distPlugIn in distPlugInsToRun)
						distPlugIn.Run(currentTime);
				}

				if (isSuccTimestep || isDistTimestep) {
					IEnumerable<ActiveSite> sites;
					if (isSuccTimestep)
						sites = Model.Landscape.ActiveSites;
					else
						sites = DisturbedSites();
					succession.AgeCohorts(sites, currentTime);
					succession.ComputeShade(sites);
					succession.ReproducePlants(sites);
				}

				//  Run output plug-ins.
				foreach (IOutput outPlugIn in outPlugInsToRun)
					outPlugIn.Run(currentTime);

				if (! isSuccTimestep) {
					SiteVars.Disturbed.ActiveSiteValues = false;
				}
			}  // main time loop
		}

		//---------------------------------------------------------------------

		private static IScenario LoadScenario(string path)
		{
			Log.Info("Loading scenario from file \"{0}\" ...", path);
			ScenarioParser parser = new ScenarioParser();
			return Data.Load<IScenario>(path, parser);
		}

		//---------------------------------------------------------------------

		private static void LoadSpecies(string path)
		{
			Log.Info("Loading species data from file \"{0}\" ...", path);
			Species.DatasetParser parser = new Species.DatasetParser();
			species = Data.Load<Species.IDataset>(path, parser);
		}

		//---------------------------------------------------------------------

		private static void LoadEcoregions(string path)
		{
			Log.Info("Loading ecoregions from file \"{0}\" ...", path);
			Ecoregions.DatasetParser parser = new Ecoregions.DatasetParser();
			ecoregions = Data.Load<Ecoregions.IDataset>(path, parser);
		}

		//---------------------------------------------------------------------

		private static T[] LoadPlugIns<T>(IPlugIn[] plugIns)
			where T : PlugIns.IPlugInWithTimestep
		{
			T[] loadedPlugIns = new T[plugIns.Length];
			foreach (int i in Indexes.Of(plugIns)) {
				IPlugIn plugIn = plugIns[i];
				Log.Info("Loading {0} plug-in ...", plugIn.Info.Name);
				T loadedPlugIn = PlugIns.Manager.Load<T>(plugIn.Info);
				loadedPlugIn.Initialize(plugIn.InitFile);
				loadedPlugIns[i] = loadedPlugIn;
			}
			return loadedPlugIns;
		}

		//---------------------------------------------------------------------

		private static List<T> GetPlugInsToRun<T>(T[] plugIns,
		                                          int currentTime)
			where T : PlugIns.IPlugInWithTimestep
		{
			List<T> plugInsToRun = new List<T>();
			foreach (T plugIn in plugIns) {
				if (plugIn.NextTimeToRun == currentTime)
					plugInsToRun.Add(plugIn);
			}
			return plugInsToRun;
		}

		//---------------------------------------------------------------------

		private static List<IDisturbance> Shuffle(List<IDisturbance> list)
		{
			// TODO
			return list;
		}

		//---------------------------------------------------------------------

		public static IEnumerable<ActiveSite> DisturbedSites()
		{
			// TODO
			return null;
		}
	}
}
