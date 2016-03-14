using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Loader = Edu.Wisc.Forest.Flel.Util.PlugIns.Loader;

using Gov.Fgdc.Csdgm;

using Landis.Landscape;
using Landis.PlugIns;
using Landis.RasterIO;

using System;
using System.Collections.Generic;

namespace Landis
{
    public class Model
        : PlugIns.ICore
    {
        private PlugIns.IDataset plugInDataset;
        private RasterIO.IDriverManager rasterDriverMgr;
        private SiteVarRegistry siteVarRegistry;
        private Species.IDataset species;
        private Ecoregions.IDataset ecoregions;
        private Landscape.Landscape landscape;
        private RasterIO.IMetadata landscapeMapMetadata;
        private float cellLength;  // meters
        private float cellArea;    // hectares
        private ISiteVar<Ecoregions.IEcoregion> ecoregionSiteVar;
        private int startTime;
        private int endTime;
        private int currentTime;
        private int timeSinceStart;
        private SuccessionPlugIn succession;
        private List<PlugIns.I2PhaseInitialization> plugInsWith2PhaseInit;
        private List<PlugIns.ICleanUp> plugInsWithCleanUp;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Model(PlugIns.IDataset        plugInDataset,
                     RasterIO.IDriverManager rasterDriverMgr)
        {
            this.plugInDataset = plugInDataset;
            this.rasterDriverMgr = rasterDriverMgr;
            siteVarRegistry = new SiteVarRegistry();
        }

        //---------------------------------------------------------------------

        IInputRaster<TPixel> IRasterFactory.OpenRaster<TPixel>(string path)
        {
            return rasterDriverMgr.OpenRaster<TPixel>(path);
        }

        //---------------------------------------------------------------------

        IOutputRaster<TPixel> IRasterFactory.CreateRaster<TPixel>(string     path,
                                                                  Dimensions dimensions,
                                                                  IMetadata  metadata)
        {
            try {
                string dir = System.IO.Path.GetDirectoryName(path);
                if (dir.Length > 0)
                    Directory.EnsureExists(dir);
                return rasterDriverMgr.CreateRaster<TPixel>(path, dimensions, metadata);
            }
            catch (System.IO.IOException exc) {
                string mesg = string.Format("Error opening map \"{0}\"", path);
                throw new MultiLineException(mesg, exc);
            }
        }

        //---------------------------------------------------------------------

        Species.IDataset ICore.Species
        {
            get {
                return species;
            }
        }

        //---------------------------------------------------------------------

        Ecoregions.IDataset ICore.Ecoregions
        {
            get {
                return ecoregions;
            }
        }

        //---------------------------------------------------------------------

        ISiteVar<Ecoregions.IEcoregion> ICore.Ecoregion
        {
            get {
                return ecoregionSiteVar;
            }
        }

        //---------------------------------------------------------------------

        Landscape.ILandscape ICore.Landscape
        {
            get {
                return landscape;
            }
        }

        //---------------------------------------------------------------------

        RasterIO.IMetadata ICore.LandscapeMapMetadata
        {
            get {
                return landscapeMapMetadata;
            }
        }

        //---------------------------------------------------------------------

        float ICore.CellLength
        {
            get {
                return cellLength;
            }
        }

        //---------------------------------------------------------------------

        float ICore.CellArea
        {
            get {
                return cellArea;
            }
        }

        //---------------------------------------------------------------------

        int ICore.StartTime
        {
            get {
                return startTime;
            }
        }

        //---------------------------------------------------------------------

        int ICore.EndTime
        {
            get {
                return endTime;
            }
        }

        //---------------------------------------------------------------------

        int ICore.CurrentTime
        {
            get {
                return currentTime;
            }
        }

        //---------------------------------------------------------------------

        int ICore.TimeSinceStart
        {
            get {
                return timeSinceStart;
            }
        }

        //---------------------------------------------------------------------

        Cohorts.TypeIndependent.ILandscapeCohorts ICore.SuccessionCohorts
        {
            get {
                if (succession == null)
                    return null;
                return succession.Cohorts;
            }
        }

        //---------------------------------------------------------------------

        void ICore.RegisterSiteVar(ISiteVariable siteVar,
                                   string        name)
        {
            siteVarRegistry.RegisterVar(siteVar, name);
        }

        //---------------------------------------------------------------------

        ISiteVar<T> ICore.GetSiteVar<T>(string name)
        {
            return siteVarRegistry.GetVar<T>(name);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Runs a model scenario.
        /// </summary>
        public void Run(string scenarioPath)
        {
            siteVarRegistry.Clear();

            Scenario scenario = LoadScenario(scenarioPath);
            startTime = scenario.StartTime;
            endTime = scenario.EndTime;
            timeSinceStart = 0;
            currentTime = startTime;
            InitializeRandomNumGenerator(scenario.RandomNumberSeed);

            LoadSpecies(scenario.Species);
            LoadEcoregions(scenario.Ecoregions);

            UI.WriteLine("Initializing landscape from ecoregions map \"{0}\" ...", scenario.EcoregionsMap);
            Ecoregions.Map ecoregionsMap = new Ecoregions.Map(scenario.EcoregionsMap,
                                                              ecoregions,
                                                              rasterDriverMgr);
            ProcessMetadata(ecoregionsMap.Metadata, scenario);
            using (IInputGrid<bool> grid = ecoregionsMap.OpenAsInputGrid()) {
                UI.WriteLine("Map dimensions: {0} = {1:#,##0} cell{2}", grid.Dimensions,
                             grid.Count, (grid.Count == 1 ? "" : "s"));
                landscape = new Landscape.Landscape(grid);
            }
            UI.WriteLine("Sites: {0:#,##0} active ({1:p1}), {2:#,##0} inactive ({3:p1})",
                         landscape.ActiveSiteCount, (landscape.Count > 0 ? ((double)landscape.ActiveSiteCount)/landscape.Count : 0),
                         landscape.InactiveSiteCount, (landscape.Count > 0 ? ((double)landscape.InactiveSiteCount)/landscape.Count : 0));

            ecoregionSiteVar = ecoregionsMap.CreateSiteVar(landscape);

            plugInsWith2PhaseInit = new List<PlugIns.I2PhaseInitialization>();
            plugInsWithCleanUp = new List<PlugIns.ICleanUp>();

            try {
                UI.WriteLine("Loading {0} plug-in ...", scenario.Succession.Info.Name);
                succession = Loader.Load<SuccessionPlugIn>(scenario.Succession.Info);
                succession.Initialize(scenario.Succession.InitFile, this);
                succession.InitializeSites(scenario.InitialCommunities,
                                           scenario.InitialCommunitiesMap);

                PlugIn[] disturbancePlugIns = LoadAndInitPlugIns(scenario.Disturbances);
                PlugIn[] otherPlugIns = LoadAndInitPlugIns(scenario.OtherPlugIns);

                //    Perform 2nd phase of initialization for those plug-ins
                //    that have it.
                foreach (PlugIns.I2PhaseInitialization plugIn in plugInsWith2PhaseInit)
                    plugIn.InitializePhase2();

                //  Run output plug-ins for TimeSinceStart = 0 (time step 0)
                foreach (PlugIn plugIn in otherPlugIns) {
                    if (plugIn.PlugInType.IsMemberOf("output"))
                        Run(plugIn);
                }

                //******************// for Rob
                //  Main time loop  //
                //******************//

                for (currentTime++, timeSinceStart++;
                     currentTime <= endTime;
                     currentTime++, timeSinceStart++) {

                    bool isSuccTimestep = IsPlugInTimestep(succession);

                    List<PlugIn> distPlugInsToRun;
                    distPlugInsToRun = GetPlugInsToRun(disturbancePlugIns);
                    bool isDistTimestep = distPlugInsToRun.Count > 0;
    
                    List<PlugIn> otherPlugInsToRun;
                    otherPlugInsToRun = GetPlugInsToRun(otherPlugIns);
    
                    if (!isSuccTimestep && !isDistTimestep 
                                        && otherPlugInsToRun.Count == 0)
                        continue;
    
                    UI.WriteLine("Current time: {0}", currentTime);
    
                    if (isDistTimestep) {
                        if (scenario.DisturbancesRandomOrder)
                            Util.Random.Shuffle(distPlugInsToRun);
                        foreach (PlugIn distPlugIn in distPlugInsToRun)
                            Run(distPlugIn);
                    }
    
                    if (isSuccTimestep || isDistTimestep)
                        Run(succession);
    
                    foreach (PlugIn otherPlugIn in otherPlugInsToRun)
                        Run(otherPlugIn);
                }  // main time loop
            }
            finally {
                foreach (PlugIns.ICleanUp plugIn in plugInsWithCleanUp)
                    plugIn.CleanUp();
            }
        }

        //---------------------------------------------------------------------

        private Scenario LoadScenario(string path)
        {
            UI.WriteLine("Loading scenario from file \"{0}\" ...", path);
            ScenarioParser parser = new ScenarioParser(plugInDataset);
            return Data.Load<Scenario>(path, parser);
        }

        //---------------------------------------------------------------------

        private void InitializeRandomNumGenerator(uint? seed)
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

        private void LoadSpecies(string path)
        {
            UI.WriteLine("Loading species data from file \"{0}\" ...", path);
            Species.DatasetParser parser = new Species.DatasetParser();
            species = Data.Load<Species.IDataset>(path, parser);
        }

        //---------------------------------------------------------------------

        private void LoadEcoregions(string path)
        {
            UI.WriteLine("Loading ecoregions from file \"{0}\" ...", path);
            Ecoregions.DatasetParser parser = new Ecoregions.DatasetParser();
            ecoregions = Data.Load<Ecoregions.IDataset>(path, parser);
        }

        //---------------------------------------------------------------------

        private const string cellLengthExceptionPrefix = "Cell Length Exception: ";

        //---------------------------------------------------------------------

        private void ProcessMetadata(RasterIO.IMetadata metadata,
                                     Scenario           scenario)
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
                //    No CellLength parameter in scenario file
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

        private float? GetCellLength(RasterIO.IMetadata metadata,
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

        private ApplicationException CellLengthException(string          message,
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

        private PlugIn[] LoadAndInitPlugIns(PlugInAndInitFile[] plugIns)
        {
            PlugIn[] loadedPlugIns = new PlugIn[plugIns.Length];
            foreach (int i in Indexes.Of(plugIns)) {
                PlugInAndInitFile plugInAndInitFile = plugIns[i];
                UI.WriteLine("Loading {0} plug-in ...", plugInAndInitFile.Info.Name);
                PlugIn loadedPlugIn = Loader.Load<PlugIn>(plugInAndInitFile.Info);
                loadedPlugIn.Initialize(plugInAndInitFile.InitFile, this);
                loadedPlugIns[i] = loadedPlugIn;
                if (loadedPlugIn is PlugIns.I2PhaseInitialization)
                    plugInsWith2PhaseInit.Add((PlugIns.I2PhaseInitialization) loadedPlugIn);
                if (loadedPlugIn is PlugIns.ICleanUp)
                    plugInsWithCleanUp.Add((PlugIns.ICleanUp) loadedPlugIn);
            }
            return loadedPlugIns;
        }

        //---------------------------------------------------------------------

        private List<PlugIn> GetPlugInsToRun(PlugIn[] plugIns)
        {
            List<PlugIn> plugInsToRun = new List<PlugIn>();
            foreach (PlugIn plugIn in plugIns) {
                if (IsPlugInTimestep(plugIn))
                    plugInsToRun.Add(plugIn);
            }
            return plugInsToRun;
        }

        //---------------------------------------------------------------------

        private bool IsPlugInTimestep(PlugIn plugIn)
        {
            return (plugIn.Timestep > 0) && (timeSinceStart % plugIn.Timestep == 0);
        }

        //---------------------------------------------------------------------

        private void Run(PlugIn plugIn)
        {
            UI.WriteLine("Running {0} ...", plugIn.Name);
            plugIn.Run();
        }
    }
}
