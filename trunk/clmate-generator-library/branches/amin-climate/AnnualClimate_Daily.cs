using Landis.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Library.Climate
{
    public class AnnualClimate_Daily: AnnualClimate
    {
        private bool isLeapYear;

        public int MaxDayInYear { get{return isLeapYear? 366 : 365; } } // = 366;

        public double[] DailyTemp = new double[366];
        public double[] DailyMinTemp = new double[366];
        public double[] DailyMaxTemp = new double[366];
        public double[] DailyPrecip = new double[366];
        public double[] DailyPAR = new double[366];
        public double[] DailyVarTemp = new double[366];
        public double[] DailyPptVarTemp = new double[366];
        public int tempEcoIndex = -1;

        public double[] DailyPET = new double[366];  // Potential Evapotranspiration
        public double[] DailyVPD = new double[366];  // Vapor Pressure Deficit
        public double[] DailyNdeposition = new double[366];
        public double[] DailyDayLength = new double[366];
        public double[] DailyNightLength = new double[366];
        public int[] DailyGDD = new int[366];


        public AnnualClimate_Daily(IEcoregion ecoregion, int actualYear, double latitude, ClimatePhase spinupOrfuture = ClimatePhase.Future_Climate, int timeStep = Int32.MinValue) //For Hist and Random timeStep arg should be passed
        {

            this.climatePhase = spinupOrfuture;

            //if (timeStep == Int32.MinValue && !Climate.ConfigParameters.ClimateFileFormat.Contains("Average"))
            //{
            //    AnnualClimate_Base(ecoregion, actualYear, latitude); //The ordinary old AnnualClimate function. This has been left here so that there lagacy uses of AnnualClimate are still supported.
            //}
            //else if(Climate.ConfigParameters.ClimateFileFormat.Contains("Hist"))
            //{
            //if(imeStep == Int32.MinValue && (Climate.ConfigParameters.ClimateTimeSeries.ToLower().Contains("average") || Climate.ConfigParameters.SpinUpClimateTimeSeries.ToLower().Contains("average")))
            if (Climate.ConfigParameters.ClimateTimeSeries.ToLower().Contains("average") || Climate.ConfigParameters.SpinUpClimateTimeSeries.ToLower().Contains("average"))
            {
                if (this.climatePhase == ClimatePhase.Future_Climate)
                {
                    //if (avgEcoClimate_future == null || avgEcoClimate_future[ecoregion.Index, 0] == null)
                        AnnualClimate_Avg(ecoregion, actualYear, latitude);
                    //else
                        Climate.TimestepData = avgEcoClimate_future;
                }
                else if (this.climatePhase == ClimatePhase.SpinUp_Climate && !Climate.ConfigParameters.SpinUpClimateTimeSeries.ToLower().Contains("monthly"))
                {
                    //if (avgEcoClimate_spinUp == null || avgEcoClimate_spinUp[ecoregion.Index, 0] == null)
                        AnnualClimate_Avg(ecoregion, actualYear, latitude);
                    //else
                        Climate.TimestepData = avgEcoClimate_spinUp;
                }
                //Climate.TimestepData = avgEcoClimate;
            }
            //}
            else if (timeStep != Int32.MinValue) //it is Random or Historic
            {
                TimeStep = timeStep;
                try
                {
                    //Presumption: The RandSelectedTimeSteps_future has been filled out in Climate.Initialize()
                    if (this.climatePhase == ClimatePhase.Future_Climate)
                    {
                        if (Climate.ConfigParameters.ClimateFileFormat.ToLower().Contains("random"))// || Climate.ConfigParameters.SpinUpClimateTimeSeries.ToLower().Contains("random")) // a specific timeStep is provided but it points to an item in the preprocessed-randomly-selected-timesteps for returning the climate
                        {
                            if (Climate.RandSelectedTimeSteps_future == null)
                            {
                                Climate.ModelCore.UI.WriteLine("Error in creating new AnnualClimate: Climate library has not been initialized.");
                                throw new ApplicationException("Error in creating new AnnualClimate: Climate library has not been initialized.");
                            }
                            Climate.TimestepData = Climate.AllData.ElementAt(Climate.RandSelectedTimeSteps_future[TimeStep]).Value;
                            //Climate.TimestepData = Climate.AllData[Climate.RandSelectedTimeSteps_future[TimeStep]];
                        }
                        else //Historic
                        {
                            Climate.TimestepData = Climate.AllData.ElementAt(TimeStep).Value;
                        }

                    }
                    else if (this.climatePhase == ClimatePhase.SpinUp_Climate)
                    {
                        if (Climate.ConfigParameters.SpinUpClimateTimeSeries.ToLower().Contains("random"))
                        {
                            if (Climate.RandSelectedTimeSteps_spinup == null)
                            {
                                Climate.ModelCore.UI.WriteLine("Error in creating new AnnualClimate: Climate library has not been initialized.");
                                throw new ApplicationException("Error in creating new AnnualClimate: Climate library has not been initialized.");
                            }
                            Climate.TimestepData = Climate.Spinup_AllData.ElementAt(Climate.RandSelectedTimeSteps_spinup[TimeStep]).Value;
                        }
                        else //Historic
                        {
                            Climate.TimestepData = Climate.Spinup_AllData.ElementAt(TimeStep).Value;
                        }

                    }

                }
                catch (System.Collections.Generic.KeyNotFoundException ex)
                {
                    throw new ClimateDataOutOfRangeException("Exception: The requested Time-step or ecoregion is out of range of the provided " + this.climatePhase.ToString() + " input file. This might have happened because the number of provided climate data is not devisable to the number of specified time-steps or there is not enoght historic climate data to run the model for the specified duration in scenario file.", ex);
                }


               
                //Climate.ModelCore.Log.WriteLine("  Generate new annual climate:  Yr={0}, Eco={1}.", year, ecoregion.Name);
                //---------------------------------
                //---------------------------------
                ////Ecoregion = ecoregion;
                ////IClimateRecord[] ecoClimate = new IClimateRecord[MaxDayInYear];

                ////this.Year = actualYear;
                ////this.AnnualPrecip = 0.0;
                ////this.AnnualN = 0.0;

                ////for (int day = 0; day < MaxDayInYear; day++)
                ////{
                    
                ////        ecoClimate[day] = Climate.TimestepData[ecoregion.Index, day];
                    
                ////    //ecoClimate[day] = Climate.TimestepData[TimeStep, day];
                ////    if (ecoClimate[day] != null)
                ////    {
                ////        double DailyAvgTemp = (ecoClimate[day].AvgMinTemp + ecoClimate[day].AvgMaxTemp) / 2.0;

                ////        double standardDeviation = ecoClimate[day].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0);

                ////        this.DailyTemp[day] = DailyAvgTemp + standardDeviation;
                ////        this.DailyMinTemp[day] = ecoClimate[day].AvgMinTemp + standardDeviation;
                ////        this.DailyMaxTemp[day] = ecoClimate[day].AvgMaxTemp + standardDeviation;
                ////        this.DailyPrecip[day] = Math.Max(0.0, ecoClimate[day].AvgPpt + (ecoClimate[day].StdDevPpt * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0)));
                ////        this.DailyPAR[day] = ecoClimate[day].PAR;

                ////        this.AnnualPrecip += this.DailyPrecip[day];

                ////        if (this.DailyPrecip[day] < 0)
                ////            this.DailyPrecip[day] = 0;

                ////        double hr = CalculateDayNightLength(day, latitude);
                ////        this.DailyDayLength[day] = (60.0 * 60.0 * hr);                  // seconds of daylight/day
                ////        this.DailyNightLength[day] = (60.0 * 60.0 * (24 - hr));         // seconds of nighttime/day

                ////        //this.DOY[day] = DayOfYear(day);
                ////    }
                ////}
                //-------------------------------------

                
//I don't think PET, VPD or GDD need to be converted to a daily time step since Fire doesn't need them (I double-checked this morning).  Can't they just stay as monthly values since Century is the only one using them?
//But we do need to fix "CalculateBeginGrowingSeason" and "CalculateEndGrowingSeason", like Rob's screenshot.
/*
                this.MonthlyPET = CalculatePotentialEvapotranspiration(ecoClimate);
                this.MonthlyVPD = CalculateVaporPressureDeficit(ecoClimate);
                this.MonthlyGDD = CalculatePnETGDD(this.MonthlyTemp, actualYear);

                this.BeginGrowing = CalculateBeginGrowingSeason(ecoClimate);
                this.EndGrowing = CalculateEndGrowingSeason(ecoClimate);
                this.GrowingDegreeDays = GrowSeasonDegreeDays(actualYear);
                 this.BeginGrowing = CalculateBeginGrowingDay_Daily(ecoClimate);
            this.EndGrowing = CalculateEndGrowingDay_Daily(ecoClimate);
            this.GrowingDegreeDays = GrowSeasonDegreeDays(actualYear);
                for (int day = 5; day < 8; day++)
                    this.JJAtemperature += this.MonthlyTemp[day];
                this.JJAtemperature /= 3.0;
 */
            }
               
            else
            {
                Climate.ModelCore.UI.WriteLine("Error in creating a new AnnualClimate: the There is an inconsistancy between the passed arguments and the parameters set up in the climate-input-file.");
                throw new ApplicationException("Error in creating a new AnnualClimate: the There is an inconsistancy between the passed arguments and the parameters set up in the climate-input-file.");
            }






            Ecoregion = ecoregion;
            IClimateRecord[] ecoClimate = new IClimateRecord[MaxDayInYear];

            this.Year = actualYear;
            this.AnnualPrecip = 0.0;
            this.AnnualN = 0.0;

            for (int day = 0; day < MaxDayInYear; day++)
            {

                ecoClimate[day] = Climate.TimestepData[ecoregion.Index, day];

                //ecoClimate[day] = Climate.TimestepData[TimeStep, day];
                if (ecoClimate[day] != null)
                {
                    double DailyAvgTemp = (ecoClimate[day].AvgMinTemp + ecoClimate[day].AvgMaxTemp) / 2.0;

                    double standardDeviation = ecoClimate[day].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0);

                    this.DailyTemp[day] = DailyAvgTemp + standardDeviation;
                    this.DailyMinTemp[day] = ecoClimate[day].AvgMinTemp + standardDeviation;
                    this.DailyMaxTemp[day] = ecoClimate[day].AvgMaxTemp + standardDeviation;
                    this.DailyPrecip[day] = Math.Max(0.0, ecoClimate[day].AvgPpt + (ecoClimate[day].StdDevPpt * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0)));
                    this.DailyPAR[day] = ecoClimate[day].PAR;

                    this.AnnualPrecip += this.DailyPrecip[day];

                    if (this.DailyPrecip[day] < 0)
                        this.DailyPrecip[day] = 0;

                    double hr = CalculateDayNightLength(day, latitude);
                    this.DailyDayLength[day] = (60.0 * 60.0 * hr);                  // seconds of daylight/day
                    this.DailyNightLength[day] = (60.0 * 60.0 * (24 - hr));         // seconds of nighttime/day

                    //this.DOY[day] = DayOfYear(day);
                }
            }

            this.beginGrowing = CalculateBeginGrowingDay_Daily(ecoClimate);
            this.endGrowing = CalculateEndGrowingDay_Daily(ecoClimate);
            this.growingDegreeDays = GrowSeasonDegreeDays(actualYear);
            if (Climate.TimestepData.GetLength(1) > 365)
                this.isLeapYear = true;
            
                    }




        private void AnnualClimate_Avg(IEcoregion ecoregion, int year, double latitude)
        {
            // check average or random
            if (ecoregion.Index != tempEcoIndex)
            {
                tempEcoIndex = ecoregion.Index;
                //get average data and assign
                // get the average of altimesteps of current ecoregion
                //Climate.TimestepData = Climate.AllData[ecoregion.Index];
                IClimateRecord[,] avgEcoClimate = new IClimateRecord[Climate.ModelCore.Ecoregions.Count, MaxDayInYear]; //Climate.AllData[0].Length returns ecoregions' count
                IClimateRecord[,] ecoClimateT = new IClimateRecord[Climate.ModelCore.Ecoregions.Count, MaxDayInYear];

                for (int i = 0; i < MaxDayInYear; i++)
                {
                    this.DailyMinTemp[i] = 0.0;
                    this.DailyMaxTemp[i] = 0.0;
                    this.DailyVarTemp[i] = 0.0;
                    this.DailyPptVarTemp[i] = 0.0;
                    this.DailyPrecip[i] = 0.0;
                    this.DailyPAR[i] = 0.0;

                }

                int allDataCount = 0;
                if (this.climatePhase == ClimatePhase.Future_Climate)
                    allDataCount = Climate.AllData.Count;
                else if (this.climatePhase == ClimatePhase.SpinUp_Climate)
                    allDataCount = Climate.Spinup_AllData.Count;

                for (int day = 0; day < MaxDayInYear; day++)
                {

                    for (int stp = 0; stp < allDataCount; stp++)
                    {
                        if (this.climatePhase == ClimatePhase.Future_Climate)
                            Climate.TimestepData = Climate.AllData.ElementAt(stp).Value;
                        else if (this.climatePhase == ClimatePhase.SpinUp_Climate)
                            Climate.TimestepData = Climate.Spinup_AllData.ElementAt(stp).Value;
                        //try
                        //{
                            ecoClimateT[ecoregion.Index, day] = Climate.TimestepData[ecoregion.Index, day];
                        //}
                        //catch (IndexOutOfRangeException e)
                        //{
                            
                        //}
                            //avgEcoClimate = ecoClimateT;
                        if (ecoClimateT[ecoregion.Index, day] != null)
                        {
                            this.DailyMinTemp[day] += ecoClimateT[ecoregion.Index, day].AvgMinTemp;
                            this.DailyMaxTemp[day] += ecoClimateT[ecoregion.Index, day].AvgMaxTemp;
                            this.DailyVarTemp[day] += ecoClimateT[ecoregion.Index, day].AvgVarTemp;
                            this.DailyPptVarTemp[day] += ecoClimateT[ecoregion.Index, day].AvgPptVarTemp;
                            this.DailyPrecip[day] += ecoClimateT[ecoregion.Index, day].AvgPpt;
                            this.DailyPAR[day] += ecoClimateT[ecoregion.Index, day].PAR;
                        }
                    }
                    this.DailyMinTemp[day] = this.DailyMinTemp[day] / allDataCount;
                    this.DailyMaxTemp[day] = this.DailyMaxTemp[day] / allDataCount;
                    this.DailyVarTemp[day] = this.DailyVarTemp[day] / allDataCount;
                    this.DailyPptVarTemp[day] = this.DailyPptVarTemp[day] / allDataCount;
                    this.DailyPrecip[day] = this.DailyPrecip[day] / allDataCount; //This DailyPrecip avg is the historic average so the average should be taken as opposed to summing that up.
                    this.DailyPAR[day] = this.DailyPAR[day] / allDataCount;
                    avgEcoClimate[ecoregion.Index, day] = new ClimateRecord();
                    avgEcoClimate[ecoregion.Index, day].AvgMinTemp = this.DailyMinTemp[day];
                    avgEcoClimate[ecoregion.Index, day].AvgMaxTemp = this.DailyMaxTemp[day];
                    avgEcoClimate[ecoregion.Index, day].AvgVarTemp = this.DailyVarTemp[day];

                    avgEcoClimate[ecoregion.Index, day].StdDevTemp = Math.Sqrt(DailyVarTemp[day]);

                    avgEcoClimate[ecoregion.Index, day].AvgPptVarTemp = this.DailyPptVarTemp[day];
                    avgEcoClimate[ecoregion.Index, day].AvgPpt = this.DailyPrecip[day];
                    avgEcoClimate[ecoregion.Index, day].StdDevPpt = Math.Sqrt(this.DailyPrecip[day]);
                    avgEcoClimate[ecoregion.Index, day].PAR = this.DailyPAR[day];
                }

                if (this.climatePhase == ClimatePhase.Future_Climate)
                    avgEcoClimate_future = avgEcoClimate;
                else if (this.climatePhase == ClimatePhase.SpinUp_Climate)
                    avgEcoClimate_spinUp = avgEcoClimate;

                Climate.TimestepData = avgEcoClimate;

            }






//            //TimeStep = timeStep;
//            //Climate.TimestepData = Climate.AllData[TimeStep];
//            ////Climate.ModelCore.Log.WriteLine("  Generate new annual climate:  Yr={0}, Eco={1}.", year, ecoregion.Name);
//            //---------------------------------------
//            //---------------------------------------
//            Ecoregion = ecoregion;
//            IClimateRecord[] ecoClimate = new IClimateRecord[12];
//            this.Year = year;
//            this.AnnualPrecip = 0.0;
//            this.AnnualN = 0.0;

//            for (int day = 0; day < MaxDayInYear; day++)
//            {
//                ecoClimate[day] = Climate.TimestepData[ecoregion.Index, day];
//                //ecoClimate[day] = Climate.TimestepData[TimeStep, day];

//                double DailyAvgTemp = (ecoClimate[day].AvgMinTemp + ecoClimate[day].AvgMaxTemp) / 2.0;

//                double standardDeviation = ecoClimate[day].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0);

//                this.DailyTemp[day] = DailyAvgTemp + standardDeviation;
//                this.DailyMinTemp[day] = ecoClimate[day].AvgMinTemp + standardDeviation;
//                this.DailyMaxTemp[day] = ecoClimate[day].AvgMaxTemp + standardDeviation;


//                this.DailyPrecip[day] = Math.Max(0.0, ecoClimate[day].AvgPpt + (ecoClimate[day].StdDevPpt * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0)));
//                this.DailyPAR[day] = ecoClimate[day].PAR;

//                this.AnnualPrecip += this.DailyPrecip[day];

//                if (this.DailyPrecip[day] < 0)
//                    this.DailyPrecip[day] = 0;

//                double hr = CalculateDayNightLength(day, latitude);
//                this.DailyDayLength[day] = (60.0 * 60.0 * hr);                  // seconds of daylight/day
//                this.DailyNightLength[day] = (60.0 * 60.0 * (24 - hr));         // seconds of nighttime/day

//                //this.DOY[day] = DayOfYear(day);
//            }

//            /*
//                        this.MonthlyPET = CalculatePotentialEvapotranspiration(ecoClimate);
//                        this.MonthlyVPD = CalculateVaporPressureDeficit(ecoClimate);
//                        this.MonthlyGDD = CalculatePnETGDD(this.MonthlyTemp, year);
//            */
//            this.beginGrowing = CalculateBeginGrowingDay_Daily(ecoClimate);
//            this.endGrowing = CalculateEndGrowingDay_Daily(ecoClimate);
//            this.growingDegreeDays = GrowSeasonDegreeDays(year);

/////*
////            this.MonthlyPET = CalculatePotentialEvapotranspiration(ecoClimate);
////            this.MonthlyVPD = CalculateVaporPressureDeficit(ecoClimate);
////            this.MonthlyGDD = CalculatePnETGDD(this.MonthlyTemp, year);
////*/
////             this.BeginGrowing = CalculateBeginGrowingDay_Daily(ecoClimate);
////             this.EndGrowing = CalculateEndGrowingDay_Daily(ecoClimate);
////             this.GrowingDegreeDays = GrowSeasonDegreeDays(year);

            //---------------------------------------
            //---------------------------------------
            /*
            for (int day = 5; day < 8; day++)
                this.JJAtemperature += this.MonthlyTemp[day];
            this.JJAtemperature /= 3.0;
            */

        }
        private void AnnualClimate_Base(IEcoregion ecoregion, int year, double latitude)
        {
            //Climate.ModelCore.Log.WriteLine("  Generate new annual climate:  Yr={0}, Eco={1}.", year, ecoregion.Name);
            Ecoregion = ecoregion;
            IClimateRecord[] ecoClimate = new IClimateRecord[MaxDayInYear];

            this.Year = year;
            this.AnnualPrecip = 0.0;
            this.AnnualN = 0.0;

             for (int day = 0; day < MaxDayInYear; day++)
            {
                try
                {
                    ecoClimate[day] = Climate.TimestepData[ecoregion.Index, day];
                }
                catch
                {
                }
                if (ecoClimate[day] != null)
                {
                    double DailyAvgTemp = (ecoClimate[day].AvgMinTemp + ecoClimate[day].AvgMaxTemp) / 2.0;

                    double standardDeviation = ecoClimate[day].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0);

                    this.DailyTemp[day] = DailyAvgTemp + standardDeviation;
                    this.DailyMinTemp[day] = ecoClimate[day].AvgMinTemp + standardDeviation;
                    this.DailyMaxTemp[day] = ecoClimate[day].AvgMaxTemp + standardDeviation;
                    this.DailyPrecip[day] = Math.Max(0.0, ecoClimate[day].AvgPpt + (ecoClimate[day].StdDevPpt * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0)));
                    this.DailyPAR[day] = ecoClimate[day].PAR;

                    this.AnnualPrecip += this.DailyPrecip[day];

                    if (this.DailyPrecip[day] < 0)
                        this.DailyPrecip[day] = 0;

                    double hr = CalculateDayNightLength(day, latitude);
                    this.DailyDayLength[day] = (60.0 * 60.0 * hr);                  // seconds of daylight/day
                    this.DailyNightLength[day] = (60.0 * 60.0 * (24 - hr));         // seconds of nighttime/day

                    //this.DOY[day] = DayOfYear(day);
                }
            }
            
/*
            this.MonthlyPET = CalculatePotentialEvapotranspiration(ecoClimate);
            this.MonthlyVPD = CalculateVaporPressureDeficit(ecoClimate);
            this.MonthlyGDD = CalculatePnETGDD(this.MonthlyTemp, year);

            this.BeginGrowing = CalculateBeginGrowingSeason(ecoClimate);
            this.EndGrowing = CalculateEndGrowingSeason(ecoClimate);
            this.GrowingDegreeDays = GrowSeasonDegreeDays(year);

            for (int day = 5; day < 8; day++)
                this.JJAtemperature += this.MonthlyTemp[day];
            this.JJAtemperature /= 3.0;
*/

        }
        
        private int GetJulianMonthFromJulianDay(int yr, int mo, int d)
        {
            //System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
            System.Globalization.JulianCalendar jc = new System.Globalization.JulianCalendar();
            return jc.GetMonth(new DateTime(yr,mo,d,jc));
        }



       

        //---------------------------------------------------------------------------
        private int CalculateBeginGrowingDay_Daily(IClimateRecord[] annualClimate)
        //Calculate Begin Growing Degree Day (Last Frost; Minimum = 0 degrees C): 
        {
            double nightTemp = 0.0;
            for (int i = 1 ; i < MaxDayInYear; i++)  //Loop through all the days of the year from day 1 to day 162
            {
                nightTemp = this.DailyMinTemp[i];
                if (nightTemp > 0)
                {
                   // this.beginGrowing = i;
                    return i;
                }
            }
            return 0;
           // return MaxDayInYear; // For the time being if no night could be find with the Temp. > 0 then 0 is returned. A result of this could be that no growth would occure.
        }

        //---------------------------------------------------------------------------
        private int CalculateEndGrowingDay_Daily(IClimateRecord[] annualClimate)//, Random autoRand)
        //Calculate End Growing Degree Day (First frost; Minimum = 0 degrees C):
        {
            double nightTemp = 0.0;
            int beginGrowingDay = CalculateBeginGrowingDay_Daily(annualClimate);
            int endGrowingDay = MaxDayInYear;
            //int i = beginGrowingDay;
            for (int i = MaxDayInYear; i > beginGrowingDay; i--)  //Loop through all the days of the year from day 1 to day 162
            {
                nightTemp = this.DailyMinTemp[i];
                if (nightTemp > 0)
                {
                    //this.endGrowing = i;
                    //endGrowingDay = i;
                    return i;
                }
            }


            //while ( i < MaxDayInYear  )  //Loop through all the days of the year from day 1 to day 162 NOTE: it cannot iterate from MaxDayInYear because it might not be the first day after begin day which has the nightTemp < 0. 
            //{
                
            //        //i = i + 1;
            //        nightTemp = this.DailyMinTemp[i];
            //        if (nightTemp < 0 && (endGrowingDay - beginGrowingDay) < 30)
            //        {
            //            endGrowingDay = i+1;
            //            return endGrowingDay;
            //            break;
            //        }
         
                
                
                
                
                
            //    i++;
            //    }
                
            
            //if ((endGrowingDay - beginGrowingDay) < 30)
            //{
            //    Climate.ModelCore.UI.WriteLine("two few Growwing days: endGrowingDay - beginGrowingDay < 30.");
            //    throw new ApplicationException("two few Growwing days: endGrowingDay - beginGrowingDay < 30.");
                
            //}
            //Console.Write(endGrowingDay - beginGrowingDay);
            //Console.Read();
            //this.EndGrowing = 

            //this.endGrowing = endGrowingDay;
            return 0;
            //return endGrowingDay;





            //return endGrowingDay; // For the time being if no night could be find with the Temp. < 0 then 0 is returned. A result of this could be that no growth would occure.
        }

       

         //---------------------------------------------------------------------------
        public int GrowSeasonDegreeDays(int currentYear)
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

        //---------------------------------------------------------------------------
        public void WriteToLogFile()
        { 
            //(IEcoregion ecoregion, int actualYear, double latitude, ClimatePhase spinupOrfuture = ClimatePhase.Future_Climate, int timeStep = Int32.MinValue)
        }

    }
}
