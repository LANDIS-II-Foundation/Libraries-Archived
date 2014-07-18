//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller

using Landis.SpatialModeling;
using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Library.Biomass
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public static class CheckParms
    {
        // For compatibility with Edu.Wisc.Forest.flel.Util
        public static void CheckBiomassParm(InputValue<float> newValue,
                                            float minValue,
                                            float maxValue)
        {
            CheckBiomassParm<float>(newValue, minValue, maxValue);
        }
        public static void CheckBiomassParm(InputValue<int> newValue,
                                            int minValue,
                                            int maxValue)
        {
            CheckBiomassParm<int>(newValue, minValue, maxValue);
        }
        public static void CheckBiomassParm(InputValue<double> newValue,
                                            double minValue,
                                            double maxValue)
        {
            CheckBiomassParm<double>(newValue, minValue, maxValue);
        }
        

        //---------------------------------------------------------------------
        public static T CheckBiomassParm<T>(InputValue<T> newValue,
                                            T minValue,
                                            T maxValue,
                                            string label = "inputparameter")
        {
            if (newValue != null)
            {
                
                double d = double.Parse(newValue.Actual.ToString());
                double min = double.Parse(minValue.ToString());
                double max = double.Parse(maxValue.ToString());
                if (d < min || d > max)
                {

                    throw new InputValueException(newValue.String,
                                                  "Input value for " + label + "{0} is not between {1:0.0} and {2:0.0}",
                                                  newValue.String, minValue, maxValue);
                }
            }
            return newValue.Actual;
        }
         
    }
}
