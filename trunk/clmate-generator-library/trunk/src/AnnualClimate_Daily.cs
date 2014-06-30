//  Copyright: Portland State University 2009-2014
//  Authors:  Robert M. Scheller, John McNabb and Amin Almassian

using Landis.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Library.Climate
{
    public class AnnualClimate_Daily: AnnualClimate
    {
        public bool DailyDataIsLeapYear;

        public int MaxDayInYear { get { return DailyDataIsLeapYear? 366 : 365; } } // = 366;

        public double[] DailyTemp = new double[366];
        public double[] DailyMinTemp = new double[366];
        public double[] DailyMaxTemp = new double[366];
        public double[] DailyPrecip = new double[366];
        public double[] DailyPAR = new double[366];
        public double[] DailyVarTemp = new double[366];
        public double[] DailyPptVarTemp = new double[366];
        //public int tempEcoIndex = -1;

        public double[] DailyPET = new double[366];  // Potential Evapotranspiration
        public double[] DailyVPD = new double[366];  // Vapor Pressure Deficit
        public double[] DailyDayLength = new double[366];
        public double[] DailyNightLength = new double[366];
        public int[] DailyGDD = new int[366];

        //For Sequenced and Random timeStep arg should be passed
        public AnnualClimate_Daily(IEcoregion ecoregion, double latitude, Climate.Phase spinupOrfuture, int timeStep, int timeStepIndex)
        {

            this.climatePhase = spinupOrfuture;
            ClimateRecord[][] timestepData = new ClimateRecord[Climate.ModelCore.Ecoregions.Count][];
            for (var i = 0; i < Climate.ModelCore.Ecoregions.Count; ++i)
                timestepData[i] = new ClimateRecord[366];

            string climateOption = Climate.ConfigParameters.ClimateTimeSeries;
            if (this.climatePhase == Climate.Phase.SpinUp_Climate)
                climateOption = Climate.ConfigParameters.SpinUpClimateTimeSeries;

            ClimateRecord[] dailyData = null;

            int actualTimeStep;

            //Climate.ModelCore.UI.WriteLine("  Calculating daily data ...  Ecoregion = {0}, Year = {1}, timestep = {2}.", ecoregion.Name, actualYear, timeStep);
            switch (climateOption)
            {
                case "Daily_RandomYear":
                    {
                        // JM: this code assumes that the constructor for AnnualClimate_Daily is ONLY called from within
                        //  AnnualClimate_Monthly.AnnualClimate_From_AnnualClimate_Daily(), and, for Daily_RandomYear, the
                        //  actualYear contains the randomly-selected year.

                        TimeStep = timeStep;
                        Dictionary<int, ClimateRecord[][]> allData;
                        List<int> randomKeyList;

                        if (this.climatePhase == Climate.Phase.Future_Climate)
                        {
                            allData = Climate.Future_AllData;
                            randomKeyList = Climate.RandSelectedTimeKeys_future;
                        }
                        else
                        {
                            allData = Climate.Spinup_AllData;
                            randomKeyList = Climate.RandSelectedTimeKeys_spinup;
                        }

                        if (timeStepIndex >= randomKeyList.Count())
                        {
                            throw new ApplicationException(string.Format("Exception: the requested Time-step {0} is out-of-range for the {1} input file.", timeStep, this.climatePhase));
                        }
                        else
                            actualTimeStep = randomKeyList[timeStepIndex];

                        Climate.ModelCore.UI.WriteLine("  AnnualClimate_Daily: Daily_RandomYear: timeStep = {0}, actualYear = {1}, phase = {2}.", timeStep, actualTimeStep, this.climatePhase);

                        dailyData = allData[actualTimeStep][ecoregion.Index];
                        CalculateDailyData(ecoregion, dailyData, actualTimeStep, latitude);
                        break;

                       
                    }
                case "Daily_AverageAllYears":
                    {
                        TimeStep = timeStep;
                        actualTimeStep = 0;

                        dailyData = AnnualClimate_AvgDaily(ecoregion, latitude);
                        CalculateDailyData(ecoregion, dailyData, actualTimeStep, latitude);
                        break;
                    }
                case "Daily_SequencedYears":
                    {
                        TimeStep = timeStep;
                        actualTimeStep = timeStep;
                        Dictionary<int, ClimateRecord[][]> allData;

                        if (this.climatePhase == Climate.Phase.Future_Climate)
                            allData = Climate.Future_AllData;
                        else
                            allData = Climate.Spinup_AllData;

                        ClimateRecord[][] yearRecords;

                        // get the climate records for the requested year, or if the year is not found, get the records for the last year
                        if (!allData.TryGetValue(timeStep, out yearRecords))
                        {
                            actualTimeStep = allData.Keys.Max();
                            yearRecords = allData[actualTimeStep];
                        }

                        dailyData = yearRecords[ecoregion.Index];
                        CalculateDailyData(ecoregion, dailyData, actualTimeStep, latitude);
                        break;
                    }
                default:
                    throw new ApplicationException(String.Format("Unknown Climate Time Series: {}", climateOption));

            }

                       
            this.beginGrowing = CalculateBeginGrowingDay_Daily(); //ecoClimate);
            this.endGrowing = CalculateEndGrowingDay_Daily(dailyData);
            this.growingDegreeDays = GrowSeasonDegreeDays();

            this.DailyDataIsLeapYear = dailyData.Length == 366;

        }

        private void CalculateDailyData(IEcoregion ecoregion, ClimateRecord[] dailyClimateRecords, int actualYear, double latitude)
        {
            this.Year = actualYear;

            this.TotalAnnualPrecip = 0.0;
            for (int d = 0; d < dailyClimateRecords.Length; d++)
            {
                this.DailyMinTemp[d] = dailyClimateRecords[d].AvgMinTemp;
                this.DailyMaxTemp[d] = dailyClimateRecords[d].AvgMaxTemp;
                this.DailyVarTemp[d] = dailyClimateRecords[d].AvgVarTemp;
                this.DailyPptVarTemp[d] = dailyClimateRecords[d].AvgPptVarTemp;
                this.DailyPrecip[d] = dailyClimateRecords[d].AvgPpt;
                this.DailyPAR[d] = dailyClimateRecords[d].PAR;

                this.DailyTemp[d] = (this.DailyMinTemp[d] + this.DailyMaxTemp[d]) / 2.0;

                this.TotalAnnualPrecip += this.DailyPrecip[d];

                var hr = CalculateDayNightLength(d, latitude);
                this.DailyDayLength[d] = (3600.0 * hr);                  // seconds of daylight/day
                this.DailyNightLength[d] = (3600.0 * (24.0 - hr));         // seconds of nighttime/day
            }
        }

        private ClimateRecord[] AnnualClimate_AvgDaily(IEcoregion ecoregion, double latitude)
        {
            var dailyData = new ClimateRecord[365];     // year-averaged data are always of length 365, even if averaging includes leapyears.

            Dictionary<int, ClimateRecord[][]> timestepData;

            if (this.climatePhase == Climate.Phase.Future_Climate)
                timestepData = Climate.Future_AllData;
            else
                timestepData = Climate.Spinup_AllData;

            var yearCount = timestepData.Count;

            var feb28DayIndex = 31 + 28 - 1;  // zero-based so subtract 1

            for (var d = 0; d < 365; ++d)
            {
                var dailyMinTemp = 0.0;
                var dailyMaxTemp = 0.0;
                var dailyVarTemp = 0.0;
                var dailyPptVarTemp = 0.0;
                var dailyPrecip = 0.0;
                var dailyPAR = 0.0;

                // loop over years
                int dIndex;

                foreach (var yearDailyRecords in timestepData.Values)
                {
                    var yearRecords = yearDailyRecords[ecoregion.Index];

                    if (yearRecords.Length == 366 && d == feb28DayIndex)
                    {
                        // average data for both Feb28 and Feb29
                        dailyMinTemp += (yearRecords[d].AvgMinTemp + yearRecords[d + 1].AvgMinTemp) / 2.0;
                        dailyMaxTemp += (yearRecords[d].AvgMaxTemp + yearRecords[d + 1].AvgMaxTemp) / 2.0;
                        dailyVarTemp += (yearRecords[d].AvgVarTemp + yearRecords[d + 1].AvgVarTemp) / 2.0;
                        dailyPptVarTemp += (yearRecords[d].AvgPptVarTemp + yearRecords[d + 1].AvgPptVarTemp) / 2.0;
                        dailyPrecip += (yearRecords[d].AvgPpt + yearRecords[d + 1].AvgPpt) / 2.0;
                        dailyPAR += (yearRecords[d].PAR + yearRecords[d + 1].PAR) / 2.0;
                    }
                    else
                    {
                        // if it is a leapyear and the day is after Feb28, add one to the day index
                        dIndex = (yearRecords.Length == 366 && d > feb28DayIndex) ? d + 1 : d;

                        dailyMinTemp += yearRecords[dIndex].AvgMinTemp;
                        dailyMaxTemp += yearRecords[dIndex].AvgMaxTemp;
                        dailyVarTemp += yearRecords[dIndex].AvgVarTemp;
                        dailyPptVarTemp += yearRecords[dIndex].AvgPptVarTemp;
                        dailyPrecip += yearRecords[dIndex].AvgPpt;
                        dailyPAR += yearRecords[dIndex].PAR;
                    }
                }

                dailyData[d] = new ClimateRecord();
                if (yearCount > 0)
                {
                    dailyData[d].AvgMinTemp = dailyMinTemp / yearCount;
                    dailyData[d].AvgMaxTemp = dailyMaxTemp / yearCount;
                    dailyData[d].AvgVarTemp = dailyVarTemp / yearCount;
                    dailyData[d].StdDevTemp = Math.Sqrt(dailyVarTemp / yearCount);
                    dailyData[d].AvgPptVarTemp = dailyPptVarTemp / yearCount;
                    dailyData[d].AvgPpt = dailyPrecip / yearCount;
                    dailyData[d].StdDevPpt = Math.Sqrt(dailyPrecip / yearCount);
                    dailyData[d].PAR = dailyPAR / yearCount;
                }
            }

            return dailyData;
        }

        
        
        private int GetJulianMonthFromJulianDay(int yr, int mo, int d)
        {
            //System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
            System.Globalization.JulianCalendar jc = new System.Globalization.JulianCalendar();
            return jc.GetMonth(new DateTime(yr,mo,d,jc));
        }
                     

        //---------------------------------------------------------------------------
        private int CalculateBeginGrowingDay_Daily()
        //Calculate Begin Growing Degree Day (Last Frost; Minimum = 0 degrees C): 
        {
            double nightTemp = 0.0;
            int beginGrow = 162;
            for (int i = 1 ; i < 162; i++)  //Loop through all the days of the year from day 1 to day 162
            {
                nightTemp = this.DailyMinTemp[i];
                if (nightTemp > 0.0)
                {
                   // this.beginGrowing = i;
                    beginGrow = i;
                    break;
                }
            }
            //Climate.ModelCore.UI.WriteLine("  Calculating daily begin growing season day...{0}", beginGrow);

            return beginGrow;
           // return MaxDayInYear; // For the time being if no night could be find with the Temp. > 0 then 0 is returned. A result of this could be that no growth would occure.
        }

        //---------------------------------------------------------------------------
        private int CalculateEndGrowingDay_Daily(ClimateRecord[] annualClimate)//, Random autoRand)
        //Calculate End Growing Degree Day (First frost; Minimum = 0 degrees C):
        {
            double nightTemp = 0.0;
            //int beginGrowingDay = CalculateBeginGrowingDay_Daily(annualClimate);
            int endGrowingDay = MaxDayInYear;
            //int i = beginGrowingDay;
            for (int day = MaxDayInYear; day > this.BeginGrowing; day--)  //Loop through all the days of the year from day 1 to day 162
            {
                nightTemp = this.DailyMinTemp[day];
                if (nightTemp > 0)
                {
                    //this.endGrowing = i;
                    //endGrowingDay = i;
                    return day;
                }
            }
             
            return 0;
        }

       

         //---------------------------------------------------------------------------
        public int GrowSeasonDegreeDays()
        //Calc growing season degree days (Degree_Day) based on monthly temperatures
        //normally distributed around a specified mean with a specified standard
        //deviation.
        {
           
            
            //degDayBase is temperature (C) above which degree days (Degree_Day)
            //are counted
            double degDayBase = 5.56;      // 42F.

            double Deg_Days = 0.0;

            //Calc monthly temperatures (mean +/- normally distributed
            //random number times standard deviation) and
            //sum degree days for consecutve months.
            for (int i = 0; i < 12 ; i++) //12 months in year
            {
                //I talked to Melissa and Allec  and we decided to use the difference between Begin and End growing days for the GrowSeasonDegreeDays. 
                //if (DailyTemp[i] > degDayBase)
                    Deg_Days += (DailyTemp[i] - degDayBase);
            }
            this.growingDegreeDays = (int)Deg_Days;
            return (int)Deg_Days;
            
        }              
    }
}
