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
        private string maxTempTrigerWord;
        private string minTempTrigerWord;
        private string precipTrigerWord;
        private string rhTrigerWord;
        private string windSpeedTrigerWord;

        
            
        //------
        public TemporalGranularity InputTimeStep { get { return this.timeStep; } }
        public string MaxTempTrigerWord { get { return this.maxTempTrigerWord; } }
        public string MinTempTrigerWord { get { return this.minTempTrigerWord; } }
        public string PrecipTrigerWord { get { return this.precipTrigerWord; } }
        public string RhTrigerWord { get { return this.rhTrigerWord; } }
        public string WindSpeedTrigerWord { get { return this.windSpeedTrigerWord; } }
        public string SelectedFormat { get { return format; } }
  
        //------
        public ClimateFileFormatProvider(string format)
        {
            this.format = format;
            //this.timeStep = ((this.format == "PRISM") ? TemporalGranularity.Monthly : TemporalGranularity.Daily);
            switch (this.format.ToLower())
            {
                case "gfdl_a1fi":
                {
                    this.timeStep = TemporalGranularity.Daily;
                    this.maxTempTrigerWord = "maxtemp";
                    this.minTempTrigerWord = "mintemp";
                    this.precipTrigerWord = "ppt";
                    this.rhTrigerWord = "rh";
                    this.windSpeedTrigerWord = "windSpeed";
                    break;
                }
                case "ipcc5_monthly":
                {
                    this.timeStep = TemporalGranularity.Monthly;
                    this.maxTempTrigerWord = "maxtemp";
                    this.minTempTrigerWord = "mintemp";
                    this.precipTrigerWord = "ppt";
                    this.rhTrigerWord = "rh";
                    this.windSpeedTrigerWord = "windSpeed";
                    break;
                }
                case "prism":
                {
                    this.timeStep = TemporalGranularity.Monthly;
                    this.maxTempTrigerWord = "tmx";
                    this.minTempTrigerWord = "tmn";
                    this.precipTrigerWord = "ppt";
                    this.rhTrigerWord = "rh";
                    this.windSpeedTrigerWord = "windSpeed";
                    break;
                }
                case "griddedobserv":
                {
                    this.timeStep = TemporalGranularity.Daily;
                    this.maxTempTrigerWord = "TMax";
                    this.minTempTrigerWord = "TMin";
                    this.precipTrigerWord = "Prcp";
                    this.rhTrigerWord = "rh";
                    this.windSpeedTrigerWord = "windSpeed";
                    break;
                }
                default:
                {
                    Climate.ModelCore.UI.WriteLine("Error in ClimateFileFormatProvider: the given \"{0}\" file format is not supported.", this.format);
                    throw new ApplicationException("Error in ClimateFileFormatProvider: the given \"" + this.format + "\" file format is not supported.");
                    //break;
                }
            }
        }

       


    }
}
