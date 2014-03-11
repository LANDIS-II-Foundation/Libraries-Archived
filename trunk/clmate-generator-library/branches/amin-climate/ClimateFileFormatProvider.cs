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
            this.maxTempTrigerWord = "maxtemp";
            this.minTempTrigerWord = "mintemp";
            this.precipTrigerWord = "ppt";
            this.rhTrigerWord = "rh";
            this.windSpeedTrigerWord = "windSpeed";

            //this.timeStep = ((this.format == "PRISM") ? TemporalGranularity.Monthly : TemporalGranularity.Daily);
            switch (this.format.ToLower())
            {
                case "ipcc3_daily":  //was 'gfdl_a1fi'
                {
                    this.timeStep = TemporalGranularity.Daily;
                    break;
                }
                case "ipcc3_monthly":  //ADD
                {
                    this.timeStep = TemporalGranularity.Monthly;
                    break;
                }
                case "ipcc5_monthly":
                {
                    this.timeStep = TemporalGranularity.Monthly;
                    break;
                }
                case "ipcc5_daily":  //ADD
                {
                    this.timeStep = TemporalGranularity.Daily;
                    break;
                }
                case "prism_monthly":  //was 'prism'
                {
                    this.timeStep = TemporalGranularity.Monthly;
                    break;
                }
                case "Mauer_daily":  //was griddedobserved
                {
                    this.timeStep = TemporalGranularity.Daily;
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
