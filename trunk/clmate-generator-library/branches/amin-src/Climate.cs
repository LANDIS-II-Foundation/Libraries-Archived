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
            get
            {
                return allData;
            }
        }
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

        public static void Write(IEcoregionDataset ecoregionDataset)
        {
            foreach (IEcoregion ecoregion in ecoregionDataset)
            {
                for (int i = 0; i < 12; i++)
                {
                    ModelCore.Log.WriteLine("Eco={0}, Month={1}, AvgMinTemp={2:0.0}, AvgMaxTemp={3:0.0}, StdDevTemp={4:0.0}, AvgPpt={5:0.0}, StdDevPpt={6:0.0}.",
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

            if (writeOutput)
                Write(Climate.ModelCore.Ecoregions);

        }

        public static void GenerateClimate_GetPDSI(int startYear, int endYear)
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
                    acs[i - startYear] = new AnnualClimate(ecoregion, 0, i, Landis.Extension.Succession.Century.EcoregionData.Latitude[ecoregion]); // Latitude should be given
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
            string outputFilePath = @"PDSI_BaseBDA.csv";
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


        /// <summary>
        /// Converts USGS Data to Input climate Data 
        /// </summary>
        public static void Convert_USGS_to_ClimateData(Period period, string climateFile)
        {
            string path = climateFile;
            StreamReader sreader ;
            // monthly and daily clmates should be filled before in order to chack weather input climatefile can be processed as daily or monthly
            List<string> montlyClimates;
            List<string> DailyClimate;
            if (period == Period.Daily)
            {
               
                //string path = @"D:\PSU\Landis_II\amin-branch\USGS_Data\Hayhoe_Climate_Data1.csv";
                sreader = new StreamReader(path);
                string line;
                string[] fields;
                string tempScenarioName = "";
                DataTable _dataTableDataByTime = new DataTable();
                int numberOfAllEcorigions = 0;
                line = sreader.ReadLine();
                fields = line.Split(',');
                tempScenarioName = fields[0].Substring(1, fields[0].LastIndexOf("t") - 2);
                line = sreader.ReadLine();
                fields = line.Split(',');
                int totalRows = 0;
                string[,] wholedata;
                string CurrentScenarioName = "";

                string CurrentScenarioType = "";
                Dictionary<string, double[]> century_climate_Dic = new Dictionary<string, double[]>();

                string currentT;
                string currentSTD;
                string currentPart = "";
                int totalRow = 0;
                string key = "";
                int IndexT = 0;
                int IndexSTD = 0;
                //bool firstFlag = false;
                bool emptytxt = false;

                foreach (string field in fields)
                {
                    if (field != "" && Convert.ToInt16(field) > numberOfAllEcorigions)
                    {
                        numberOfAllEcorigions = Convert.ToInt16(field);
                    }
                }
                //6 beacuse for each ecoriogn we need AvgMaxT, StdMaxT, AvgMinT,StdMinT,AvgPpt,	StdDev
                int dicSize = numberOfAllEcorigions * 3;
                sreader.Close();
                StreamReader reader = new StreamReader(path);

                while (reader.Peek() >= 0)
                {
                    line = reader.ReadLine();
                    fields = line.Split(',');
                    foreach (string field in fields)
                    {
                        if (field.Contains("#"))
                        {
                            //tempScenarioName = CurrentScenarioName;
                            if (field.Contains("tmax") || field.Contains("tmin"))
                            {
                                CurrentScenarioName = field.Substring(1, field.LastIndexOf("t") - 2);
                                if (field.Contains("tmax"))
                                    CurrentScenarioType = "tmax";
                                if (field.Contains("tmin"))
                                    CurrentScenarioType = "tmin";
                            }
                            if (field.Contains("pr"))
                            {
                                CurrentScenarioName = field.Substring(1, field.LastIndexOf("p") - 2);
                                CurrentScenarioType = "pr";

                            }

                            //if (tempScenarioName != CurrentScenarioName)// firstFlag == false)
                            //{
                            //    tempScenarioName = CurrentScenarioName;
                            //    //firstFlag = true;
                            //}



                            //line = reader.ReadLine();
                            //fields = line.Split(',');

                        }



                    }

                    if (fields[0] == string.Empty && !fields[0].Contains("#"))
                    {
                        line = reader.ReadLine();
                        fields = line.Split(',');

                        if (fields[0].Contains("TIME"))
                        {
                            line = reader.ReadLine();
                            fields = line.Split(',');

                            //now fill array 
                            //Get the lenght of array according to the number of ecorigions/
                            //

                        }
                    }
                    if (CurrentScenarioName == tempScenarioName && !fields[0].Contains("#"))
                    {

                        key = fields[0].ToString();
                        if (CurrentScenarioType.Contains("max"))
                        {
                            IndexT = 0;
                            //IndexSTD = 1;
                            //int indexofSTD = 0;
                            //indexofSTD = fields.Length - (numberOfAllEcorigions);

                            century_climate_Dic.Add(key, new double[dicSize]);//{ currentT, currentSTD, 0, 0, 0, 0 });

                            //set index of max and maxSTD for each ecorigion
                            for (int i = 1; i <= numberOfAllEcorigions; i++)
                            {
                                currentT = fields[i];
                                //if (indexofSTD < 26)
                                //{
                                    //currentSTD = fields[indexofSTD];
                                    century_climate_Dic[key].SetValue(Convert.ToDouble(currentT), IndexT);
                                    //century_climate_Dic[key].SetValue(Convert.ToDouble(currentSTD), IndexSTD);
                                    IndexT = IndexT + 3;
                                    //IndexT = IndexT + 6;
                                    //IndexSTD = IndexSTD + 6;
                                    //indexofSTD++;
                                //}

                            }
                        }
                        if (CurrentScenarioType.Contains("min"))
                        {
                            IndexT = 1;
                            //IndexSTD = 3;
                            //int indexofSTD = 0;
                            //indexofSTD = fields.Length - (numberOfAllEcorigions);

                            // century_climate_Dic.Add(key, new double[dicSize]);//{ currentT, currentSTD, 0, 0, 0, 0 });

                            //set index of max and maxSTD for each ecorigion
                            for (int i = 1; i <= numberOfAllEcorigions; i++)
                            {
                                currentT = fields[i];
                                //if (indexofSTD < 26)
                                //{
                                    //currentSTD = fields[indexofSTD];
                                    century_climate_Dic[key].SetValue(Convert.ToDouble(currentT), IndexT);
                                    //century_climate_Dic[key].SetValue(Convert.ToDouble(currentSTD), IndexSTD);
                                    IndexT = IndexT + 3;
                                    //IndexT = IndexT + 6;
                                //    IndexSTD = IndexSTD + 6;
                                //    indexofSTD++;
                                //}

                            }
                        }
                        if (CurrentScenarioType.Contains("pr"))
                        {
                            IndexT = 2;
                            //IndexSTD = 5;
                            //int indexofSTD = 0;
                            //indexofSTD = fields.Length - (numberOfAllEcorigions);

                            // century_climate_Dic.Add(key, new double[dicSize]);//{ currentT, currentSTD, 0, 0, 0, 0 });

                            //set index of max and maxSTD for each ecorigion
                            for (int i = 1; i <= numberOfAllEcorigions; i++)
                            {
                                currentT = fields[i];
                                //if (indexofSTD < 26)
                                //{
                                    //currentSTD = fields[indexofSTD];
                                    century_climate_Dic[key].SetValue(Convert.ToDouble(currentT), IndexT);
                                    //century_climate_Dic[key].SetValue(Convert.ToDouble(currentSTD), IndexSTD);
                                    IndexT = IndexT + 3;
                                    //IndexT = IndexT + 6;
                                    //IndexSTD = IndexSTD + 6;
                                    //indexofSTD++;
                                }

                            }

                        }
                    }
                    if (CurrentScenarioName != tempScenarioName || reader.EndOfStream)
                    {
                        //tempScenarioName = CurrentScenarioName;
                        //Print file for one scenario then clear dictionary to use for another scenario

                        //Daily peiod
                        string centuryPath = @"C:\Program Files\LANDIS-II\v6\examples\base-BDA_1\Century_Climate_Inputs_NEW.txt";
                        //int AverageMaxT = 0;
                        //int AverageMaxSTD = 1;
                        //int AverageMinT = 2;
                        //int AverageMinSTD = 3;
                        //int AveragePrec = 4;
                        //int AveragePrecSTD = 5;

                        int AverageMaxT = 0;
                        //int AverageMaxSTD = 1;
                        int AverageMinT = 1;
                        //int AverageMinSTD = 3;
                        int AveragePrec = 2;
                        //int AveragePrecSTD = 5;
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(centuryPath, emptytxt))
                        {
                            file.WriteLine("LandisData" + "Climate Data" + "\n");
                            file.WriteLine("ClimateTable \n");
                            file.WriteLine(tempScenarioName + "\n");
                            file.WriteLine(">>Eco" + "\t" + "Time" + "\t" + "\t" + "AvgMaxT" + "\t"  + "AvgMinT" + "\t" + "AvgPpt"  + "\n");
                            file.WriteLine(">>Name" + "\t" + "Step" + "\t" + "\t" + "(C)" + "\t" +  "(C)" + "\t"  + "(C)" + "\n");
                           //file.WriteLine(">>Eco" + "\t" + "Time" + "\t" + "\t" + "AvgMaxT" + "\t" + "StdMaxT" + "\t" + "AvgMinT" + "\t" + "StdMinT" + "\t" + "AvgPpt" + "\t" + "StdDev" + "\n");
                           // file.WriteLine(">>Name" + "\t" + "Step" + "\t" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\n");
                            for (int i = 1; i <= numberOfAllEcorigions; i++)
                            {
                                foreach (KeyValuePair<string, double[]> row in century_climate_Dic)
                                {

                                    file.WriteLine("eco" + i.ToString() + "\t" + row.Key.Remove(10) + "\t" + Math.Round(row.Value[AverageMaxT], 2) +  "\t" + Math.Round(row.Value[AverageMinT], 2) +  "\t" + Math.Round(row.Value[AveragePrec], 2)  + "\n");
                                    //file.WriteLine("eco" + i.ToString() + "\t" + row.Key.Remove(10) + "\t" + Math.Round(row.Value[AverageMaxT], 2) + "\t" + Math.Round(row.Value[AverageMaxSTD], 2) + "\t" + Math.Round(row.Value[AverageMinT], 2) + "\t" + Math.Round(row.Value[AverageMinSTD], 2) + "\t" + Math.Round(row.Value[AveragePrec], 2) + "\t" + Math.Round(row.Value[AveragePrecSTD], 2) + "\n");
                                }

                                AverageMaxT = AverageMaxT + 3;
                                //AverageMaxSTD = AverageMaxSTD + 6;
                                AverageMinT = AverageMinT + 3;
                                //AverageMinSTD = AverageMinSTD + 6;
                                AveragePrec = AveragePrec + 3;
                                //AveragePrecSTD = AveragePrecSTD + 6;
                            }

                        }
                        century_climate_Dic.Clear();
                        emptytxt = true;
                        tempScenarioName = CurrentScenarioName;

                    }

            }
            else if (period == Period.Monthly)
            {

            }
                //while (sreader.Peek() >= 0)
                //{
                //    if (_dataTableDataByTime.Columns.Count == 0)
                //    {
                //        foreach (string field in fields)
                //        {
                //             //will add default names like "Column1", "Column2", and so on
                //            _dataTableDataByTime.Columns.Add();
                //        }
                //}
                //_dataTableDataByTime.Rows.Add(fields);

                //}

                //string tableName = "Hayhoe_Climate_Data_1";
                //OleDbConnection dbConnection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;");
                //Exception excelFileReadingException = null;
                //try
                //{
                //dbConnection.Open();
                //OleDbDataAdapter dbAdapter =
                //    new OleDbDataAdapter
                //        ("SELECT * FROM [original$]", dbConnection);
                //dbAdapter.Fill(_dataTableDataByTime);
                //string currentT;
                //string currentSTD;
                //string currentPart = "";
                ////Dictionary<int, string[]> century_climate_Dic = new Dictionary<int, string[]>();
                //int totalRow = 0;
                //string key = "";
                //int IndexT = 0;
                //int IndexSTD = 0;

                //for (int i = 0; i < _dataTableDataByTime.Rows.Count; i++)
                //{
                //    key = _dataTableDataByTime.Rows[i][0].ToString();
                //    if (key.Contains("TIMESTEP"))
                //    {
                //        if (currentPart == "" || currentPart =="pr")
                //        {
                //            currentPart = "max";
                //            IndexT = 1;
                //            IndexSTD = 2;
                //            //i++;
                //        }
                //        else if (currentPart == "max")
                //        {
                //            currentPart = "min";
                //            IndexT = 3;
                //            IndexSTD = 4;
                //        }
                //        else if (currentPart == "min")
                //        {
                //            currentPart = "pr";
                //            IndexT = 5;
                //            IndexSTD = 6;
                //        }
                //    }
                //    else
                //    {
                //        if (_dataTableDataByTime.Rows[i][0].ToString().Trim() != string.Empty)
                //        {
                //            //now should fetch the mean column and STD column
                //            if (_dataTableDataByTime.Rows[i][1] !="" && _dataTableDataByTime.Rows[i][2] != "")
                //            {
                //                currentT = _dataTableDataByTime.Rows[i][1].ToString();
                //                currentSTD = _dataTableDataByTime.Rows[i][2].ToString();

                //                if (currentPart == "max")
                //                {
                //                    //insert new item 
                //                    century_climate_Dic.Add(i, new string[7]);//{ currentT, currentSTD, 0, 0, 0, 0 });
                //                    century_climate_Dic[i].SetValue(key, 0);
                //                    century_climate_Dic[i].SetValue(currentT, IndexT);
                //                    century_climate_Dic[i].SetValue(currentSTD, IndexSTD);
                //                }
                //                else if (currentPart == "min")
                //                {
                //                    century_climate_Dic[i].SetValue(key, 0);
                //                    century_climate_Dic[i].SetValue(currentT, IndexT);
                //                    century_climate_Dic[i].SetValue(currentSTD, IndexSTD);

                //                }
                //                else if (currentPart == "pr")
                //                {
                //                    century_climate_Dic[i].SetValue(key, 0);
                //                    century_climate_Dic[i].SetValue(currentT, IndexT);
                //                    century_climate_Dic[i].SetValue(currentSTD, IndexSTD);
                //                }
                //            }

                //        }

                //    }

                //}

                //Now Dictionary is ready to print in txt file
                //Daily peiod
                //string centuryPath = @"C:\Program Files\LANDIS-II\v6\examples\base-BDA_1\Century_Climate_Inputs_1.txt";
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(centuryPath, false))
                //{
                //    file.WriteLine("LandisData" + "Climate Data" + "\n");
                //    file.WriteLine("ClimateTable \n");
                //    file.WriteLine(">>Eco" + "\t" + "Time" + "\t" + "\t" + "AvgMaxT" + "\t" + "StdMaxT" + "\t" + "AvgMinT" + "\t" + "StdMinT" + "\t" + "AvgPpt" + "\t" + "StdDev" + "\n");
                //    file.WriteLine(">>Name" + "\t" + "Step" + "\t" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\n");
                //    foreach (KeyValuePair<int, string[]> row in century_climate_Dic)
                //    {
                //        file.WriteLine("eco1" + "\t" + row.Value[0].Remove(10) + "\t" + Math.Round(Convert.ToDouble(row.Value[1]), 2) + "\t" + Math.Round(Convert.ToDouble(row.Value[2]), 2) + "\t" + Math.Round(Convert.ToDouble(row.Value[3]), 2) + "\t" + Math.Round(Convert.ToDouble(row.Value[4]), 2) + "\t" + Math.Round(Convert.ToDouble(row.Value[5]), 2) + "\t" + Math.Round(Convert.ToDouble(row.Value[6]), 2) + "\n");
                //    }

                //}
            
            // should apply later
                //monthly period
                //string currentYear = "";
                //int currentMonth = 1;
                //int tempMonth = 1;
                //double AverageMaxT = 0;
                //double AverageMaxSTD = 0;
                //double AverageMinT = 0;
                //double AverageMinSTD = 0;
                //double AveragePrec = 0;
                //double AveragePrecSTD = 0;
                //int numberOfDays = 0;
                //string centuryPathMonthly = @"C:\Program Files\LANDIS-II\v6\examples\base-BDA_1\Century_Climate_Inputs_2.txt";
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(centuryPathMonthly, false))
                //{
                //    file.WriteLine("LandisData" + "Climate Data" + "\n");
                //    file.WriteLine("ClimateTable \n");
                //    file.WriteLine(">>Eco" + "\t" + "Year" + "\t" + "Month" + "\t" + "AvgMaxT" + "\t" + "StdMaxT" + "\t" + "AvgMinT" + "\t" + "StdMinT" + "\t" + "AvgPpt" + "\t" + "StdDev" + "\n");
                //    file.WriteLine(">>Name" + "\t" + " " + "\t" + " " + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\t" + "(C)" + "\n");

                //    foreach (KeyValuePair<int, string[]> row in century_climate_Dic)
                //    {
                //        if (currentYear == row.Value[0].Substring(0, 4).ToString())
                //        {

                //            if (currentMonth == Convert.ToInt16(row.Value[0].Substring(5, 2)))
                //            {
                //                AverageMaxT += Math.Round(Convert.ToDouble(row.Value[1]), 2);
                //                AverageMaxSTD += Math.Round(Convert.ToDouble(row.Value[2]), 2);
                //                AverageMinT += Math.Round(Convert.ToDouble(row.Value[3]), 2);
                //                AverageMinSTD += Math.Round(Convert.ToDouble(row.Value[4]), 2);
                //                AveragePrec += Math.Round(Convert.ToDouble(row.Value[5]), 2);
                //                AveragePrecSTD += Math.Round(Convert.ToDouble(row.Value[6]), 2);
                //                numberOfDays++;
                //            }
                //            else
                //            {

                //                file.WriteLine("eco1" + "\t" + currentYear + "\t" + currentMonth + "\t" + Math.Round(AverageMaxT / numberOfDays, 2) + "\t" + Math.Round(AverageMaxSTD / numberOfDays, 2) + "\t" + Math.Round(AverageMinT / numberOfDays, 2) + "\t" + Math.Round(AverageMinSTD / numberOfDays, 2) + "\t" + Math.Round(AveragePrec / numberOfDays, 2) + "\t" + Math.Round(AveragePrecSTD / numberOfDays, 2) + "\n");
                //                //tempMonth = currentMonth;
                //                currentMonth = Convert.ToInt16(row.Value[0].Substring(5, 2));
                //                //if (tempMonth != currentMonth)

                //                AverageMaxT = 0;
                //                AverageMaxSTD = 0;
                //                AverageMinT = 0;
                //                AverageMinSTD = 0;
                //                AveragePrec = 0;
                //                AveragePrecSTD = 0;
                //                numberOfDays = 0;
                //                AverageMaxT += Math.Round(Convert.ToDouble(row.Value[1]), 2);
                //                AverageMaxSTD += Math.Round(Convert.ToDouble(row.Value[2]), 2);
                //                AverageMinT += Math.Round(Convert.ToDouble(row.Value[3]), 2);
                //                AverageMinSTD += Math.Round(Convert.ToDouble(row.Value[4]), 2);
                //                AveragePrec += Math.Round(Convert.ToDouble(row.Value[5]), 2);
                //                AveragePrecSTD += Math.Round(Convert.ToDouble(row.Value[6]), 2);
                //                numberOfDays++;
                //            }

                //        }
                //        else
                //        {
                //            if (currentMonth == 12)
                //                file.WriteLine("eco1" + "\t" + currentYear + "\t" + currentMonth + "\t" + Math.Round(AverageMaxT / numberOfDays, 2) + "\t" + Math.Round(AverageMaxSTD / numberOfDays, 2) + "\t" + Math.Round(AverageMinT / numberOfDays, 2) + "\t" + Math.Round(AverageMinSTD / numberOfDays, 2) + "\t" + Math.Round(AveragePrec / numberOfDays, 2) + "\t" + Math.Round(AveragePrecSTD / numberOfDays, 2) + "\n");

                //            currentYear = row.Value[0].Substring(0, 4).ToString();
                //            currentMonth = 1;
                //            AverageMaxT = 0;
                //            AverageMaxSTD = 0;
                //            AverageMinT = 0;
                //            AverageMinSTD = 0;
                //            AveragePrec = 0;
                //            AveragePrecSTD = 0;
                //            numberOfDays = 0;
                //            AverageMaxT += Math.Round(Convert.ToDouble(row.Value[1]), 2);
                //            AverageMaxSTD += Math.Round(Convert.ToDouble(row.Value[2]), 2);
                //            AverageMinT += Math.Round(Convert.ToDouble(row.Value[3]), 2);
                //            AverageMinSTD += Math.Round(Convert.ToDouble(row.Value[4]), 2);
                //            AveragePrec += Math.Round(Convert.ToDouble(row.Value[5]), 2);
                //            AveragePrecSTD += Math.Round(Convert.ToDouble(row.Value[6]), 2);
                //            numberOfDays++;
                //        }

                //    }
                //}




            ///-------------------------------------
            //}

            //catch (Exception ex)
            //{
            //    excelFileReadingException = ex;
            //    //MessageBox.Show("Import Failed: Could not read the excel file or excel file was not found!");
            //}
            //finally
            //{
            //    dbConnection.Close();
            //}
            
        }


    }

}
