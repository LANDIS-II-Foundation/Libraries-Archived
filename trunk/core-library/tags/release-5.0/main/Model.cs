using Edu.Wisc.Forest.Flel.Util;
using Gov.Fgdc.Csdgm;
using Landis.Landscape;
using Landis.PlugIns;
using System;
using System.Collections.Generic;

namespace Landis
{
	public static class Model
	{
		private static Species.IDataset species;
		private static Ecoregions.IDataset ecoregions;
		private static Landscape.Landscape landscape;
		private static RasterIO.Dimensions landscapeMapDims;
		private static RasterIO.IMetadata landscapeMapMetadata;
		private static float cellLength;  // meters
		private static float cellArea;    // hectares
		private static SiteVariables siteVars;
		private static ISuccession succession;
		private static List<PlugIns.I2PhaseInitialization> plugInsWith2PhaseInit;
		private static List<PlugIns.ICleanUp> plugInsWithCleanUp;

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

		public static RasterIO.Dimensions LandscapeMapDims
		{
			get {
				return landscapeMapDims;
			}
		}

		//---------------------------------------------------------------------

		public static RasterIO.IMetadata LandscapeMapMetadata
		{
			get {
				return landscapeMapMetadata;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The length of a cell's side (meters).
		/// </summary>
		public static float CellLength
		{
			get {
				return cellLength;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The area of a cell (hectares).
		/// </summary>
		public static float CellArea
		{
			get {
				return cellArea;
			}
		}

		//---------------------------------------------------------------------

		public static SiteVariables SiteVars
		{
			get {
				return siteVars;
			}
		}

		//---------------------------------------------------------------------

		public static Cohorts.ISuccession<TCohort> GetSuccession<TCohort>()
		{
			return succession as Cohorts.ISuccession<TCohort>;
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
			int startTime = 1;
			InitializeRandomNumGenerator(scenario.RandomNumberSeed);

			LoadSpecies(scenario.Species);
			LoadEcoregions(scenario.Ecoregions);

			UI.WriteLine("Initializing landscape from ecoregions map \"{0}\" ...", scenario.EcoregionsMap);
			Ecoregions.Map ecoregionsMap = new Ecoregions.Map(scenario.EcoregionsMap,
			                                                  Model.Ecoregions);
			ProcessMetadata(ecoregionsMap.Metadata, scenario);
			using (Landscape.IInputGrid<bool> grid = ecoregionsMap.OpenAsInputGrid()) {
				UI.WriteLine("Map dimensions: {0} = {1:#,##0} cell{2}", grid.Dimensions,
				             grid.Count, (grid.Count == 1 ? "" : "s"));
				landscape = new Landscape.Landscape(grid);
			}
			UI.WriteLine("Sites: {0:#,##0} active ({1:p1}), {2:#,##0} inactive ({3:p1})",
			             landscape.ActiveSiteCount, (landscape.Count > 0 ? ((double)landscape.ActiveSiteCount)/landscape.Count : 0),
			             landscape.InactiveSiteCount, (landscape.Count > 0 ? ((double)landscape.InactiveSiteCount)/landscape.Count : 0));
			landscapeMapDims = new RasterIO.Dimensions((int) landscape.Rows,
			                                           (int) landscape.Columns);
			siteVars = new SiteVariables(landscape, ecoregionsMap);

			DisturbedSiteEnumerator disturbedSites = new DisturbedSiteEnumerator(landscape,
			                                                                     SiteVars.Disturbed);

			plugInsWith2PhaseInit = new List<PlugIns.I2PhaseInitialization>();
			plugInsWithCleanUp = new List<PlugIns.ICleanUp>();
			try {
				//  Load and initialize plug-ins.
	
				UI.WriteLine("Loading {0} plug-in ...", scenario.Succession.Info.Name);
				succession = PlugIns.Manager.Load<ISuccession>(scenario.Succession.Info);
				succession.Initialize(scenario.Succession.InitFile, startTime);
				succession.InitializeSites(scenario.InitialCommunities,
				                           scenario.InitialCommunitiesMap);
	
				IDisturbance[] disturbancePlugIns = LoadPlugIns<IDisturbance>(scenario.Disturbances, startTime);
				IOutput[] outputPlugIns = LoadPlugIns<IOutput>(scenario.Outputs, startTime);

				//	Perform 2nd phase of initialization for those plug-ins
				//	that have it.
				foreach (PlugIns.I2PhaseInitialization plugIn in plugInsWith2PhaseInit)
					plugIn.InitializePhase2();

				//  Run those output plug-ins whose next time to run is startTime-1.
				foreach (IOutput outPlugIn in GetPlugInsToRun<IOutput>(outputPlugIns, startTime-1))
					outPlugIn.Run(startTime-1);
	
				//******************// for Rob
				//  Main time loop  //
				//******************//
				// currentTime (years)
				int endTime = startTime + scenario.Duration - 1;
				for (int currentTime = startTime; currentTime <= endTime; ++currentTime) {
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
	
					UI.WriteLine("Current time: {0}", currentTime);
	
					if (isDistTimestep) {
						if (scenario.DisturbancesRandomOrder)
							Util.Random.Shuffle(distPlugInsToRun);
						foreach (IDisturbance distPlugIn in distPlugInsToRun)
							distPlugIn.Run(currentTime);
					}
	
					if (isSuccTimestep || isDistTimestep) {
						IEnumerable<MutableActiveSite> sites;
						if (isSuccTimestep)
							sites = Model.Landscape.ActiveSites;
						else
							sites = disturbedSites;
						succession.AgeCohorts(sites, currentTime);
						succession.ComputeShade(sites);
						succession.ReproduceCohorts(sites);
					}
	
					//  Run output plug-ins.
					foreach (IOutput outPlugIn in outPlugInsToRun)
						outPlugIn.Run(currentTime);
	
					if (! isSuccTimestep) {
						SiteVars.Disturbed.ActiveSiteValues = false;
					}
				}  // main time loop
			}
			finally {
				foreach (PlugIns.ICleanUp plugIn in plugInsWithCleanUp)
					plugIn.CleanUp();
			}
		}

		//---------------------------------------------------------------------

		private static IScenario LoadScenario(string path)
		{
			UI.WriteLine("Loading scenario from file \"{0}\" ...", path);
			ScenarioParser parser = new ScenarioParser();
			return Data.Load<IScenario>(path, parser);
		}

		//---------------------------------------------------------------------

		private static void InitializeRandomNumGenerator(uint? seed)
		{
			if (seed.HasValue)
				Landis.Util.Random.Initialize(seed.Value);
			else {
				uint generatedSeed = Landis.Util.Random.GenerateSeed();
				Landis.Util.Random.Initialize(generatedSeed);
				UI.WriteLine("Initialized random number generator with seed = {0:#,##0}", generatedSeed);
			}
		}

		//---------------------------------------------------------------------

		private static void LoadSpecies(string path)
		{
			UI.WriteLine("Loading species data from file \"{0}\" ...", path);
			Species.DatasetParser parser = new Species.DatasetParser();
			species = Data.Load<Species.IDataset>(path, parser);
		}

		//---------------------------------------------------------------------

		private static void LoadEcoregions(string path)
		{
			UI.WriteLine("Loading ecoregions from file \"{0}\" ...", path);
			Ecoregions.DatasetParser parser = new Ecoregions.DatasetParser();
			ecoregions = Data.Load<Ecoregions.IDataset>(path, parser);
		}

		//---------------------------------------------------------------------

		private const string cellLengthExceptionPrefix = "Cell Length Exception: ";

		//---------------------------------------------------------------------

		private static void ProcessMetadata(RasterIO.IMetadata metadata,
		                                    IScenario          scenario)
		{
			landscapeMapMetadata = metadata;

			string warning = "";
			float? mapCellLength = null;
			string mapCellLengthStr = "";
			try {
				mapCellLength = GetCellLength(metadata, ref mapCellLengthStr);
			}
			catch (ApplicationException exc) {
				string message = exc.Message;
				if (! message.StartsWith(cellLengthExceptionPrefix))
					throw;
				message = message.Replace(cellLengthExceptionPrefix, "");
				if (scenario.CellLength.HasValue)
					warning = message;
				else
					throw new ApplicationException("Error: " + message);
			}

			if (scenario.CellLength.HasValue) {
				cellLength = scenario.CellLength.Value;
				UI.WriteLine("Cell length: {0} meters", cellLength);
				if (mapCellLength.HasValue) {
					if (cellLength == mapCellLength.Value)
						UI.WriteLine("Cell length in map: {0}", mapCellLengthStr);
					else
						UI.WriteLine("Warning: Cell length in map: {0}", mapCellLengthStr);
				}
				else {
					if (warning.Length > 0)
						UI.WriteLine("Warning: {0}", warning);
					else
						UI.WriteLine("Map has no cell length");
				}
			}
			else {
				//	No CellLength parameter in scenario file
				if (mapCellLength.HasValue) {
					UI.WriteLine("Cell length in map: {0}", mapCellLengthStr);
					cellLength = mapCellLength.Value;
				}
				else {
					string[] message = new string[] {
						"Error: Ecoregion map doesn't have cell dimensions; therefore, the",
						"       CellLength parameter must be in the scenario file."
					};
					throw new MultiLineException(message);
				}
			}

			cellArea = (float) ((cellLength * cellLength) / 10000);
		}

		//---------------------------------------------------------------------

		private static float? GetCellLength(RasterIO.IMetadata metadata,
		                                    ref string         cellLengthStr)
		{
			float mapCellLength = 0;

			float cellWidth = 0;
			bool hasCellWidth = metadata.TryGetValue(AbscissaResolution.Name,
			                                         ref cellWidth);
			float cellHeight = 0;
			bool hasCellHeight = metadata.TryGetValue(OrdinateResolution.Name,
			                                          ref cellHeight);
			if (hasCellWidth && hasCellHeight) {
				if (cellWidth != cellHeight)
					throw CellLengthException("Cell width ({0}) in map is not = to cell height ({1})",
				                              cellWidth, cellHeight);
				if (cellWidth <= 0.0)
					throw CellLengthException("Cell width ({0}) in map is not > 0",
					                          cellWidth);

				string units = null;
				if (! metadata.TryGetValue(PlanarDistanceUnits.Name, ref units)) {
					UI.WriteLine("Map doesn't have units for cell dimensions; assuming meters");
					units = PlanarDistanceUnits.Meters;
				}
				if (units == PlanarDistanceUnits.Meters)
					mapCellLength = cellWidth;
				else if (units == PlanarDistanceUnits.InternationalFeet)
					mapCellLength = (float) (cellWidth * 0.3048);
				else if (units == PlanarDistanceUnits.SurveyFeet)
					mapCellLength = (float) (cellWidth * (1200.0 / 3937));
				else
					throw CellLengthException("Map has unknown units for cell dimensions: {0}",
					                          units);
				cellLengthStr = string.Format("{0} meters{1}", mapCellLength,
								              (units == PlanarDistanceUnits.Meters) ? ""
								         									        : string.Format(" ({0} {1})", cellWidth, units));
				return mapCellLength;
			}
			else if (hasCellWidth && !hasCellHeight) {
				throw CellLengthException("Map has cell width (x-dimension) but no height (y-dimension).");
			}
			else if (!hasCellWidth && hasCellHeight) {
				throw CellLengthException("Map has cell height (y-dimension) but no width (x-dimension).");
			}
			return null;
		}

		//---------------------------------------------------------------------

		private static ApplicationException CellLengthException(string          message,
		                                                        params object[] mesgArgs)
		{
			string excMessage;
			if (mesgArgs == null)
				excMessage = message;
			else
				excMessage = string.Format(message, mesgArgs);
			return new ApplicationException(cellLengthExceptionPrefix + excMessage);
		}

		//---------------------------------------------------------------------

		private static T[] LoadPlugIns<T>(IPlugIn[] plugIns,
		                                  int       startTime)
			where T : PlugIns.IPlugInWithTimestep
		{
			T[] loadedPlugIns = new T[plugIns.Length];
			foreach (int i in Indexes.Of(plugIns)) {
				IPlugIn plugIn = plugIns[i];
				UI.WriteLine("Loading {0} plug-in ...", plugIn.Info.Name);
				T loadedPlugIn = PlugIns.Manager.Load<T>(plugIn.Info);
				loadedPlugIn.Initialize(plugIn.InitFile, startTime);
				loadedPlugIns[i] = loadedPlugIn;
				if (loadedPlugIn is PlugIns.I2PhaseInitialization)
					plugInsWith2PhaseInit.Add((PlugIns.I2PhaseInitialization) loadedPlugIn);
				if (loadedPlugIn is PlugIns.ICleanUp)
					plugInsWithCleanUp.Add((PlugIns.ICleanUp) loadedPlugIn);
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
	}
}
