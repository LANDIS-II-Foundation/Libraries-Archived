using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Library.Climate
{
    public class InputLog
    {
        
        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Input Year")]
        public int Year {set; get;}

        [DataFieldAttribute(Desc = "Input Timestep")]
        public int Timestep { set; get; }

        [DataFieldAttribute(Desc = "Ecoregion Name")]
        public string EcoregionName { set; get; }

        [DataFieldAttribute(Desc = "Ecoregion Index")]
        public int EcoregionIndex { set; get; }

        [DataFieldAttribute(Desc = "Precipitation (units variable)", Format = "0.00")]
        public double ppt {get; set;}

        [DataFieldAttribute(Desc = "Average Minimum Air Temperature (units variable)", Format = "0.00")]
        public double min_airtemp { get; set; }

        [DataFieldAttribute(Desc = "Average Maximum Air Temperature (units variable)", Format = "0.00")]
        public double max_airtemp { get; set; }

        [DataFieldAttribute(Desc = "Standard Deviation Precipitation (units variable)", Format = "0.00")]
        public double std_ppt { get; set; }

        [DataFieldAttribute(Desc = "Standard Deviation Temperature (units variable)", Format = "0.00")]
        public double std_temp { get; set; }
    }
}
