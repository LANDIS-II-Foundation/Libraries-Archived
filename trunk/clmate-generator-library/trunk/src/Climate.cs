//  Copyright: Portland State University 2009-2014
//  Authors:  Robert M. Scheller, John McNabb, Amin Almassian

using Landis.Core;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections;
using Landis.Library.Metadata;
using System.Linq;


namespace Landis.Library.Climate
{

    public class Climate
    {

        private static TemporalGranularity future_allData_granularity;
        private static TemporalGranularity spinup_allData_granularity;
        private static Dictionary<int, ClimateRecord[][]> future_allData;
        private static Dictionary<int, ClimateRecord[][]> spinup_allData;
        private static List<int> randSelectedTimeKeys_future;
        private static List<int> randSelectedTimeKeys_spinup;
        private static ICore modelCore;
        private static IInputParameters configParameters;

        //private static System.Data.DataTable annualPDSI;
        private static double[,] annualPDSI;
        private static double[] landscapeAnnualPDSI;

        public static MetadataTable<PDSI_Log> PdsiLog;
        public static MetadataTable<InputLog> SpinupInputLog;
        public static MetadataTable<InputLog> FutureInputLog;
        public static MetadataTable<AnnualLog> AnnualLog;

        public enum Phase {SpinUp_Climate = 0, Future_Climate = 1 }

        // Rob testing storing all monthly and daily data during spinup, to avoid new data creation.
        public static Dictionary<int, AnnualClimate_Daily[] > Future_DailyData;  //dict key = year; climate record = ecoreregion, day
        public static Dictionary<int, AnnualClimate_Monthly[]> Future_MonthlyData;  //dict key = year; climate record = ecoreregion, month
        public static Dictionary<int, AnnualClimate_Daily[]> Spinup_DailyData;  //dict key = year; climate record = ecoreregion, day
        public static Dictionary<int, AnnualClimate_Monthly[]> Spinup_MonthlyData;  //dict key = year; climate record = ecoreregion, month

        public Climate()
        {
        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }

        public static double[,] AnnualPDSI  //ecoregion.Index, Year
        {
            get
            {
                return annualPDSI;
            }
            set
            {
                annualPDSI = value;
            }
        }

        public static double[] LandscapeAnnualPDSI //year
        {
            get
            {
                return landscapeAnnualPDSI;
            }
            set
            {
                landscapeAnnualPDSI = value;
            }

        }

        public static TemporalGranularity AllData_granularity
        {
            get
            {
                return future_allData_granularity;
            }
        }
        public static TemporalGranularity Spinup_allData_granularity
        {
            get
            {
                return spinup_allData_granularity;
            }
        }
        public static Dictionary<int, ClimateRecord[][]> Future_AllData 
        {
            get
            {
                return future_allData;
            }
        }
        public static Dictionary<int, ClimateRecord[][]> Spinup_AllData
        {
            get
            {
                return spinup_allData;
            }
        }

        public static List<int> RandSelectedTimeKeys_future { get { return randSelectedTimeKeys_future; } }
        public static List<int> RandSelectedTimeKeys_spinup { get { return randSelectedTimeKeys_spinup; } }

       
        public static IInputParameters ConfigParameters
        {
            get
            {
                return configParameters;
            }
            //set
            //{
            //    configParameters = value;
            //}
        }



        //---------------------------------------------------------------------
        public static void Initialize(string climateConfigFilename, bool writeOutput, ICore mCore)
        {
            InputParametersParser inParamsParser = new InputParametersParser();
            configParameters = Landis.Data.Load<IInputParameters>(climateConfigFilename, inParamsParser);

            modelCore = mCore;
            MetadataHandler.InitializeMetadata(1, modelCore);

            ModelCore.UI.WriteLine("   Loading weather data ...");
            Climate.future_allData = new Dictionary<int, ClimateRecord[][]>();
            Climate.spinup_allData = new Dictionary<int, ClimateRecord[][]>();

            Future_MonthlyData = new Dictionary<int, AnnualClimate_Monthly[]>();
            Spinup_MonthlyData = new Dictionary<int, AnnualClimate_Monthly[]>();
            Future_DailyData = new Dictionary<int, AnnualClimate_Daily[]>();
            Spinup_DailyData = new Dictionary<int, AnnualClimate_Daily[]>();
            LandscapeAnnualPDSI = new double[Climate.ModelCore.EndTime - Climate.ModelCore.StartTime + 1];

            ModelCore.UI.WriteLine("   Loading spin-up weather data from file {0} ...", configParameters.SpinUpClimateFile);
            Climate.ConvertFileFormat_FillOutAllData(configParameters.SpinUpClimateTimeSeries, configParameters.SpinUpClimateFile, configParameters.SpinUpClimateFileFormat, Climate.Phase.SpinUp_Climate);

            ModelCore.UI.WriteLine("   Loading future weather data from file {0} ...", configParameters.ClimateFile);
            Climate.ConvertFileFormat_FillOutAllData(configParameters.ClimateTimeSeries, configParameters.ClimateFile, configParameters.ClimateFileFormat, Climate.Phase.Future_Climate);
            
            
            // for all options except random, the spinupTimeStepKeys are those from "allData"
            List<int> spinupTimeStepKeys = new List<int>(Climate.spinup_allData.Keys);

            if (Climate.ConfigParameters.SpinUpClimateTimeSeries.ToLower().Contains("random"))
            {
                // generate random keys up to the maximum possible length of spinup
                
                int maxSpeciesAge = 0;
                foreach (ISpecies sp in ModelCore.Species)
                {
                    if (sp.Longevity > maxSpeciesAge)
                        maxSpeciesAge = sp.Longevity;
                }

                // pick a random year key from allData and make spinupTimeStepKeys up to the maximum possible length of spinup
                List<int> keyList = new List<int>(Climate.spinup_allData.Keys);
                var startYear = keyList.Min();
                Climate.randSelectedTimeKeys_spinup = new List<int>();
                spinupTimeStepKeys.Clear();

                for (var i = 0; i < maxSpeciesAge; ++i)
                {
                    Climate.randSelectedTimeKeys_spinup.Add(keyList[(int)(keyList.Count * Climate.ModelCore.GenerateUniform())]);
                    spinupTimeStepKeys.Add(startYear + i);
                }

                
            }

            // write input data to the log
            foreach (KeyValuePair<int, ClimateRecord[][]> timeStep in spinup_allData)
            {
                Climate.WriteSpinupInputLog(timeStep.Value, timeStep.Key, Climate.Phase.SpinUp_Climate.ToString());
            }

            // initialize Spinup data arrays
            foreach (var timeStepKey in spinupTimeStepKeys)
            {
                Spinup_MonthlyData.Add(timeStepKey, new AnnualClimate_Monthly[modelCore.Ecoregions.Count]);
                Spinup_DailyData.Add(timeStepKey, new AnnualClimate_Daily[modelCore.Ecoregions.Count]);
            }


            // **
            // future

            // for all options except random, the futureTimeStepKeys are those from "allData"
            List<int> futureTimeStepKeys = new List<int>(Climate.future_allData.Keys);

            if (Climate.ConfigParameters.ClimateTimeSeries.ToLower().Contains("random")) 
            {
                // generate random keys for the length of the simulation
                var yearCount = ModelCore.EndTime - ModelCore.StartTime;

                // pick a random year key from allData and make futureTimeStepKeys up to the length of the simulation
                List<int> keyList = new List<int>(Climate.future_allData.Keys);
                var startYear = keyList.Min();
                Climate.randSelectedTimeKeys_future = new List<int>();
                futureTimeStepKeys.Clear();
                
                for (var i = 0; i < yearCount; ++i)
                {
                    Climate.randSelectedTimeKeys_future.Add(keyList[(int)(keyList.Count * Climate.ModelCore.GenerateUniform())]);
                    futureTimeStepKeys.Add(startYear + i);
                }

                                
            }

            // write input data to the log
            foreach (KeyValuePair<int, ClimateRecord[][]> timeStep in future_allData)
            {
                Climate.WriteFutureInputLog(timeStep.Value, timeStep.Key, Climate.Phase.Future_Climate.ToString());
            }

            // initialize Future data arrays
            foreach (var timeStepKey in futureTimeStepKeys)
            {
                Future_MonthlyData.Add(timeStepKey, new AnnualClimate_Monthly[modelCore.Ecoregions.Count]);
                Future_DailyData.Add(timeStepKey, new AnnualClimate_Daily[modelCore.Ecoregions.Count]);
            }


        }


        public static void GenerateEcoregionClimateData(IEcoregion ecoregion, int startYear, double latitude, double fieldCapacity, double wiltingPoint)
        {
                                    
            // JM:  these next three lines are not currently used, but may need to be modified if used:
            //int numberOftimeSteps = Climate.ModelCore.EndTime - Climate.ModelCore.StartTime;
            //annualPDSI = new double[Climate.ModelCore.Ecoregions.Count, future_allData.Count]; //numberOftimeSteps + 1];
            //landscapeAnnualPDSI = new double[future_allData.Count]; //numberOftimeSteps+1];
            double[] temperature_normals = new double[12];
            
            double availableWaterCapacity = fieldCapacity - wiltingPoint;

            Climate.ModelCore.UI.WriteLine("   Core.StartTime = {0}, Core.EndTime = {1}.", ModelCore.StartTime, ModelCore.EndTime);
            Climate.ModelCore.UI.WriteLine("   Climate.LandscapeAnnualPDSI.Length = {0}.", Climate.LandscapeAnnualPDSI.Length);

            //First Calculate Climate Normals from Spin-up data
            int timeStepIndex = 0;
            foreach (KeyValuePair<int, AnnualClimate_Monthly[]> timeStep in Spinup_MonthlyData)
            {

                //Climate.ModelCore.UI.WriteLine("  Calculating Weather for SPINUP: timeStep = {0}, actualYear = {1}", timeStep.Key, startYear + timeStep.Key);
                AnnualClimate_Monthly annualClimateMonthly = new AnnualClimate_Monthly(ecoregion, latitude, Climate.Phase.SpinUp_Climate, timeStep.Key, timeStepIndex); 
                Spinup_MonthlyData[startYear + timeStep.Key][ecoregion.Index] = annualClimateMonthly;

                for (int mo = 0; mo < 12; mo++)
                {
                    temperature_normals[mo] += annualClimateMonthly.MonthlyTemp[mo]; 
                }

                timeStepIndex++;
            }

            // Calculate AVERAGE T normal.
            for (int mo = 0; mo < 12; mo++)
            {
                temperature_normals[mo] /= (double)Spinup_MonthlyData.Count;
                //Climate.ModelCore.UI.WriteLine("Month = {0}, Original Monthly T normal = {1}", mo, month_Temp_normal[mo]);

            }
            
            timeStepIndex = 0;

            foreach (KeyValuePair<int, AnnualClimate_Monthly[]> timeStep in Future_MonthlyData)
            {
                //Climate.ModelCore.UI.WriteLine("  Completed calculations for Future_Climate: TimeStepYear = {0}, actualYear = {1}", timeStep.Key, startYear + timeStep.Key);
                AnnualClimate_Monthly annualClimateMonthly = new AnnualClimate_Monthly(ecoregion, latitude, Climate.Phase.Future_Climate, timeStep.Key, timeStepIndex);
                Future_MonthlyData[startYear + timeStep.Key][ecoregion.Index] = annualClimateMonthly;

                // Next calculate PSDI for the future data
                Future_MonthlyData[startYear + timeStep.Key][ecoregion.Index].PDSI = PDSI_Calculator.CalculatePDSI(annualClimateMonthly, temperature_normals, availableWaterCapacity, latitude, UnitSystem.metrics, ecoregion);
                Climate.LandscapeAnnualPDSI[timeStepIndex] += (Future_MonthlyData[startYear + timeStep.Key][ecoregion.Index].PDSI / Climate.ModelCore.Ecoregions.Count);

                //Climate.ModelCore.UI.WriteLine("Calculated PDSI for Ecoregion {0}, timestep {1}, PDSI Year {2}; PDSI={3:0.00}.", ecoregion.Name, timestepIndex, timeStep.Key, PDSI);
                timeStepIndex++;

                WriteAnnualLog(ecoregion, startYear + timeStep.Key, annualClimateMonthly);
            }


        }



        /// <summary>
        /// Converts USGS Data to Standard Input climate Data and fill out the Future_AllData and/or Spinup_AllData
        /// </summary>
        /// 
        public static void ConvertFileFormat_FillOutAllData(String timeSeries, string filePath, string fileFormat, Climate.Phase climatePhase)
        {
            if (climatePhase == Climate.Phase.Future_Climate && timeSeries.Contains("Daily"))
                future_allData_granularity = TemporalGranularity.Daily;
                
            else if (climatePhase == Climate.Phase.Future_Climate && timeSeries.Contains("Monthly"))
                future_allData_granularity = TemporalGranularity.Monthly;

            else if (climatePhase == Climate.Phase.SpinUp_Climate && timeSeries.Contains("Daily"))
                spinup_allData_granularity = TemporalGranularity.Daily;

            else if (climatePhase == Climate.Phase.SpinUp_Climate && timeSeries.Contains("Monthly"))
                spinup_allData_granularity = TemporalGranularity.Monthly;

            if (timeSeries.Contains("Daily"))
                ClimateDataConvertor.Convert_USGS_to_ClimateData_FillAlldata(TemporalGranularity.Daily, filePath, fileFormat, climatePhase);
            
            else if (timeSeries.Contains("Monthly"))
                ClimateDataConvertor.Convert_USGS_to_ClimateData_FillAlldata(TemporalGranularity.Monthly, filePath, fileFormat, climatePhase);

            return;

        }
        //---------------------------------------------------------------------
        private static void WriteSpinupInputLog(ClimateRecord[][] TimestepData, int year, string period)
        {
            //spinup_allData.
            foreach (IEcoregion ecoregion in Climate.ModelCore.Ecoregions)
            {
                if (ecoregion.Active)
                {
                    for (int month = 0; month < 12; month++)
                    {
                        SpinupInputLog.Clear();
                        InputLog sil = new InputLog();

                        sil.SimulationPeriod = period;
                        sil.Time = year;
                        sil.Month = month + 1;
                        sil.EcoregionName = ecoregion.Name;
                        sil.EcoregionIndex = ecoregion.Index;
                        sil.min_airtemp = TimestepData[ecoregion.Index][month].AvgMinTemp;
                        sil.max_airtemp = TimestepData[ecoregion.Index][month].AvgMaxTemp;
                        sil.std_temp = TimestepData[ecoregion.Index][month].StdDevTemp;
                        sil.ppt = TimestepData[ecoregion.Index][month].AvgPpt;
                        sil.std_ppt = TimestepData[ecoregion.Index][month].StdDevPpt;

                        SpinupInputLog.AddObject(sil);
                        SpinupInputLog.WriteToFile();

                    }
                }
            }

        }

        //---------------------------------------------------------------------
        private static void WriteFutureInputLog(ClimateRecord[][] TimestepData, int year, string period)
        {
            //spinup_allData.
            foreach (IEcoregion ecoregion in Climate.ModelCore.Ecoregions)
            {
                if (ecoregion.Active)
                {
                    for (int month = 0; month < 12; month++)
                    {
                        FutureInputLog.Clear();
                        InputLog fil = new InputLog();

                        fil.SimulationPeriod = period;
                        fil.Time = year;
                        fil.Month = month + 1;
                        fil.EcoregionName = ecoregion.Name;
                        fil.EcoregionIndex = ecoregion.Index;
                        fil.min_airtemp = TimestepData[ecoregion.Index][month].AvgMinTemp;
                        fil.max_airtemp = TimestepData[ecoregion.Index][month].AvgMaxTemp;
                        fil.std_temp = TimestepData[ecoregion.Index][month].StdDevTemp;
                        fil.ppt = TimestepData[ecoregion.Index][month].AvgPpt;
                        fil.std_ppt = TimestepData[ecoregion.Index][month].StdDevPpt;

                        FutureInputLog.AddObject(fil);
                        FutureInputLog.WriteToFile();

                    }
                }
            }

        }

        //---------------------------------------------------------------------
        private static void WriteAnnualLog(IEcoregion ecoregion, int year, AnnualClimate_Monthly annualClimateMonthly)
        {
            AnnualLog.Clear();
            AnnualLog al = new AnnualLog();

            al.Time = year;
            al.EcoregionName = ecoregion.Name;
            al.EcoregionIndex = ecoregion.Index;
            al.BeginGrow = annualClimateMonthly.BeginGrowing;
            al.EndGrow = annualClimateMonthly.EndGrowing;
            //al.MAP = TBD;
            //al.MAT = TBD;

            AnnualLog.AddObject(al);
            AnnualLog.WriteToFile();


        }


    }

}









