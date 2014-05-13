//  Copyright: Portland State University 2009-2014
//  Authors:  Robert M. Scheller, Amin Almassian

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Library.Climate
{
    public class ClimateFileFormatProvider
    {
        private string format;
        private TemporalGranularity timeStep;
        private string maxTempTriggerWord;
        private string minTempTriggerWord;
        private string precipTriggerWord;
        private string rhTriggerWord;
        private string windSpeedTriggerWord;

        // JM: suggestion: place this value in a global static "constants" class (perhaps it already is), so there's no confusion about the value, e.g. -273 vs -273.15.
        private const double ABS_ZERO = -273.15;
            
        //------
        public TemporalGranularity InputTimeStep { get { return this.timeStep; } }
        public string MaxTempTriggerWord { get { return this.maxTempTriggerWord; } }
        public string MinTempTriggerWord { get { return this.minTempTriggerWord; } }
        public string PrecipTriggerWord { get { return this.precipTriggerWord; } }
        public string RhTriggerWord { get { return this.rhTriggerWord; } }
        public string WindSpeedTriggerWord { get { return this.windSpeedTriggerWord; } }
        public string SelectedFormat { get { return format; } }
  
        // JM: properties for transformations
        public double PrecipTransformation { get; private set; }
        public double TemperatureTransformation { get; private set; }

        //------
        public ClimateFileFormatProvider(string format)
        {
            this.format = format;

            // default trigger words
            this.maxTempTriggerWord = "maxtemp";
            this.minTempTriggerWord = "mintemp";
            this.precipTriggerWord = "ppt";
            this.rhTriggerWord = "rh";
            this.windSpeedTriggerWord = "windSpeed";

            // default transformations
            this.PrecipTransformation = 0.1;        // converts mm to cm.
            this.TemperatureTransformation = 0.0;   // assumes data is in degrees Celsius.

            // JM: TODO: determine trigger words for file formats to see if they are different from the default ones above.

            //this.timeStep = ((this.format == "PRISM") ? TemporalGranularity.Monthly : TemporalGranularity.Daily);
            switch (this.format.ToLower())
            {
                case "ipcc3_daily":  //was 'gfdl_a1fi'
                    this.timeStep = TemporalGranularity.Daily;
                    break;

                case "ipcc3_monthly":  //ADD
                    this.timeStep = TemporalGranularity.Monthly;
                    break;

                case "ipcc5_monthly":
                    this.timeStep = TemporalGranularity.Monthly;
                    this.TemperatureTransformation = ABS_ZERO;      // ipcc5 temp. data are in Kelvin.
                    this.PrecipTransformation = 8640.0;            // ipcc5 precip. data are in kg / m2 / sec.
                    break;

                case "ipcc5_daily":  //ADD
                    this.timeStep = TemporalGranularity.Daily;
                    this.TemperatureTransformation = ABS_ZERO;      // ipcc5 temp. data are in Kelvin.
                    this.PrecipTransformation = 8640.0;  
                    break;

                case "prism_monthly":  //was 'prism'
                    this.timeStep = TemporalGranularity.Monthly;
                    break;

                case "mauer_daily":  //was griddedobserved
                    this.timeStep = TemporalGranularity.Daily;
                    this.precipTriggerWord = "Prcp";
                    this.maxTempTriggerWord = "Tmax";
                    this.minTempTriggerWord = "Tmin";
                    // RH and wind speed for Mauer are the default trigger words
                    break;

                default:
                    Climate.ModelCore.UI.WriteLine("Error in ClimateFileFormatProvider: the given \"{0}\" file format is not supported.", this.format);
                    throw new ApplicationException("Error in ClimateFileFormatProvider: the given \"" + this.format + "\" file format is not supported.");
                    //break;
            }
        }

       


    }
}
