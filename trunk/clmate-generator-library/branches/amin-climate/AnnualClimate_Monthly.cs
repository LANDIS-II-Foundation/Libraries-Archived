using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Core;

namespace  Landis.Library.Climate
{
    public class AnnualClimate_Monthly: AnnualClimate
    {
        public double[] MonthlyTemp = new double[12];
        public double[] MonthlyMinTemp = new double[12];
        public double[] MonthlyMaxTemp = new double[12];
        public double[] MonthlyPrecip = new double[12];
        public double[] MonthlyPAR = new double[12];
        public double[] MonthlyVarTemp = new double[12];
        public double[] MonthlyPptVarTemp = new double[12];
        public int tempEcoIndex = -1;

        public double[] MonthlyPET = new double[12];  // Potential Evapotranspiration
        public double[] MonthlyVPD = new double[12];  // Vapor Pressure Deficit
        public double[] MonthlyNdeposition = new double[12];
        public double[] MonthlyDayLength = new double[12];
        public double[] MonthlyNightLength = new double[12];
        public int[] MonthlyGDD = new int[12];





        public AnnualClimate_Monthly(IEcoregion ecoregion, int actualYear, double latitude, ClimatePhase spinupOrfuture = ClimatePhase.Future_Climate, int timeStep = Int32.MinValue) //For Hist and Random timeStep arg should be passed
        {
            this.climatePhase = spinupOrfuture;


            if (Climate.AllData_granularity == TemporalGranularity.Daily && spinupOrfuture == ClimatePhase.Future_Climate)
            {
                this.AnnualCliamte_From_AnnualCliamte_Daily(ecoregion,  actualYear, latitude, spinupOrfuture,  timeStep);
                return;
            }

 
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
                else if (this.climatePhase == ClimatePhase.SpinUp_Climate)
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
                    throw new ClimateDataOutOfRangeException("Exception: The requested Time-step or ecoregion is out of range of the provided " + this.climatePhase.ToString() + " input file. This might happened because the number of provided climate data is not devisable to the number of specified time-steps or there is not enoght historic climate data to run the model for the specified duration in scenario file.", ex);
                }

                //Climate.ModelCore.Log.WriteLine("  Generate new annual climate:  Yr={0}, Eco={1}.", year, ecoregion.Name);
                Ecoregion = ecoregion;
                IClimateRecord[] ecoClimate = new IClimateRecord[12];

                this.Year = actualYear;
                this.AnnualPrecip = 0.0;
                this.AnnualN = 0.0;

                for (int mo = 0; mo < 12; mo++)
                {
                    //here
                    ecoClimate[mo] = Climate.TimestepData[ecoregion.Index, mo];
                    //ecoClimate[mo] = Climate.TimestepData[TimeStep, mo];

                    double MonthlyAvgTemp = (ecoClimate[mo].AvgMinTemp + ecoClimate[mo].AvgMaxTemp) / 2.0;

                    double standardDeviation = ecoClimate[mo].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0);

                    this.MonthlyTemp[mo] = MonthlyAvgTemp + standardDeviation;
                    this.MonthlyMinTemp[mo] = ecoClimate[mo].AvgMinTemp + standardDeviation;
                    this.MonthlyMaxTemp[mo] = ecoClimate[mo].AvgMaxTemp + standardDeviation;
                    this.MonthlyPrecip[mo] = Math.Max(0.0, ecoClimate[mo].AvgPpt + (ecoClimate[mo].StdDevPpt * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0)));
                    this.MonthlyPAR[mo] = ecoClimate[mo].PAR;

                    this.AnnualPrecip += this.MonthlyPrecip[mo];

                    if (this.MonthlyPrecip[mo] < 0)
                        this.MonthlyPrecip[mo] = 0;

                    double hr = CalculateDayNightLength(mo, latitude);
                    this.MonthlyDayLength[mo] = (60.0 * 60.0 * hr);                  // seconds of daylight/day
                    this.MonthlyNightLength[mo] = (60.0 * 60.0 * (24 - hr));         // seconds of nighttime/day

                    //this.DOY[mo] = DayOfYear(mo);
                }


                this.MonthlyPET = CalculatePotentialEvapotranspiration(ecoClimate);
                this.MonthlyVPD = CalculateVaporPressureDeficit(ecoClimate);
                this.MonthlyGDD = CalculatePnETGDD(this.MonthlyTemp, actualYear);

                this.BeginGrowing = CalculateBeginGrowingSeason(ecoClimate);
                this.EndGrowing = CalculateEndGrowingSeason(ecoClimate);
                this.GrowingDegreeDays = GrowSeasonDegreeDays(actualYear);

                for (int mo = 5; mo < 8; mo++)
                    this.JJAtemperature += this.MonthlyTemp[mo];
                this.JJAtemperature /= 3.0;
            }
            else
            {
                Climate.ModelCore.UI.WriteLine("Error in creating a new AnnualClimate: the There is an inconsistancy between the passed arguments and the parameters set up in the climate-input-file.");
                throw new ApplicationException("Error in creating a new AnnualClimate: the There is an inconsistancy between the passed arguments and the parameters set up in the climate-input-file.");
            }

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
                IClimateRecord[,] avgEcoClimate = new IClimateRecord[Climate.ModelCore.Ecoregions.Count, 12]; //Climate.AllData[0].Length returns ecoregions' count
                IClimateRecord[,] ecoClimateT = new IClimateRecord[Climate.ModelCore.Ecoregions.Count, 12];

                for (int i = 0; i < 12; i++)
                {
                    this.MonthlyMinTemp[i] = 0.0;
                    this.MonthlyMaxTemp[i] = 0.0;
                    this.MonthlyVarTemp[i] = 0.0;
                    this.MonthlyPptVarTemp[i] = 0.0;
                    this.MonthlyPrecip[i] = 0.0;
                    this.MonthlyPAR[i] = 0.0;

                }

                int allDataCount = 0;
                if (this.climatePhase == ClimatePhase.Future_Climate)
                    allDataCount = Climate.AllData.Count;
                else if (this.climatePhase == ClimatePhase.SpinUp_Climate)
                    allDataCount = Climate.Spinup_AllData.Count;

                for (int mo = 0; mo < 12; mo++)
                {

                    for (int stp = 0; stp < allDataCount; stp++)
                    {

                        if (this.climatePhase == ClimatePhase.Future_Climate)
                            Climate.TimestepData = Climate.AllData.ElementAt(stp).Value;
                        else if (this.climatePhase == ClimatePhase.SpinUp_Climate)
                            Climate.TimestepData = Climate.Spinup_AllData.ElementAt(stp).Value;

                        ecoClimateT[ecoregion.Index, mo] = Climate.TimestepData[ecoregion.Index, mo];
                        //avgEcoClimate = ecoClimateT;

                        this.MonthlyMinTemp[mo] += ecoClimateT[ecoregion.Index, mo].AvgMinTemp;
                        this.MonthlyMaxTemp[mo] += ecoClimateT[ecoregion.Index, mo].AvgMaxTemp;
                        this.MonthlyVarTemp[mo] += ecoClimateT[ecoregion.Index, mo].AvgVarTemp;
                        this.MonthlyPptVarTemp[mo] += ecoClimateT[ecoregion.Index, mo].AvgPptVarTemp;
                        this.MonthlyPrecip[mo] += ecoClimateT[ecoregion.Index, mo].AvgPpt;
                        this.MonthlyPAR[mo] += ecoClimateT[ecoregion.Index, mo].PAR;


                    }
                    this.MonthlyMinTemp[mo] = this.MonthlyMinTemp[mo] / allDataCount;
                    this.MonthlyMaxTemp[mo] = this.MonthlyMaxTemp[mo] / allDataCount;
                    this.MonthlyVarTemp[mo] = this.MonthlyVarTemp[mo] / allDataCount;
                    this.MonthlyPptVarTemp[mo] = this.MonthlyPptVarTemp[mo] / allDataCount;
                    this.MonthlyPrecip[mo] = this.MonthlyPrecip[mo] / allDataCount;
                    this.MonthlyPAR[mo] = this.MonthlyPAR[mo] / allDataCount;
                    avgEcoClimate[ecoregion.Index, mo] = new ClimateRecord();
                    avgEcoClimate[ecoregion.Index, mo].AvgMinTemp = this.MonthlyMinTemp[mo];
                    avgEcoClimate[ecoregion.Index, mo].AvgMaxTemp = this.MonthlyMaxTemp[mo];
                    avgEcoClimate[ecoregion.Index, mo].AvgVarTemp = this.MonthlyVarTemp[mo];

                    avgEcoClimate[ecoregion.Index, mo].StdDevTemp = Math.Sqrt(MonthlyVarTemp[mo]);

                    avgEcoClimate[ecoregion.Index, mo].AvgPptVarTemp = this.MonthlyPptVarTemp[mo];
                    avgEcoClimate[ecoregion.Index, mo].AvgPpt = this.MonthlyPrecip[mo];
                    avgEcoClimate[ecoregion.Index, mo].StdDevPpt = Math.Sqrt(this.MonthlyPrecip[mo]);
                    avgEcoClimate[ecoregion.Index, mo].PAR = this.MonthlyPAR[mo];

                }


                if (this.climatePhase == ClimatePhase.Future_Climate)
                    avgEcoClimate_future = avgEcoClimate;
                else if (this.climatePhase == ClimatePhase.SpinUp_Climate)
                    avgEcoClimate_spinUp = avgEcoClimate;

                Climate.TimestepData = avgEcoClimate;

            }






            //TimeStep = timeStep;
            //Climate.TimestepData = Climate.AllData[TimeStep];
            ////Climate.ModelCore.Log.WriteLine("  Generate new annual climate:  Yr={0}, Eco={1}.", year, ecoregion.Name);
            Ecoregion = ecoregion;
            IClimateRecord[] ecoClimate = new IClimateRecord[12];
            this.Year = year;
            this.AnnualPrecip = 0.0;
            this.AnnualN = 0.0;

            for (int mo = 0; mo < 12; mo++)
            {
                ecoClimate[mo] = Climate.TimestepData[ecoregion.Index, mo];
                //ecoClimate[mo] = Climate.TimestepData[TimeStep, mo];

                double MonthlyAvgTemp = (ecoClimate[mo].AvgMinTemp + ecoClimate[mo].AvgMaxTemp) / 2.0;

                double standardDeviation = ecoClimate[mo].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0);

                this.MonthlyTemp[mo] = MonthlyAvgTemp + standardDeviation;
                this.MonthlyMinTemp[mo] = ecoClimate[mo].AvgMinTemp + standardDeviation;
                this.MonthlyMaxTemp[mo] = ecoClimate[mo].AvgMaxTemp + standardDeviation;


                this.MonthlyPrecip[mo] = Math.Max(0.0, ecoClimate[mo].AvgPpt + (ecoClimate[mo].StdDevPpt * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0)));
                this.MonthlyPAR[mo] = ecoClimate[mo].PAR;

                this.AnnualPrecip += this.MonthlyPrecip[mo];

                if (this.MonthlyPrecip[mo] < 0)
                    this.MonthlyPrecip[mo] = 0;

                double hr = CalculateDayNightLength(mo, latitude);
                this.MonthlyDayLength[mo] = (60.0 * 60.0 * hr);                  // seconds of daylight/day
                this.MonthlyNightLength[mo] = (60.0 * 60.0 * (24 - hr));         // seconds of nighttime/day

                //this.DOY[mo] = DayOfYear(mo);
            }


            this.MonthlyPET = CalculatePotentialEvapotranspiration(ecoClimate);
            this.MonthlyVPD = CalculateVaporPressureDeficit(ecoClimate);
            this.MonthlyGDD = CalculatePnETGDD(this.MonthlyTemp, year);

            this.BeginGrowing = CalculateBeginGrowingSeason(ecoClimate);
            this.EndGrowing = CalculateEndGrowingSeason(ecoClimate);
            this.GrowingDegreeDays = GrowSeasonDegreeDays(year);

            for (int mo = 5; mo < 8; mo++)
                this.JJAtemperature += this.MonthlyTemp[mo];
            this.JJAtemperature /= 3.0;


        }
        private void AnnualClimate_Base(IEcoregion ecoregion, int year, double latitude)
        {
            //Climate.ModelCore.Log.WriteLine("  Generate new annual climate:  Yr={0}, Eco={1}.", year, ecoregion.Name);
            Ecoregion = ecoregion;
            IClimateRecord[] ecoClimate = new IClimateRecord[12];

            this.Year = year;
            this.AnnualPrecip = 0.0;
            this.AnnualN = 0.0;

            for (int mo = 0; mo < 12; mo++)
            {
                ecoClimate[mo] = Climate.TimestepData[ecoregion.Index, mo];

                double MonthlyAvgTemp = (ecoClimate[mo].AvgMinTemp + ecoClimate[mo].AvgMaxTemp) / 2.0;

                double standardDeviation = ecoClimate[mo].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0);

                this.MonthlyTemp[mo] = MonthlyAvgTemp + standardDeviation;
                this.MonthlyMinTemp[mo] = ecoClimate[mo].AvgMinTemp + standardDeviation;
                this.MonthlyMaxTemp[mo] = ecoClimate[mo].AvgMaxTemp + standardDeviation;
                this.MonthlyPrecip[mo] = Math.Max(0.0, ecoClimate[mo].AvgPpt + (ecoClimate[mo].StdDevPpt * (Climate.ModelCore.GenerateUniform() * 2.0 - 1.0)));
                this.MonthlyPAR[mo] = ecoClimate[mo].PAR;

                this.AnnualPrecip += this.MonthlyPrecip[mo];

                if (this.MonthlyPrecip[mo] < 0)
                    this.MonthlyPrecip[mo] = 0;

                double hr = CalculateDayNightLength(mo, latitude);
                this.MonthlyDayLength[mo] = (60.0 * 60.0 * hr);                  // seconds of daylight/day
                this.MonthlyNightLength[mo] = (60.0 * 60.0 * (24 - hr));         // seconds of nighttime/day

                //this.DOY[mo] = DayOfYear(mo);
            }


            this.MonthlyPET = CalculatePotentialEvapotranspiration(ecoClimate);
            this.MonthlyVPD = CalculateVaporPressureDeficit(ecoClimate);
            this.MonthlyGDD = CalculatePnETGDD(this.MonthlyTemp, year);

            this.BeginGrowing = CalculateBeginGrowingSeason(ecoClimate);
            this.EndGrowing = CalculateEndGrowingSeason(ecoClimate);
            this.GrowingDegreeDays = GrowSeasonDegreeDays(year);

            for (int mo = 5; mo < 8; mo++)
                this.JJAtemperature += this.MonthlyTemp[mo];
            this.JJAtemperature /= 3.0;


        }
        private void AnnualCliamte_From_AnnualCliamte_Daily(IEcoregion ecoregion,  int actualYear, double latitude, ClimatePhase spinupOrfuture,  int timeStep)
        {
            int nDays;
            int dayOfYear = 0;
            AnnualClimate_Daily annDaily = new AnnualClimate_Daily(ecoregion, actualYear, latitude, spinupOrfuture, timeStep); //for the same timeStep
            IClimateRecord[] ecoClimate = new IClimateRecord[12];
            for (int mo = 0; mo < 12; mo++)
            {
                ecoClimate[mo] = Climate.TimestepData[ecoregion.Index, mo];
                /*
                nDays = DaysInMonth(mo, actualYear);
                int dayOfMo = Convert.ToInt32(Climate.ModelCore.GenerateUniform() * nDays);

                MonthlyTemp[mo] = annDaily.DailyTemp[dayOfYear + dayOfMo];
                MonthlyMinTemp[mo] = annDaily.DailyMinTemp[dayOfYear + dayOfMo];
                MonthlyMaxTemp[mo] = annDaily.DailyMaxTemp[dayOfYear + dayOfMo];
                MonthlyPrecip[mo] = annDaily.DailyPrecip[dayOfYear + dayOfMo];
                MonthlyPAR[mo] = annDaily.DailyPAR[dayOfYear + dayOfMo];
                MonthlyVarTemp[mo] = annDaily.DailyVarTemp[dayOfYear + dayOfMo];
                MonthlyPptVarTemp[mo] = annDaily.DailyPptVarTemp[dayOfYear + dayOfMo];
                */
                

                
                nDays = DaysInMonth(mo, actualYear);
                for (int d=1; d <= nDays; d++)
                {
                    

                    MonthlyTemp[mo]+= annDaily.DailyTemp[dayOfYear];
                    MonthlyMinTemp[mo] += annDaily.DailyMinTemp[dayOfYear];
                    MonthlyMaxTemp[mo] += annDaily.DailyMaxTemp[dayOfYear];
                    MonthlyPrecip[mo] += annDaily.DailyPrecip[dayOfYear];
                    MonthlyPAR[mo] += annDaily.DailyPAR[dayOfYear];
                    MonthlyVarTemp[mo] += annDaily.DailyVarTemp[dayOfYear];
                    MonthlyPptVarTemp[mo] += annDaily.DailyPptVarTemp[dayOfYear];
                    
                }


                MonthlyTemp[mo] /= nDays;
                MonthlyMinTemp[mo] /= nDays;
                MonthlyMaxTemp[mo] /= nDays;
                //MonthlyPrecip[mo] /= nDays;
                MonthlyPAR[mo] /= nDays;
                MonthlyVarTemp[mo] /= nDays;
                MonthlyPptVarTemp[mo] /= nDays;

                dayOfYear++;
                //dayOfYear += nDays;
                
            }

            this.BeginGrowing = annDaily.BeginGrowing;
            this.EndGrowing = annDaily.EndGrowing;
            //  this.GrowingDegreeDays = annDaily.gro GrowSeasonDegreeDays(actualYear);
            //-----------------------------------------
            //this.MonthlyPET = CalculatePotentialEvapotranspiration(ecoClimate);
            //this.MonthlyVPD = CalculateVaporPressureDeficit(ecoClimate);
            //this.MonthlyGDD = CalculatePnETGDD(this.MonthlyTemp, actualYear);

            //this.BeginGrowing = CalculateBeginGrowingSeason(ecoClimate);
            //this.EndGrowing = CalculateEndGrowingSeason(ecoClimate);
            //this.GrowingDegreeDays = GrowSeasonDegreeDays(actualYear);

            //for (int mo = 5; mo < 8; mo++)
            //    this.JJAtemperature += this.MonthlyTemp[mo];
            //this.JJAtemperature /= 3.0;
            //------------------------------------------
            
        }









        //---------------------------------------------------------------------------
        public override string Write()
        {
            string s = String.Format(
                " Climate:  Year={0}, Number GDD={1}." +
                " AnnualPpt={2:000.0}," +
                " JanMinTemp={3:0.0}," +
                " JanMaxTemp={4:0.0}," +
                " JanPpt={5:0.0}",
                this.Year, this.GrowingDegreeDays,
                TotalAnnualPrecip(), this.MonthlyMinTemp[0], this.MonthlyMaxTemp[0], this.MonthlyPrecip[0]);
            return s;
        }
        //---------------------------------------------------------------------------
        public void SetAnnualN(double Nslope, double Nintercept)
        {
            AnnualN = CalculateAnnualN(AnnualPrecip, Nslope, Nintercept);
            for (int mo = 0; mo < 12; mo++)
                MonthlyNdeposition[mo] = AnnualN * MonthlyPrecip[mo] / AnnualPrecip;

        }
        //---------------------------------------------------------------------------
        private static double CalculateAnnualN(double annualPrecip, double Nslope, double Nintercept)
        {
            //wet fixation , rain in cm, not mm
            //dry fixation , rain in cm, not mm

            double annualN = 0.0;

            annualN = Nintercept + Nslope * annualPrecip;

            return annualN;
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
            for (int i = 0; i < 12; i++) //12 months in year
            {
                if (MonthlyTemp[i] > degDayBase)
                    Deg_Days += (MonthlyTemp[i] - degDayBase) * DaysInMonth(i, currentYear);
            }
            return (int)Deg_Days;
        }


        //---------------------------------------------------------------------------
        public static int[] CalculatePnETGDD(double[] monthlyTemp, int currentYear)
        {
            //************************************************
            //  Heat Sum Routine
            //**********************

            int[] MonthlyGDD = new int[12];

            for (int i = 0; i < 12; i++) //12 months in year
            {
                double GDD = monthlyTemp[i] * DaysInMonth(i, currentYear);
                if (GDD < 0)
                    GDD = 0;
                MonthlyGDD[i] = (int)GDD;
                //GDDTot = GDDTot + GDD;
            }

            return MonthlyGDD;
        }


        //---------------------------------------------------------------------------
        private static int CalculateBeginGrowingSeason(IClimateRecord[] yearClimate)
        //Calculate Begin Growing Degree Day (Last Frost; Minimum = 0 degrees C):
        {

            double lastMonthMinTemp = yearClimate[0].AvgMinTemp;
            int dayCnt = 15;  //the middle of February
            int beginGrowingSeason = -1;

            for (int i = 1; i < 7; i++)  //Begin looking in February (1).  Should be safe for at least 100 years.
            {

                int totalDays = (DaysInMonth(i, 3) + DaysInMonth(i - 1, 3)) / 2;
                double MonthlyMinTemp = yearClimate[i].AvgMinTemp;// + (monthlyTempSD[i] * randVar.GenerateNumber());

                //Now interpolate between days:
                double degreeIncrement = System.Math.Abs(MonthlyMinTemp - lastMonthMinTemp) / (double)totalDays;
                double Tnight = MonthlyMinTemp;  //start from warmer month
                double TnightRandom = Tnight + (yearClimate[i].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2 - 1));

                for (int day = 1; day <= totalDays; day++)
                {
                    if (TnightRandom <= 0)
                        beginGrowingSeason = (dayCnt + day);
                    Tnight += degreeIncrement;  //work backwards to find last frost day.
                    TnightRandom = Tnight + (yearClimate[i].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2 - 1));
                }

                lastMonthMinTemp = MonthlyMinTemp;
                dayCnt += totalDays;  //new monthly mid-point
            }
            
            return beginGrowingSeason;
        }

        //---------------------------------------------------------------------------
        private static int CalculateEndGrowingSeason(IClimateRecord[] annualClimate)//, Random autoRand)
        //Calculate End Growing Degree Day (First frost; Minimum = 0 degrees C):
        {
            //Climate.ModelCore.NormalDistribution.Mu = 0.0;
            //Climate.ModelCore.NormalDistribution.Sigma = 1.0;
            //NormalRandomVar randVar = new NormalRandomVar(0, 1);

            //Defaults for the middle of July:
            double lastMonthTemp = annualClimate[6].AvgMinTemp;
            int dayCnt = 198;
            //int endGrowingSeason = 198;

            for (int i = 7; i < 12; i++)  //Begin looking in August.  Should be safe for at least 100 years.
            {
                int totalDays = (DaysInMonth(i, 3) + DaysInMonth(i - 1, 3)) / 2;
                double MonthlyMinTemp = annualClimate[i].AvgMinTemp;

                //Now interpolate between days:
                double degreeIncrement = System.Math.Abs(lastMonthTemp - MonthlyMinTemp) / (double)totalDays;
                double Tnight = lastMonthTemp;  //start from warmer month

                double TnightRandom = Tnight + (annualClimate[i].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2 - 1));

                for (int day = 1; day <= totalDays; day++)
                {
                    if (TnightRandom <= 0)
                        return (dayCnt + day);
                    Tnight -= degreeIncrement;  //work forwards to find first frost day.
                    TnightRandom = Tnight + (annualClimate[i].StdDevTemp * (Climate.ModelCore.GenerateUniform() * 2 - 1));
                    //Console.WriteLine("Tnight = {0}.", TnightRandom);
                }

                lastMonthTemp = MonthlyMinTemp;
                dayCnt += totalDays;  //new monthly mid-point
            }
            return 365;
        }


        //---------------------------------------------------------------------------
        private static double[] CalculateVaporPressureDeficit(IClimateRecord[] annualClimate)
        {
            // From PnET
            //Estimation of saturated vapor pressure from daily average temperature.

            //   calculates saturated vp and delta from temp
            //   from Murray J Applied Meteorol 6:203
            //   ?? are the < 0 equations from there also
            //   Tday    average air temperature, degC
            //   ES  saturated vapor pressure at Tday, kPa
            //   DELTA dES/dTA at TA, kPa/K which is the slope of the sat. vapor pressure curve
            //   Saturation equations are from:
            //       Murry, (1967). Journal of Applied Meteorology. 6:203.
            //
            //
            //
            double[] monthlyVPD = new double[12];

            for (int month = 0; month < 12; month++)
            {
                double Tmin = annualClimate[month].AvgMinTemp;
                double Tday = (annualClimate[month].AvgMinTemp + annualClimate[month].AvgMaxTemp) / 2.0;

                double es = 0.61078 * Math.Exp(17.26939 * Tday / (Tday + 237.3)); //kPa
                //double delta = 4098 * es / (Tday + 237.3) ^ 2.0;
                if (Tday < 0)
                {
                    es = 0.61078 * Math.Exp(21.87456 * Tday / (Tday + 265.5)); //kPa
                    //delta = 5808 * es / (Tday + 265.5) ^ 2
                }

                //Calculation of mean daily vapor pressure from minimum daily temperature.

                //   Tmin = minimum daily air temperature                  //degrees C
                //   emean = mean daily vapor pressure                     //kPa
                //   Vapor pressure equations are from:
                //       Murray (1967). Journal of Applied Meteorology. 6:203.

                double emean = 0.61078 * Math.Exp(17.26939 * Tmin / (Tmin + 237.3)); //kPa

                if (Tmin < 0)
                    emean = 0.61078 * Math.Exp(21.87456 * Tmin / (Tmin + 265.5));

                double VPD = es - emean;
                //if (VPD = 0)
                //  VPD = VPD;
                monthlyVPD[month] = VPD;
            }

            return monthlyVPD;
        }
        //---------------------------------------------------------------------------
        private static double[] CalculatePotentialEvapotranspiration(IClimateRecord[] annualClimate)
        {
            //Calculate potential evapotranspiration (pevap)
            //...Originally from pevap.f
            // FWLOSS(4) - Scaling factor for potential evapotranspiration (pevap).
            double waterLossFactor4 = 0.9;  //from Century v4.5


            // FINISH - from ecoregion data???
            double elev = 1.0;       //Definition?? Set elevation = 0???
            double sitlat = 0.0; // Site latitude???

            double highest = -40.0;
            double lowest = 100.0;

            for (int i = 0; i < 12; i++)
            {
                double avgTemp = (annualClimate[i].AvgMinTemp + annualClimate[i].AvgMaxTemp) / 2.0;
                highest = System.Math.Max(highest, avgTemp);
                lowest = System.Math.Min(lowest, avgTemp);
            }

            lowest = System.Math.Max(lowest, -10.0);

            //...Determine average temperature range
            double avgTempRange = System.Math.Abs(highest - lowest);

            double[] monthlyPET = new double[12];


            for (int month = 0; month < 12; month++)
            {

                //...Temperature range calculation
                double tr = annualClimate[month].AvgMaxTemp - System.Math.Max(-10.0, annualClimate[month].AvgMinTemp);

                double t = tr / 2.0 + annualClimate[month].AvgMinTemp;
                double tm = t + 0.006 * elev;
                double td = (0.0023 * elev) + (0.37 * t) + (0.53 * tr) + (0.35 * avgTempRange) - 10.9;
                double e = ((700.0 * tm / (100.0 - System.Math.Abs(sitlat))) + 15.0 * td) / (80.0 - t);
                double monpet = (e * 30.0) / 10.0;

                if (monpet < 0.5)
                    monpet = 0.5;

                //...fwloss(4) is a modifier for PET loss.   vek may90
                monthlyPET[month] = monpet * waterLossFactor4;

            }

            return monthlyPET;
        }


        //---------------------------------------------------------------------------
        public static double CalculateAnnualActualEvapotranspiration(AnnualClimate_Monthly annualClimate, double fieldCapacity)
        {
            // field capacity input as cm
            // variable with xVariableName indicate conversion to mm

            double xFieldCap = fieldCapacity * 10.0;

            double waterAvail = 0.0;
            double actualET = 0.0;
            double oldWaterAvail = 0.0;
            double accPotWaterLoss = 0.0;

            for (int month = 0; month < 12; month++)
            {

                double monthlyRain = annualClimate.MonthlyPrecip[month];
                double potentialET = annualClimate.MonthlyPET[month];


                //Calc potential water loss this month
                double potWaterLoss = monthlyRain - potentialET;

                //If monthlyRain doesn't satisfy potentialET, add this month's potential
                //water loss to accumulated water loss from soil
                if (potWaterLoss < 0.0)
                {
                    accPotWaterLoss += potWaterLoss;
                    double xAccPotWaterLoss = accPotWaterLoss * 10.0;

                    //Calc water retained in soil given so much accumulated potential
                    //water loss Pastor and Post. 1984.  Can. J. For. Res. 14:466:467.

                    waterAvail = fieldCapacity *
                                 System.Math.Exp((.000461 - 1.10559 / xFieldCap) * (-1.0 * xAccPotWaterLoss));

                    if (waterAvail < 0.0)
                        waterAvail = 0.0;

                    //changeSoilMoisture - during this month
                    double changeSoilMoisture = waterAvail - oldWaterAvail;

                    //Calc actual evapotranspiration (AET) if soil water is drawn down
                    actualET += (monthlyRain - changeSoilMoisture);
                }

                //If monthlyRain satisfies potentialET, don't draw down soil water
                else
                {
                    waterAvail = oldWaterAvail + potWaterLoss;
                    if (waterAvail >= fieldCapacity)
                        waterAvail = fieldCapacity;

                    double changeSoilMoisture = waterAvail - oldWaterAvail;

                    //If soil partially recharged, reduce accumulated potential
                    //water loss accordingly
                    accPotWaterLoss += changeSoilMoisture;

                    //If soil completely recharged, reset accumulated potential
                    //water loss to zero
                    if (waterAvail >= fieldCapacity)
                        accPotWaterLoss = 0.0;

                    //If soil water is not drawn upon, add potentialET to AET
                    actualET += potentialET;
                }

                oldWaterAvail = waterAvail;
            }

            return actualET;
        }

        //---------------------------------------------------------------------------
        public double MeanAnnualTemp(int currentYear)
        {
            double MAT = 0.0;
            //Calc monthly temperatures (mean +/- normally distributed
            //random number times  standard deviation) and
            //sum degree days for consecutve months.
            for (int i = 0; i < 12; i++) //12 months in year
            {
                int daysInMonth = DaysInMonth(i, currentYear);
                MAT += daysInMonth * MonthlyTemp[i];
            }

            if (currentYear % 4 == 0)
                MAT /= 366.0;
            else
                MAT /= 365.0;

            return MAT;
        }

        //---------------------------------------------------------------------------
        public double TotalAnnualPrecip()
        {
            //Main loop for yearly water balance calculation by month   */
            double TAP = 0.0;
            for (int i = 0; i < 12; i++)
            {
                TAP += MonthlyPrecip[i];
            }
            return TAP;
        }


    }
}
