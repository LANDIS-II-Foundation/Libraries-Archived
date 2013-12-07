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
        public TemporalGranularity TimeStep { get { return this.timeStep; } }
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
            this.timeStep = ((this.format == "PRISM") ? TemporalGranularity.Monthly : TemporalGranularity.Daily);
            switch (this.format.ToLower())
            {
                case "gfdl_a1fi":
                {
                    this.maxTempTrigerWord = "maxtemp";
                    this.minTempTrigerWord = "mintemp";
                    this.precipTrigerWord = "ppt";
                    this.rhTrigerWord = "rh";
                    this.windSpeedTrigerWord = "windSpeed";
                    break;
                }
                case "prism":
                {
                    this.maxTempTrigerWord = "maxtemp";
                    this.minTempTrigerWord = "mintemp";
                    this.precipTrigerWord = "ppt";
                    this.rhTrigerWord = "rh";
                    this.windSpeedTrigerWord = "windSpeed";
                    break;
                }
                case "griddedobserv":
                {
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
