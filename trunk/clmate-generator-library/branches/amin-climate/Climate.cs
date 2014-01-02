//  Copyright 2009-2010 Portland State University, Conservation Biology Institute
//  Authors:  Robert M. Scheller

using Landis.Core;
using System.Collections.Generic;
using System.IO;
using System;
//using climate_generator;
using System.Collections;
using System.Data.OleDb;
using System.Data;
using System.Linq;



namespace Landis.Library.Climate
{

    public class Climate
    {
        private static TemporalGranularity future_allData_granularity;
        private static TemporalGranularity spinup_allData_granularity;
        private static Dictionary<int, IClimateRecord[,]> future_allData;
        private static Dictionary<int, IClimateRecord[,]> spinup_allData;
        private static int[] randSelectedTimeSteps_future;
        private static int[] randSelectedTimeSteps_spinup;
        private static IClimateRecord[,] timestepData;
        private static ICore modelCore;
        private static bool flag;
        private static IInputParameters configParameters;
        internal static Dictionary<int, IClimateRecord[,]> avgEcoClimate_future_cache;
        internal static Dictionary<int, IClimateRecord[,]> avgEcoClimate_spinup_cache;

        private static System.Data.DataTable annualPDSI;
        private static double[] landscapAnnualPDSI;

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

        public static System.Data.DataTable AnnualPDSI
        {
            get
            {
                return annualPDSI;
            }
        }

        public static double[] LandscapAnnualPDSI
        {
            get
            {
                return landscapAnnualPDSI;
            }
            set
            {
                landscapAnnualPDSI = value;
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
        public static Dictionary<int, IClimateRecord[,]> AllData //This could not be renamed to Future_AllData because AllData is used in other extensions like century-succession
        {
            get
            {
                return future_allData;
            }
        }
        public static Dictionary<int, IClimateRecord[,]> Spinup_AllData
        {
            get
            {
                return spinup_allData;
            }
        }

        public static int[] RandSelectedTimeSteps_future { get { return randSelectedTimeSteps_future; } }
        public static int[] RandSelectedTimeSteps_spinup { get { return randSelectedTimeSteps_spinup; } }

        //---------------------------------------------------------------------
        public static IClimateRecord[,] TimestepData
        {
            get
            {
                return timestepData;
            }
            set
            {
                timestepData = value;
            }
        }
        //---------------------------------------------------------------------
        public static bool Flag
        {
            get
            {
                return flag;
            }
            set
            {
                flag = value;
            }
        }

        //------------------------------------------------------------------------
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



        public static void Write(IEcoregionDataset ecoregionDataset)
        {
            foreach (IEcoregion ecoregion in ecoregionDataset)
            {
                for (int i = 0; i < 12; i++)
                {
                    ModelCore.UI.WriteLine("Eco={0}, Month={1}, AvgMinTemp={2:0.0}, AvgMaxTemp={3:0.0}, StdDevTemp={4:0.0}, AvgPpt={5:0.0}, StdDevPpt={6:0.0}.",
                        ecoregion.Index, i + 1,
                        TimestepData[ecoregion.Index, i].AvgMinTemp,
                        TimestepData[ecoregion.Index, i].AvgMaxTemp,
                        TimestepData[ecoregion.Index, i].StdDevTemp,
                        TimestepData[ecoregion.Index, i].AvgPpt,
                        TimestepData[ecoregion.Index, i].StdDevPpt
                        );
                }
            }

        }
        //---------------------------------------------------------------------
        public static void Initialize(string climateConfigFilename, bool writeOutput, ICore mCore)
        {
            InputParametersParser inParamsParser = new InputParametersParser();
            //inParamsParser.Parse();
            configParameters = Landis.Data.Load<IInputParameters>(climateConfigFilename, inParamsParser);
            // call parser--- read climate-generator.txt
            //climate.ClimateFileFormat,...
            // call   Climate.Convert_FileFormat(climate.ClimateFileFormat,--)--- return string--> fill all data
            // same steps for spinup data

            modelCore = mCore;

            ModelCore.UI.WriteLine("   Loading weather data from file \"{0}\" ...", configParameters.ClimateFile);
            //Climate.Convert_FileFormat(parameters.ClimateFileFormat, parameters.ClimateFile), Climate.Convert_FileFormat(parameters.SpinUpClimateFileFormat, parameters.SpinUpClimateFile)
            ClimateParser parser = new ClimateParser();
            ClimateParser spinup_parser = new ClimateParser();
            //"Century_Climate_Inputs_Monthly.txt";//
            Climate.future_allData = new Dictionary<int, IClimateRecord[,]>();
            Climate.spinup_allData = new Dictionary<int, IClimateRecord[,]>();
            string convertedClimateFileName = Climate.ConvertFileFormat_FillOutAllData(configParameters.ClimateTimeSeries, configParameters.ClimateFile, configParameters.ClimateFileFormat, ClimatePhase.Future_Climate);
//            future_allData = Landis.Data.Load<Dictionary<int, IClimateRecord[,]>>(convertedClimateFileName, parser);
            //modelCore = mCore;
            if (configParameters.SpinUpClimateTimeSeries.ToLower() != "no")
            {
                ModelCore.UI.WriteLine("   Loading spin-up weather data from file \"{0}\" ...", configParameters.SpinUpClimateFile);
                //"Century_Climate_Inputs_PRISM_Monthly.txt";//
                string convertedSpinupClimateFileName = Climate.ConvertFileFormat_FillOutAllData(configParameters.SpinUpClimateTimeSeries, configParameters.SpinUpClimateFile, configParameters.SpinUpClimateFileFormat, ClimatePhase.SpinUp_Climate);
 //               spinup_allData = Landis.Data.Load<Dictionary<int, IClimateRecord[,]>>(convertedSpinupClimateFileName, spinup_parser);
            }

            if (Climate.ConfigParameters.ClimateTimeSeries.ToLower().Contains("random") || Climate.ConfigParameters.SpinUpClimateTimeSeries.ToLower().Contains("random"))
            {
                Climate.randSelectedTimeSteps_future = new int[Climate.future_allData.Count];//should be future_allData.Count or it needs to be different?
                for (int i = 0; i < Climate.future_allData.Count; i++)
                    Climate.randSelectedTimeSteps_future[i] = (int)Math.Round(Climate.ModelCore.GenerateUniform() * (Climate.future_allData.Count - 1));

                int maxSpeciesAge = modelCore.Species.Max(sp => sp.Longevity);
                //int maxSpeciesAge = 0;
                //foreach (ISpecies sp in ModelCore.Species)
                //{ 
                //    if(sp. maxSpeciesAge 
                //}

                Climate.randSelectedTimeSteps_spinup = new int[maxSpeciesAge]; //new int[Climate.spinup_allData.Count];
                for (int i = 0; i < maxSpeciesAge; i++)
                    Climate.randSelectedTimeSteps_spinup[i] = (int)Math.Round(Climate.ModelCore.GenerateUniform() * (Climate.spinup_allData.Count - 1));
            }

            // Have to ask
            //if (Climate. ClimatePhase.Future_Climate)
               // timestepData = Climate.future_allData.ElementAt(0).Value;
            //else if(ClimatePhase.SpinUp_Climate)
            //    timestepData = Climate.spinup_allData.ElementAt(0).Value;

            //timestepData = future_allData.ElementAt(0).Value;
                // timestepData = future_allData.ElementAt(0).Value; //time step zero!
            
            //timestepData = allData[1];
            //TimestepData[1,11].AvgMinTemp,  //should get ecoregion (index=1), month 11, time step 1

            if (writeOutput)
                Write(Climate.ModelCore.Ecoregions);
        }

        public static void GenerateClimate_GetPDSI(int startYear, int endYear, int latitude, double fieldCapacity, double wiltingPoint)
        {
            string outputFilePath = @"PDSI_BaseBDA_Genrated_Climate.csv";
            File.WriteAllText(outputFilePath, String.Empty);

            foreach (IEcoregion ecoregion in Climate.ModelCore.Ecoregions)
            {
                AnnualClimate_Monthly[] acs;
                int numOfYears = endYear - startYear + 1;
                acs = new AnnualClimate_Monthly[numOfYears];

                //foreach time step it should be called

                for (int i = startYear; i <= endYear; i++)
                {
                    acs[i - startYear] = new AnnualClimate_Monthly(ecoregion, 0, latitude); // Latitude should be given
                    //Console.WriteLine(ac.MonthlyTemp[0].ToString() + "\n");
                    //Console.WriteLine(ac.MonthlyPrecip[0].ToString() + "\n");
                }



                double[] mon_T_normal = new double[12];//new double[12] { 19.693, 23.849, 34.988, 49.082, 60.467, 70.074, 75.505, 73.478, 64.484, 52.634, 36.201, 24.267 };
                IClimateRecord[] climateRecs = new ClimateRecord[12];

                //If timestep is 0 then calculate otherwise get the mon_T_normal for timestep 0

                Climate.TimestepData = future_allData[0];
                for (int mo = 0; mo < 12; mo++)
                {
                    climateRecs[mo] = Climate.TimestepData[ecoregion.Index, mo];

                    mon_T_normal[mo] = (climateRecs[mo].AvgMinTemp + climateRecs[mo].AvgMinTemp) / 2;
                }
                double AWC = fieldCapacity - wiltingPoint;
                //double latitude = Landis.Extension.Succession.Century.EcoregionData.Latitude[ecoregion];
                new PDSI_Calculator().CalculatePDSI(acs, mon_T_normal, AWC, latitude, outputFilePath, UnitSystem.metrics);
            }
        }

        public static void GetPDSI(int startYear, int latitude, double fieldCapacity, double wiltingPoint, ClimatePhase climatePhase = ClimatePhase.Future_Climate)
        {
            Climate.Flag = false;
            string outputFilePath = "";
            if (File.Exists("bda\\PDSI_BaseBDA.csv")) //("C:\\Program Files\\LANDIS-II\\v6\\examples\\base-BDA_1\\bda\\PDSI_BaseBDA.csv"))
                outputFilePath = @"bda\PDSI_BaseBDA.csv";// @"C:\Program Files\LANDIS-II\v6\examples\base-BDA_1\bda\PDSI_BaseBDA.csv";
            else
            {
                File.Create("bda\\PDSI_BaseBDA.csv");//("C:\\Program Files\\LANDIS-II\\v6\\examples\\base-BDA_1\\bda\\PDSI_BaseBDA.csv");
                outputFilePath = @"bda\PDSI_BaseBDA.csv"; //@"C:\Program Files\LANDIS-II\v6\examples\base-BDA_1\bda\PDSI_BaseBDA.csv";
            }
            File.WriteAllText(outputFilePath, String.Empty);
            Climate.annualPDSI = new System.Data.DataTable();//final list of annual PDSI values
            foreach (IEcoregion ecoregion in Climate.ModelCore.Ecoregions)
            {
                if (ecoregion.Active)
                {
                    if (true)//(ecoregion.Index == 0)
                    {
                        AnnualClimate_Monthly[] acs;
                        int numOfYears = future_allData.Count - 1; //-1 is because we dont want the timestep 0
                        acs = new AnnualClimate_Monthly[numOfYears];
                        int timestepIndex = 0;

                        double[] mon_T_normal = new double[12];//new double[12] { 19.693, 23.849, 34.988, 49.082, 60.467, 70.074, 75.505, 73.478, 64.484, 52.634, 36.201, 24.267 };
                        IClimateRecord[] climateRecs = new ClimateRecord[12];

                        //If timestep is 0 then calculate otherwise get the mon_T_normal for timestep 0

                        Climate.TimestepData = future_allData[0];
                        for (int mo = 0; mo < 12; mo++)
                        {
                            climateRecs[mo] = Climate.TimestepData[ecoregion.Index, mo];
                            mon_T_normal[mo] = (climateRecs[mo].AvgMinTemp + climateRecs[mo].AvgMinTemp) / 2;
                        }

                        foreach (KeyValuePair<int, IClimateRecord[,]> timeStep in future_allData)
                        {
                            if (timeStep.Key != 0)
                            {
                                acs[timestepIndex] = new AnnualClimate_Monthly(ecoregion, startYear + timeStep.Key, latitude, climatePhase, timeStep.Key); // Latitude should be given
                                timestepIndex++;
                            }
                        }
                        //double AWC = Landis.Extension.Succession.Century.EcoregionData.FieldCapacity[ecoregion] - Landis.Extension.Succession.Century.EcoregionData.WiltingPoint[ecoregion];
                        double AWC = fieldCapacity - wiltingPoint;
                        //double latitude = Landis.Extension.Succession.Century.EcoregionData.Latitude[ecoregion];
                        new PDSI_Calculator().CalculatePDSI(acs, mon_T_normal, AWC, latitude, outputFilePath, UnitSystem.metrics);
                    }
                }
            }



            int numberOftimeStaps = 0;
            int numberOfEcoregions = 0;

            int index = 0;
            double ecoAverage = 0;

            //List<int> levels = Climate.AnnualPDSI.AsEnumerable().Select(al => al.Field<int>("TimeStep")).Distinct().ToList().Max();
            //numberOftimeStaps = levels.Max();
            numberOftimeStaps = Climate.AnnualPDSI.AsEnumerable().Select(al => al.Field<int>("TimeStep")).Distinct().ToList().Max();
            //numberOftimeStaps = Climate.allData.Count;
            //List<int> ecos = Climate.AnnualPDSI.AsEnumerable().Select(a2 => a2.Field<int>("Ecorigion")).Distinct().ToList().Max();
            //numberOfEcoregions = ecos.Max();
            numberOfEcoregions = Climate.AnnualPDSI.AsEnumerable().Select(a2 => a2.Field<int>("Ecorigion")).Distinct().ToList().Max();
            //numberOfEcoregions = Climate.ModelCore.Ecoregions.Count;
            Climate.LandscapAnnualPDSI = new double[numberOftimeStaps];

            for (int timeStep = 1; timeStep <= numberOftimeStaps; timeStep++)
            {
                index = timeStep;
                for (int eco = 1; eco <= numberOfEcoregions; eco++)
                {
                    if (index <= Climate.AnnualPDSI.Rows.Count && (Int32)Climate.AnnualPDSI.Rows[index - 1][0] == timeStep && (Int32)Climate.AnnualPDSI.Rows[index - 1][1] == eco)
                    {
                        ecoAverage += (double)Climate.AnnualPDSI.Rows[index - 1][2];// get the valuse of annualPDSI

                        if (eco == numberOfEcoregions)
                        {
                            ecoAverage = ecoAverage / numberOfEcoregions;
                            Climate.LandscapAnnualPDSI[timeStep - 1] = ecoAverage;
                            //Can be printed
                            //file.WriteLine(timeStep + ", " + ecoAverage) ;

                            ecoAverage = 0;
                        }
                    }
                    index = index + numberOftimeStaps;
                }
            }

        }

        public static void GetPDSI_Test()
        {
            IEcoregion ecoregion = Climate.ModelCore.Ecoregions[0];
            //here:
            string outputFilePath = @"C:\Program Files\LANDIS-II\v6\examples\base-BDA_1\PDSI_BaseBDA_Test2.csv";
            File.WriteAllText(outputFilePath, String.Empty);
            int startYear = 1893, endYear = 1897;
            AnnualClimate_Monthly[] acs;
            if (endYear > startYear)
            {
                int numOfYears = endYear - startYear + 1;
                acs = new AnnualClimate_Monthly[numOfYears];

                double[] mon_T_normal = new double[12] { 19.693, 23.849, 34.988, 49.082, 60.467, 70.074, 75.505, 73.478, 64.484, 52.634, 36.201, 24.267 };
                IClimateRecord[] climateRecs = new ClimateRecord[12];

                //Climate.TimestepData = allData[0];
                //for (int mo = 0; mo < 12; mo++)
                //{
                //    climateRecs[mo] = Climate.TimestepData[ecoregion.Index, mo];
                //    //mon_T_normal[mo] = (climateRecs[mo].AvgMinTemp + climateRecs[mo].AvgMinTemp) / 2;
                //}

                acs[0] = new AnnualClimate_Monthly(ecoregion, 1893, 0);
                ((AnnualClimate_Monthly)acs[0]).MonthlyTemp = new double[] { 14.371, 14.000, 26.435, 44.250, 54.645, 70.683, 73.355, 69.323, 63.600, 48.806, 32.867, 19.161 };
                //acs[0].MonthlyPrecip = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                ((AnnualClimate_Monthly)acs[0]).MonthlyPrecip = new double[] { 0.610, 1.500, 1.730, 4.050, 1.950, 0.790, 3.020, 2.570, 1.430, 0.850, 1.260, 2.350 };

                acs[1] = new AnnualClimate_Monthly(ecoregion, 1894, 0);
                ((AnnualClimate_Monthly)acs[1]).MonthlyTemp = new double[] { 12.705, 14.979, 37.984, 49.700, 61.209, 71.463, 77.935, 74.312, 65.283, 51.516, 34.767, 29.548 };
                ((AnnualClimate_Monthly)acs[1]).MonthlyPrecip = new double[] { 0.700, 0.550, 0.580, 4.240, 2.430, 1.150, 0.580, 1.480, 0.550, 1.760, 0.050, 1.000 };

                acs[2] = new AnnualClimate_Monthly(ecoregion, 1895, 0);
                ((AnnualClimate_Monthly)acs[2]).MonthlyTemp = new double[] { 12.519, 17.964, 33.994, 54.506, 60.411, 66.172, 70.548, 69.622, 65.288, 44.795, 32.433, 23.333 };
                ((AnnualClimate_Monthly)acs[2]).MonthlyPrecip = new double[] { 0.650, 0.540, 0.520, 3.980, 2.380, 6.240, 2.320, 3.920, 4.770, 0.060, 1.040, 0.000 };

                acs[3] = new AnnualClimate_Monthly(ecoregion, 1896, 0);
                ((AnnualClimate_Monthly)acs[3]).MonthlyTemp = new double[] { 23.258, 27.397, 26.425, 48.833, 62.790, 68.054, 71.365, 70.677, 57.991, 46.355, 21.154, 28.597 };
                ((AnnualClimate_Monthly)acs[3]).MonthlyPrecip = new double[] { 0.250, 0.270, 1.670, 5.680, 6.240, 7.740, 5.550, 1.660, 1.810, 3.230, 3.850, 0.230 };

                acs[4] = new AnnualClimate_Monthly(ecoregion, 1897, 0);
                ((AnnualClimate_Monthly)acs[4]).MonthlyTemp = new double[] { 13.758, 20.179, 26.613, 46.700, 59.016, 66.533, 74.032, 67.928, 71.617, 54.613, 32.450, 18.686 };
                ((AnnualClimate_Monthly)acs[4]).MonthlyPrecip = new double[] { 2.500, 0.540, 3.010, 4.480, 0.980, 5.820, 3.780, 1.600, 1.010, 1.940, 0.910, 2.950 };



                //for (int i = startYear; i <= endYear; i++)
                //{
                //    acs[i - startYear] = new AnnualClimate(ecoregion, i, 0); // Latitude should be given
                //    //Console.WriteLine(ac.MonthlyTemp[0].ToString() + "\n");
                //    //Console.WriteLine(ac.MonthlyPrecip[0].ToString() + "\n");
                //}



                //for (int mo = 0; mo < 12; mo++)
                //{
                //    climateRecs[mo] = Climate.TimestepData[ecoregion.Index, mo];
                //    mon_T_normal[mo] = (climateRecs[mo].AvgMinTemp + climateRecs[mo].AvgMinTemp) / 2;
                //}

                double AWC = 0.3;//Landis.Extension.Succession.Century.EcoregionData.FieldCapacity[ecoregion] - Landis.Extension.Succession.Century.EcoregionData.WiltingPoint[ecoregion];
                double latitude = 42.60;//Landis.Extension.Succession.Century.EcoregionData.Latitude[ecoregion];
                new PDSI_Calculator().CalculatePDSI(acs, mon_T_normal, AWC, latitude, outputFilePath, UnitSystem.USCustomaryUnits);

            }




        }


        /// <summary>
        /// Converts USGS Data to Standard Input climate Data and fill out the Future_AllData and/or Spinup_AllData
        /// </summary>
        /// 
        public static string ConvertFileFormat_FillOutAllData(String timeSeries, string File, string fileFormat, ClimatePhase climatePhase)
        {
            if (climatePhase == ClimatePhase.Future_Climate && timeSeries.Contains("Daily"))
                future_allData_granularity = TemporalGranularity.Daily;
                
            else if (climatePhase == ClimatePhase.Future_Climate && timeSeries.Contains("Monthly"))
                future_allData_granularity = TemporalGranularity.Monthly;

                spinup_allData_granularity = TemporalGranularity.Monthly;

            string readableFile = "";
            if (timeSeries.Contains("MonthlyStandard"))
                return File;

            else if (timeSeries.Contains("Average") || timeSeries.Contains("Random"))
            {
                if (timeSeries.Contains("Daily"))
                    return readableFile = Landis.Library.Climate.ClimateDataConvertor.Convert_USGS_to_ClimateData_FillAlldata(TemporalGranularity.Daily, File, fileFormat, climatePhase);
                else if (timeSeries.Contains("Monthly"))
                    return readableFile = Landis.Library.Climate.ClimateDataConvertor.Convert_USGS_to_ClimateData_FillAlldata(TemporalGranularity.Monthly, File, fileFormat, climatePhase);

            }
            
            else if (timeSeries.Contains("MonthlyAverage"))//AverageMonthly
            {
                return readableFile = Landis.Library.Climate.ClimateDataConvertor.Convert_USGS_to_ClimateData_FillAlldata(TemporalGranularity.Monthly, File, fileFormat, climatePhase);
            }
            
            //else if (timeSeries.Contains("Random"))
            //{
            //    if (timeSeries.Contains("Daily"))
            //        return readableFile = Landis.Library.Climate.ClimateDataConvertor.Convert_USGS_to_ClimateData_FillAlldata(TemporalGranularity.Daily, File, fileFormat, climatePhase);
            //    else if (timeSeries.Contains("Monthly"))
            //        return readableFile = Landis.Library.Climate.ClimateDataConvertor.Convert_USGS_to_ClimateData_FillAlldata(TemporalGranularity.Monthly, File, fileFormat, climatePhase);
            //}
            
            else if (timeSeries.Contains("DailyGCM"))
            {
                return readableFile = Landis.Library.Climate.ClimateDataConvertor.Convert_USGS_to_ClimateData_FillAlldata(TemporalGranularity.Daily, File, fileFormat, climatePhase);
            }
            
            else
            {
                ModelCore.UI.WriteLine("Error in converting input-climate-file format: invalid ClimateTimeSeries value provided in cliamte-generator input file.");
                throw new Exception("Error in converting input-climate-file format: invalid ClimateTimeSeries value provided in cliamte-generator input file.");
            }
            return readableFile;

        }



    }

}









