//  Copyright 2009-2010 Portland State University, Conservation Biology Institute
//  Authors:  Robert M. Scheller

using Landis.Core;
using System.Collections.Generic;
using System.IO;
using System;

namespace Landis.Library.Climate
{

    public class Climate
    {
        private static Dictionary<int, IClimateRecord[,]> allData;
        private static IClimateRecord[,] timestepData;
        private static ICore modelCore;
        
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

        public static Dictionary<int, IClimateRecord[,]> AllData
        {
            get {
                return allData;
            }
        }
        //---------------------------------------------------------------------
        public static IClimateRecord[,] TimestepData
        {
            get {
                return timestepData;
            }
            set {
                timestepData = value;
            }
        }
        
        public static void Write(IEcoregionDataset ecoregionDataset) 
        {
            foreach(IEcoregion ecoregion in ecoregionDataset)
            {
                for(int i = 0; i < 12; i++)
                {
                    ModelCore.Log.WriteLine("Eco={0}, Month={1}, AvgMinTemp={2:0.0}, AvgMaxTemp={3:0.0}, StdDevTemp={4:0.0}, AvgPpt={5:0.0}, StdDevPpt={6:0.0}.",
                        ecoregion.Index, i+1, 
                        TimestepData[ecoregion.Index,i].AvgMinTemp,
                        TimestepData[ecoregion.Index,i].AvgMaxTemp,
                        TimestepData[ecoregion.Index,i].StdDevTemp,
                        TimestepData[ecoregion.Index,i].AvgPpt,
                        TimestepData[ecoregion.Index,i].StdDevPpt
                        );
                }
            }
            
        }
        //---------------------------------------------------------------------
        public static void Initialize(string filename, bool writeOutput, ICore mCore) 
        {
            modelCore = mCore;
            ModelCore.Log.WriteLine("   Loading weather data from file \"{0}\" ...", filename);
            ClimateParser parser = new ClimateParser();
            allData = ModelCore.Load<Dictionary<int, IClimateRecord[,]>>(filename, parser);
            modelCore = mCore;
            
            timestepData = allData[0]; //time step zero!
            
            //timestepData = allData[1];
            //TimestepData[1,11].AvgMinTemp,  //should get ecoregion (index=1), month 11, time step 1
            
            if(writeOutput)
                Write(Climate.ModelCore.Ecoregions);

        }

        public static void GenerateClimate_GetPDSI( int startYear, int endYear)
        {
            string outputFilePath = @"PDSI_BaseBDA_Genrated_Climate.csv";
            File.WriteAllText(outputFilePath, String.Empty);

            foreach (IEcoregion ecoregion in Climate.ModelCore.Ecoregions)
            {
                AnnualClimate[] acs;
                int numOfYears = endYear - startYear + 1;
                acs = new AnnualClimate[numOfYears];

                //foreach time step it should be called
                
                for (int i = startYear; i <= endYear; i++)
                {
                    acs[i - startYear] = new AnnualClimate(ecoregion , 0 , i, Landis.Extension.Succession.Century.EcoregionData.Latitude[ecoregion]); // Latitude should be given
                    //Console.WriteLine(ac.MonthlyTemp[0].ToString() + "\n");
                    //Console.WriteLine(ac.MonthlyPrecip[0].ToString() + "\n");
                }
            

                double[] mon_T_normal = new double[12];//new double[12] { 19.693, 23.849, 34.988, 49.082, 60.467, 70.074, 75.505, 73.478, 64.484, 52.634, 36.201, 24.267 };
                IClimateRecord[] climateRecs = new ClimateRecord[12];

                //If timestep is 0 then calculate otherwise get the mon_T_normal for timestep 0
            
                Climate.TimestepData = allData[0];
                for (int mo = 0; mo < 12; mo++)
                {
                    climateRecs[mo] = Climate.TimestepData[ecoregion.Index, mo];

                    mon_T_normal[mo] = (climateRecs[mo].AvgMinTemp + climateRecs[mo].AvgMinTemp) / 2;
                }
                double AWC = Landis.Extension.Succession.Century.EcoregionData.FieldCapacity[ecoregion] - Landis.Extension.Succession.Century.EcoregionData.WiltingPoint[ecoregion];
                double latitude = Landis.Extension.Succession.Century.EcoregionData.Latitude[ecoregion];
                new PDSI_Calculator().CalculatePDSI(acs, mon_T_normal, AWC, latitude, outputFilePath, UnitSystem.metrics);
            }
        }

        public static void GetPDSI(int startYear)
        {
           string outputFilePath = @"C:\Program Files\LANDIS-II\v6\examples\base-BDA_1\PDSI_BaseBDA.csv";
           File.WriteAllText(outputFilePath, String.Empty);
           foreach (IEcoregion ecoregion in Climate.ModelCore.Ecoregions)
           {
                if (true)//(ecoregion.Index == 0)
                {
                    AnnualClimate[] acs;
                    int numOfYears = allData.Count - 1; //-1 is because we dont want the timestep 0
                    acs = new AnnualClimate[numOfYears];
                    int timestepIndex = 0;

                    double[] mon_T_normal = new double[12];//new double[12] { 19.693, 23.849, 34.988, 49.082, 60.467, 70.074, 75.505, 73.478, 64.484, 52.634, 36.201, 24.267 };
                    IClimateRecord[] climateRecs = new ClimateRecord[12];

                    //If timestep is 0 then calculate otherwise get the mon_T_normal for timestep 0

                    Climate.TimestepData = allData[0];
                    for (int mo = 0; mo < 12; mo++)
                    {
                        climateRecs[mo] = Climate.TimestepData[ecoregion.Index, mo];
                        mon_T_normal[mo] = (climateRecs[mo].AvgMinTemp + climateRecs[mo].AvgMinTemp) / 2;
                    }

                    foreach (KeyValuePair<int, IClimateRecord[,]> timeStep in allData)
                    {
                        if (timeStep.Key != 0)
                        {
                            acs[timestepIndex] = new AnnualClimate(ecoregion, timeStep.Key, startYear + timeStep.Key, Landis.Extension.Succession.Century.EcoregionData.Latitude[ecoregion]); // Latitude should be given
                            timestepIndex++;
                        }
                    }
                    double AWC = Landis.Extension.Succession.Century.EcoregionData.FieldCapacity[ecoregion] - Landis.Extension.Succession.Century.EcoregionData.WiltingPoint[ecoregion];
                    double latitude = Landis.Extension.Succession.Century.EcoregionData.Latitude[ecoregion];
                    new PDSI_Calculator().CalculatePDSI(acs, mon_T_normal, AWC, latitude, outputFilePath, UnitSystem.metrics);
                }
            }
        }

        public static void GetPDSI_Test()
        {
            IEcoregion ecoregion = Climate.ModelCore.Ecoregions[0];
            //here:
            string outputFilePath = @"C:\Program Files\LANDIS-II\v6\examples\base-BDA_1\PDSI_BaseBDA_Test.csv";
            File.WriteAllText(outputFilePath, String.Empty);
            int startYear = 1893, endYear = 1897;
            AnnualClimate[] acs;
            if (endYear > startYear)
            {
                int numOfYears = endYear - startYear + 1;
                acs = new AnnualClimate[numOfYears];


                double[] mon_T_normal = new double[12] { 19.693, 23.849, 34.988, 49.082, 60.467, 70.074, 75.505, 73.478, 64.484, 52.634, 36.201, 24.267 };
                IClimateRecord[] climateRecs = new ClimateRecord[12];

                //Climate.TimestepData = allData[0];
                //for (int mo = 0; mo < 12; mo++)
                //{
                //    climateRecs[mo] = Climate.TimestepData[ecoregion.Index, mo];
                //    //mon_T_normal[mo] = (climateRecs[mo].AvgMinTemp + climateRecs[mo].AvgMinTemp) / 2;
                //}

                acs[0] = new AnnualClimate(ecoregion, 1893, 0);
                acs[0].MonthlyTemp = new double[] { 14.371, 14.000, 26.435, 44.250, 54.645, 70.683, 73.355, 69.323, 63.600, 48.806, 32.867, 19.161 };
                acs[0].MonthlyPrecip = new double[] { 0.610, 1.500, 1.730, 4.050, 1.950, 0.790, 3.020, 2.570, 1.430, 0.850, 1.260, 2.350 };

                acs[1] = new AnnualClimate(ecoregion, 1894, 0);
                acs[1].MonthlyTemp = new double[] { 12.705, 14.979, 37.984, 49.700, 61.209, 71.463, 77.935, 74.312, 65.283, 51.516, 34.767, 29.548 };
                acs[1].MonthlyPrecip = new double[] { 0.700, 0.550, 0.580, 4.240, 2.430, 1.150, 0.580, 1.480, 0.550, 1.760, 0.050, 1.000 };

                acs[2] = new AnnualClimate(ecoregion, 1895, 0);
                acs[2].MonthlyTemp = new double[] { 12.519, 17.964, 33.994, 54.506, 60.411, 66.172, 70.548, 69.622, 65.288, 44.795, 32.433, 23.333 };
                acs[2].MonthlyPrecip = new double[] { 0.650, 0.540, 0.520, 3.980, 2.380, 6.240, 2.320, 3.920, 4.770, 0.060, 1.040, 0.000 };

                acs[3] = new AnnualClimate(ecoregion, 1896, 0);
                acs[3].MonthlyTemp = new double[] { 23.258, 27.397, 26.425, 48.833, 62.790, 68.054, 71.365, 70.677, 57.991, 46.355, 21.154, 28.597 };
                acs[3].MonthlyPrecip = new double[] { 0.250, 0.270, 1.670, 5.680, 6.240, 7.740, 5.550, 1.660, 1.810, 3.230, 3.850, 0.230 };

                acs[4] = new AnnualClimate(ecoregion, 1897, 0);
                acs[4].MonthlyTemp = new double[] { 13.758, 20.179, 26.613, 46.700, 59.016, 66.533, 74.032, 67.928, 71.617, 54.613, 32.450, 18.686 };
                acs[4].MonthlyPrecip = new double[] { 2.500, 0.540, 3.010, 4.480, 0.980, 5.820, 3.780, 1.600, 1.010, 1.940, 0.910, 2.950 };

                

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


    }

}
