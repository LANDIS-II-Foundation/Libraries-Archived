using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Library.BiomassCohortsPnET
{
    public class Constants 
    {
        //public static float MaxNscFrac = 0.035F;
        public static string delim = ",";
        public static string ext = ".csv";
        public static float MC = 12;    // Molecular weight of C
        public static float MCO2 = 44; // Molecular weight of CO2
        public static int SecondsPerHour = 60 * 60;
        public static int billion = 1000000000;
        
        public enum Months
        {
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }

        public static int NrOfMonths = Enum.GetValues(typeof(Months)).GetLength(0);
         
    }
}
