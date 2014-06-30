using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Library.Climate
{
    public class InputLog
    {
        //[DataFieldAttribute(Desc = "Input Period")]
        //public string SimulationPeriod { set; get; }
        
        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Input Year")]
        public int Year {set; get;}

        [DataFieldAttribute(Desc = "Input Timestep")]
        public int Timestep { set; get; }

        //[DataFieldAttribute(Desc = "Input Day")]
        //public int Day { set; get; }

        [DataFieldAttribute(Desc = "Ecoregion Name")]
        public string EcoregionName { set; get; }

        [DataFieldAttribute(Desc = "Ecoregion Index")]
        public int EcoregionIndex { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.cm, Desc = "Precipitation", Format = "0.00")]
        public double ppt {get; set;}

        [DataFieldAttribute(Unit = FieldUnits.DegreeC, Desc = "Average Minimum Air Temperature", Format = "0.00")]
        public double min_airtemp { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.DegreeC, Desc = "Average Maximum Air Temperature", Format = "0.00")]
        public double max_airtemp { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.cm, Desc = "Standard Deviation Precipitation", Format = "0.00")]
        public double std_ppt { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.DegreeC, Desc = "Standard Deviation Temperature", Format = "0.00")]
        public double std_temp { get; set; }
    }
}
