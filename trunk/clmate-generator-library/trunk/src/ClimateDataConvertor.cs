//  Copyright: Portland State University 2009-2014
//  Authors:  Robert M. Scheller, John McNabb and Amin Almassian

using System;
//using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Landis.Core;

namespace Landis.Library.Climate
{
    public class ClimateDataConvertor
    {        
        // JM: private enum used in parsing data
        private enum FileSection
        {
            Precipitation = 1,
            MaxTemperature = 2,
            MinTemperature = 3,
            NDeposition = 6,
            Winddirection = 4,
            Windspeed = 5,
            CO2 = 7
        }

        public static void Convert_USGS_to_ClimateData_FillAlldata(TemporalGranularity timeStep, string climateFile, string climateFileFormat, Climate.Phase climatePhase)
        {
            Dictionary<int, ClimateRecord[][]> allDataRef = null; //this dictionary is filled out either by Daily data or Monthly
            if (climatePhase == Climate.Phase.Future_Climate)
                allDataRef = Climate.Future_AllData;

            if (climatePhase == Climate.Phase.SpinUp_Climate)
                allDataRef = Climate.Spinup_AllData;

            // parse the input file into lists of timestamps and corresponding climate records arrays
            List<string> timeStamps;
            List<ClimateRecord>[] climateRecords;       // indexing: [ecoregion.Count][i]
            Convert_USGS_to_ClimateData(timeStep, climateFile, climateFileFormat, out timeStamps, out climateRecords);
            
            // break up the ecoregion lists into a dictionary by year based on timeStamp keys
            var yearData = new Dictionary<int, List<ClimateRecord>[]>();

            var currentYear = -999;
            List<ClimateRecord>[] yearRecords = null;

            for (var j = 0; j < timeStamps.Count; ++j)
            {
                var year = int.Parse(timeStamps[j].Substring(0, 4));

                // timestamps are grouped by year in the input files
                if (year != currentYear)
                {
                    // make yearRecords instance for the new year
                    currentYear = year;
                    yearData[year] = yearRecords = new List<ClimateRecord>[Climate.ModelCore.Ecoregions.Count];
                    for (var i = 0; i < Climate.ModelCore.Ecoregions.Count; ++i)
                        yearRecords[i] = new List<ClimateRecord>();
                }

                // add the climate records onto the year records
                for (var i = 0; i < Climate.ModelCore.Ecoregions.Count; ++i)
                    yearRecords[i].Add(climateRecords[i][j]);
            }

            // transfer the data to allDataRef and 
            // do some basic error checking

            if (allDataRef == null)
                allDataRef = new Dictionary<int, ClimateRecord[][]>();
            else
                allDataRef.Clear();

            foreach (var key in yearData.Keys)
            {
                allDataRef[key] = new ClimateRecord[Climate.ModelCore.Ecoregions.Count][];

                for (var i = 0; i < Climate.ModelCore.Ecoregions.Count; ++i)
                {
                    if (timeStep == TemporalGranularity.Monthly && yearData[key][i].Count != 12)
                        throw new ApplicationException(string.Format("Error in ClimateDataConvertor: Monthly data for year {0} in climate file '{1}' does not have 12 records.  It has {2} records.", key, climateFile, yearData[key][i].Count));

                    if (timeStep == TemporalGranularity.Daily && yearData[key][i].Count != 365 && yearData[key][i].Count != 366)
                        throw new ApplicationException(string.Format("Error in ClimateDataConvertor: Daily data for year {0} in climate file '{1}' does not have 365 or 366 records.  It has {2} records.", key, climateFile, yearData[key][i].Count));

                    // convert the yearRecords from List<ClimateRecord>[] to ClimateRecord[][]
                    allDataRef[key][i] = yearData[key][i].ToArray();
                }
            }
        }

        private static void Convert_USGS_to_ClimateData(TemporalGranularity sourceTemporalGranularity, string climateFile, string climateFileFormat, out List<string> timeStamps, out List<ClimateRecord>[] climateRecords)
        {
            // each item in timeStamps is the timeStamp 'key' for the data
            // each item in climateRecords is of length Climate.ModelCore.Ecoregions.Count, and is filled in with data from the input file, indexed by Ecoregion.Index.

            timeStamps = new List<string>();
            climateRecords = new List<ClimateRecord>[Climate.ModelCore.Ecoregions.Count];
            for (var i = 0; i < Climate.ModelCore.Ecoregions.Count; ++i)
                climateRecords[i] = new List<ClimateRecord>();

            // get trigger words for parsing based on file format
            ClimateFileFormatProvider format = new ClimateFileFormatProvider(climateFileFormat);

            StreamReader sreader;

            try
            {
                sreader = new StreamReader(climateFile);
            }
            catch
            {
                throw new ApplicationException("Error in ClimateDataConvertor: Cannot open climate file" + climateFile);
            }

            Climate.ModelCore.UI.WriteLine("   Converting raw data from text file: {0}, Format={1}, Temporal={2}.", climateFile, climateFileFormat.ToLower(), sourceTemporalGranularity);

            // maps from ecoregion column index in the input file to the ecoregion.index for the region
            int[] ecoRegionIndexMap = null;
            var ecoRegionCount = 0;

            var rowIndex = -1;
            var sectionIndex = -1;
            FileSection section = 0;

            while (sreader.Peek() >= 0)
            {
                var fields = sreader.ReadLine().Replace(" ", "").Split(',').ToList();    // JM: don't know if stripping blanks is needed, but just in case

                // check for trigger word
                if (fields[0].StartsWith("#"))
                {
                    // determine which section we're in
                    var triggerWord = fields[0].TrimStart('#');   // remove the leading "#"

                    if (format.PrecipTriggerWord.FindIndex(x => x.Equals(triggerWord, StringComparison.OrdinalIgnoreCase)) >= 0)
                        section = FileSection.Precipitation;
                    else if (format.MaxTempTriggerWord.FindIndex(x => x.Equals(triggerWord, StringComparison.OrdinalIgnoreCase)) >= 0)
                        section = FileSection.MaxTemperature;
                    else if (format.MinTempTriggerWord.FindIndex(x => x.Equals(triggerWord, StringComparison.OrdinalIgnoreCase)) >= 0)
                        section = FileSection.MinTemperature;
                    else if (format.NDepositionTriggerWord.FindIndex(x => x.Equals(triggerWord, StringComparison.OrdinalIgnoreCase)) >= 0)
                        section = FileSection.NDeposition;
                    else if (format.WindDirectionTriggerWord.FindIndex(x => x.Equals(triggerWord, StringComparison.OrdinalIgnoreCase)) >= 0)
                        section = FileSection.Winddirection;
                    else if (format.WindSpeedTriggerWord.FindIndex(x => x.Equals(triggerWord, StringComparison.OrdinalIgnoreCase)) >= 0)
                        section = FileSection.Windspeed;
                    else if (format.CO2TriggerWord.FindIndex(x => x.Equals(triggerWord, StringComparison.OrdinalIgnoreCase)) >= 0)
                        section = FileSection.CO2;
                    else
                        throw new ApplicationException(string.Format("Error in ClimateDataConvertor: Unrecognized trigger word '{0}' in climate file '{1}'.", triggerWord, climateFile));

                    sectionIndex++;

                    // if this is the first section then parse the ecoregions, etc.
                    if (sectionIndex == 0)
                    {
                        // read next line to get ecoregion headers
                        var ecoRegionHeaders = sreader.ReadLine().Replace(" ", "").Split(',').ToList();
                        ecoRegionHeaders.RemoveAt(0);   // remove blank cell at the beginning of ecoregion header row

                        // JM: the next line assumes all input files have exactly three groups of columns: Mean, Variance, Std_dev
                        ecoRegionCount = ecoRegionHeaders.Count / 3;

                        if (ecoRegionCount == 0)
                            throw new ApplicationException(string.Format("Error in ClimateDataConvertor: climate file '{0}' contains no ecoregion data.", climateFile));

                        var modelCoreActiveEcoRegionCount = Climate.ModelCore.Ecoregions.Count(x => x.Active);

                        if (ecoRegionCount != modelCoreActiveEcoRegionCount)
                            throw new ApplicationException(string.Format("Error in ClimateDataConvertor: climate file '{0}' contains data for {1} ecoregions, but the simulation has {2} active ecoregions.", climateFile, ecoRegionCount, modelCoreActiveEcoRegionCount));

                        // determine the map from ecoregions in this file to ecoregion indicies in ModelCore
                        ecoRegionIndexMap = new int[ecoRegionCount];
                        for (var i = 0; i < ecoRegionCount; ++i)
                        {
                            IEcoregion eco = Climate.ModelCore.Ecoregions[ecoRegionHeaders[i]];     // JM:  Ecoregions appear to be indexed by string name, but I don't know if it is case-sensitive.
                            if (eco != null && eco.Active)
                                ecoRegionIndexMap[i] = eco.Index;
                            else
                                throw new ApplicationException(string.Format("Error in ClimateDataConvertor: Ecoregion name '{0}' in climate file '{1}' is not recognized or is inactive", ecoRegionHeaders[i], climateFile));
                        }
                    }
                    else
                        // skip ecoregion header line
                        sreader.ReadLine();

                    // skip data headers
                    sreader.ReadLine();

                    // get next line as first line of data
                    fields = sreader.ReadLine().Replace(" ", "").Split(',').ToList();

                    // reset row index
                    rowIndex = -1;
                }


                // **
                // process line of data
                
                ++rowIndex;

                // grab the key as the first field and remove it from the data
                var key = fields[0];
                fields.RemoveAt(0);

                // if this is the first section then add key to timestamps and add new climate records for this rowIndex to the ecoregion array
                if (sectionIndex == 0)
                {
                    timeStamps.Add(key);
                    for (var i = 0; i < Climate.ModelCore.Ecoregions.Count; ++i)
                        climateRecords[i].Add(new ClimateRecord());
                }
                else
                {
                    // check that the timestamp key order matches
                    if (key != timeStamps[rowIndex])
                        throw new ApplicationException(string.Format("Error in ClimateDataConvertor: Timestamp order mismatch in section '{0}' timestamp '{1}' in climate file '{2}'.", section, key, climateFile));
                }

                for (var i = 0; i < ecoRegionCount; ++i)
                {
                    var ecoIndex = ecoRegionIndexMap[i];

                    var ecoRecords = climateRecords[ecoIndex];

                    // JM: the next line assumes all input files have exactly three groups of columns: Mean, Variance, Std_dev
                    var mean = double.Parse(fields[i]);
                    var variance = double.Parse(fields[ecoRegionCount + i]);
                    var stdev = double.Parse(fields[2 * ecoRegionCount + i]);

                    switch (section)
                    {
                        case FileSection.Precipitation:
                            ecoRecords[rowIndex].AvgPpt = mean * format.PrecipTransformation;
                            ecoRecords[rowIndex].StdDevPpt = stdev * format.PrecipTransformation;
                            break;

                        case FileSection.MaxTemperature:
                        case FileSection.MinTemperature:

                            mean += format.TemperatureTransformation;


                            if (section == FileSection.MaxTemperature)
                                ecoRecords[rowIndex].AvgMaxTemp = mean;
                            else
                                ecoRecords[rowIndex].AvgMinTemp = mean;

                            // for temperature variance wait until both min and max have been read before calculating the final value
                            if (ecoRecords[rowIndex].AvgVarTemp == -99.0)
                                ecoRecords[rowIndex].AvgVarTemp = variance;        // set AvgVarTemp to the first value we have (min or max)
                            else
                                // have both min and max, so average the variance
                                ecoRecords[rowIndex].AvgVarTemp = (ecoRecords[rowIndex].AvgVarTemp + variance) / 2.0;

                            ecoRecords[rowIndex].StdDevTemp = System.Math.Sqrt(ecoRecords[rowIndex].AvgVarTemp);      // this will set the st dev even if the data file only has one temperature section
                            break;

                        case FileSection.Winddirection:

                            ecoRecords[rowIndex].AvgWindDirection = mean;
                            if (mean < 180) mean += (format.WindDirectionTransformation);
                            else mean -= (format.WindDirectionTransformation);
                                                       
                            ecoRecords[rowIndex].AvgVarWindDirection = variance;
                            ecoRecords[rowIndex].StdDevWindDirection = stdev;
                            break;

                        case FileSection.Windspeed:
                            ecoRecords[rowIndex].AvgWindSpeed = mean * format.WindSpeedTransformation;
                            ecoRecords[rowIndex].AvgVarWindSpeed = variance;
                            ecoRecords[rowIndex].StdDevWindSpeed = stdev;
                            break;

                        case FileSection.NDeposition:
                            ecoRecords[rowIndex].AvgNDeposition = mean;
                            ecoRecords[rowIndex].AvgVarNDeposition = variance;
                            ecoRecords[rowIndex].StdDevNDeposition = stdev;
                            break;

                        case FileSection.CO2:
                            ecoRecords[rowIndex].AvgCO2 = mean;
                            ecoRecords[rowIndex].AvgVarCO2 = variance;
                            ecoRecords[rowIndex].StdDevCO2 = stdev;
                            break;
                    }
                }                
            }
        }

      
    }
}

