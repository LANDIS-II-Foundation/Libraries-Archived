using Landis.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Library.Climate
{
    public class AnnualClimate_Daily: AnnualClimate
    {
        public double[] DailyTemp = new double[365];
        public double[] DailyMinTemp = new double[365];
        public double[] DailyMaxTemp = new double[365];
        public double[] DailyPrecip = new double[365];
        public double[] DailyPAR = new double[365];
        public double[] DailyVarTemp = new double[365];
        public double[] DailyPptVarTemp = new double[365];
        public int tempEcoIndex = -1;

        public double[] DailyPET = new double[365];  // Potential Evapotranspiration
        public double[] DailyVPD = new double[365];  // Vapor Pressure Deficit
        public double[] DailyNdeposition = new double[365];
        public double[] DailyDayLength = new double[365];
        public double[] DailyNightLength = new double[365];
        public int[] DailyGDD = new int[365];


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
                            Climate.TimestepData = Climate.AllData[Climate.RandSelectedTimeSteps_future[TimeStep]];
                        }
                        else //Historic
                        {
                            Climate.TimestepData = Climate.AllData[TimeStep];
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
                            Climate.TimestepData = Climate.Spinup_AllData[Climate.RandSelectedTimeSteps_spinup[TimeStep]];
                        }
                        else //Historic
                        {
                            Climate.TimestepData = Climate.Spinup_AllData[TimeStep];
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
                            Climate.TimestepData = Climate.AllData[stp];
                        else if (this.climatePhase == ClimatePhase.SpinUp_Climate)
                            Climate.TimestepData = Climate.Spinup_AllData[stp];

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


    }
}
