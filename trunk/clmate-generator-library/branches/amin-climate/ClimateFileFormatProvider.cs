using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Library.Climate
{
    public class ClimateFileFormatProvider
    {
        private string format;
        private TimeStep timeStep;
        private string maxTempTrigerWord;
        private string minTempTrigerWord;
        private string precipTrigerWord;

        //------
        public TimeStep TimeStep { get { return this.timeStep; } }
        public string MaxTempTrigerWord { get { return this.maxTempTrigerWord; } }
        public string MinTempTrigerWord { get { return this.minTempTrigerWord; } }
        public string PrecipTrigerWord { get { return this.precipTrigerWord; } }
  
        //------
        public ClimateFileFormatProvider(string format)
        {
            this.format = format;
            this.timeStep = ((this.format == "PRISM") ? TimeStep.Monthly : TimeStep.Daily);
            switch (this.format.ToLower())
            {
                case "gfdla_a1fi":
                {
                    this.maxTempTrigerWord = "maxtemp";
                    this.minTempTrigerWord = "mintemp";
                    this.precipTrigerWord = "ppt";
                    break;
                }
                case "prism":
                {
                    this.maxTempTrigerWord = "maxtemp";
                    this.minTempTrigerWord = "mintemp";
                    this.precipTrigerWord = "ppt";
                    break;
                }
                case "gridded_observ":
                {
                    this.maxTempTrigerWord = "TMax";
                    this.minTempTrigerWord = "TMin";
                    this.precipTrigerWord = "Prcp";
                    break;
                }
                default:
                {
                    Climate.ModelCore.UI.WriteLine("Error in ClimateFileFormatProvider: the given \"{0}\" file format is not supported.", this.format);
                    break;
                }
            }
        }

       


    }
}
