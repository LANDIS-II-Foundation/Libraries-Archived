using Landis.Landscape;
using System.Collections.Generic;

namespace Landis
{
	public static class Model
	{
		private static object log;
//		private static Ecoregion[] ecoregions;
		private static Raster.IRasterFactory rasterFactory;
		private static Landscape.Landscape landscape;
		private static Raster.Dimensions landscapeRasterDims;
//		private static Util.Random.Uniform<float> randomNumGen;
		private static float cellSizeHectares;

		//---------------------------------------------------------------------

		public static object Log
		{
			get {
				return log;
			}
		}

		//---------------------------------------------------------------------
/*
		public static Ecoregion[] Ecoregions
		{
			get {
				return ecoregions;
			}
		}
*/
		//---------------------------------------------------------------------

		public static Raster.IRasterFactory RasterFactory
		{
			get {
				return rasterFactory;
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
/*
		public static Util.Random.Uniform<float> RandomNumGenerator
		{
			get {
				return randomNumGen;
			}
		}
*/
		//---------------------------------------------------------------------

		public static float CellSizeHectares
		{
			get {
				return cellSizeHectares;
			}
		}

		//---------------------------------------------------------------------

		public static class SiteVars
		{
//			private static Landscape.ISiteVar<Ecoregion> ecoregion;
			private static Landscape.ISiteVar<int> timeLastSuccession;
			private static Landscape.ISiteVar<byte> shade;
			private static Landscape.ISiteVar<IReproduction> reproduction;
			private static Landscape.ISiteVar<bool> disturbed;

			//-----------------------------------------------------------------

#if ECOREGIONS
			public static Landscape.ISiteVar<Ecoregion> Ecoregion
			{
				get {
					return ecoregion;
				}
			}
#endif

			//-----------------------------------------------------------------

			public static Landscape.ISiteVar<int> TimeLastSuccession
			{
				get {
					return timeLastSuccession;
				}
			}

			//-----------------------------------------------------------------

			public static Landscape.ISiteVar<byte> Shade
			{
				get {
					return shade;
				}
			}

			//-----------------------------------------------------------------

			public static Landscape.ISiteVar<IReproduction> Reproduction
			{
				get {
					return reproduction;
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

			internal static void Initialize()
			{
				timeLastSuccession = Model.Landscape.NewSiteVar<int>();
				shade              = Model.Landscape.NewSiteVar<byte>();
				reproduction       = Model.Landscape.NewSiteVar<IReproduction>();
				disturbed          = Model.Landscape.NewSiteVar<bool>();
			}
		}

		//---------------------------------------------------------------------
	
		/// <summary>
		/// Runs a model scenario.
		/// </summary>
		public static void Run(IScenario scenario)
		{
			try {
				//  Load species dataset from file (scenario.Species)
				//  Load ecoregions dataset from file (scenario.Ecoregions)
				//  Create landscape using scenario.EcoregionMap
				//		IIndexableGrid<bool> grid = Ecoregions.OpenAsBoolGrid(scenario.EcoregionMap);
				//		Model.Landscape = new Landscape.Landscape(grid);
				SiteVars.Initialize();
	
				//  Initialize succession.
				ISuccession successionPlugIn = PlugIn.Manager.Load<ISuccession>(scenario.Succession);
				Succession succession = new Succession(successionPlugIn);
				InitializeSites(successionPlugIn, scenario.InitialSiteClasses);

				//  Default form of reproduction is seeding using the seed
				//	dispersal algorithm from the succession plug-in.
				Seeding seeding = new Seeding(successionPlugIn.DisperseSeed);
				SiteVars.Reproduction.ActiveSiteValues = seeding;

				//  Load disturbance plug-ins.
				IDisturbance[] disturbancePlugIns = new IDisturbance[scenario.Disturbances.Length];
				int i = 0;
				foreach (PlugIn.Info distPlugInInfo in scenario.Disturbances) {
					disturbancePlugIns[i] = PlugIn.Manager.Load<IDisturbance>(distPlugInInfo);
					i++;
				}
	
				//  Load output plug-ins
	
				//******************// for Rob
				//  Main time loop  //
				//******************//
				// currentTime (years)
				for (int currentTime = 1; currentTime <= scenario.Duration; ++currentTime) {
					List<IDisturbance> distPlugInsToRun = new List<IDisturbance>();
					foreach (IDisturbance distPlugIn in disturbancePlugIns)
						if (distPlugIn.NextTimeToRun == currentTime)
							distPlugInsToRun.Add(distPlugIn);
					bool isDistTimestep = distPlugInsToRun.Count > 0;

					bool isSuccTimestep = succession.NextTimeToRun == currentTime;

					//  If not a succession timestep and not a disturance
					//	timestep, then go to the next timestep.
					if (! isSuccTimestep && ! isDistTimestep)
						continue;

					if (isDistTimestep) {
						if (scenario.DisturbancesRandomOrder)
							distPlugInsToRun = Shuffle(distPlugInsToRun);
						foreach (IDisturbance distPlugIn in distPlugInsToRun)
							distPlugIn.Run(currentTime);
					}

					IEnumerable<ActiveSite> sites;
					if (isSuccTimestep)
						sites = Model.Landscape.ActiveSites;
					else
						sites = DisturbedSites();
					succession.AgeCohorts(sites, currentTime);
					succession.ComputeShade(sites);
					succession.ReproducePlants(sites);

					//  Run output plug-ins.

					if (! isSuccTimestep) {
						SiteVars.Reproduction.ActiveSiteValues = seeding;
						SiteVars.Disturbed.ActiveSiteValues = false;
					}
				}  // main time loop
			}
			finally {
				PlugIn.Manager.UnloadAll();
			}
		}

		//---------------------------------------------------------------------

		public static void InitializeSites(ISuccession 				     successionPlugIn,
		                                   SiteInitialization.InputFiles inputFiles)
		{
			SiteInitialization.IClasses initialSiteClasses = new SiteInitialization.Classes(inputFiles);
			foreach (ActiveSite site in Model.Landscape) {
				int classId = initialSiteClasses.Map[site];
				SiteInitialization.IClass initialSiteClass = initialSiteClasses[classId];
				if (initialSiteClass == null) {
					string message = string.Format("Error at site {0} in map \"{1}\": Unknown class id: {2}",
					                               site.Location, inputFiles.Map, classId);
					throw new System.ApplicationException(message);
				}
				successionPlugIn.InitializeSite(site, initialSiteClass);
			}
			// TODO - Close the input raster.
		}

		//---------------------------------------------------------------------

		public static List<IDisturbance> Shuffle(List<IDisturbance> list)
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
