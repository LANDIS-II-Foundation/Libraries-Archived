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
        public static float CheckBiomassParm(InputValue<float> newValue,
                                            float minValue,
                                            float maxValue)
        {
            float value = CheckBiomassParm<float>(newValue, minValue, maxValue);
            return value;
        }
        public static int CheckBiomassParm(InputValue<int> newValue,
                                            int minValue,
                                            int maxValue)
        {
            int value = CheckBiomassParm<int>(newValue, minValue, maxValue);
            return value;
        }
        public static double CheckBiomassParm(InputValue<double> newValue,
                                            double minValue,
                                            double maxValue)
        {
            double value = CheckBiomassParm<double>(newValue, minValue, maxValue);
            return value;
        }
        

        //---------------------------------------------------------------------
        public static double ToDouble<T>(T value)
        {
            // This function is needed because double.Parse(double.minValue) throws an error (value to big) so an exception is needed in case CheckBiomassParm
            // is called with maxValue of any numerical type
            if (typeof(T) == typeof(double) && value.ToString() == double.MinValue.ToString()) return double.MinValue;
            if (typeof(T) == typeof(double) && value.ToString() == double.MaxValue.ToString()) return double.MaxValue;
            
            return double.Parse(value.ToString());
        }
        public static T CheckBiomassParm<T>(T newValue,
                                            T minValue,
                                            T maxValue,
                                            string label = null)
        {
            if (newValue != null)
            {
                double d = ToDouble(newValue);
                double min = ToDouble(minValue);
                double max = ToDouble(maxValue);  
                if (d < min || d > max)
                {
                    if (label == null)
                    {
                        throw new InputValueException(newValue.ToString(),
                                                     "Input value {0} is not between {1:0.0} and {2:0.0}",
                                                     newValue, minValue, maxValue);
                    }
                    else
                    {
                        throw new InputValueException(newValue.ToString(),
                                                      "Input value for " + label + " {0} is not between {1:0.0} and {2:0.0}",
                                                      newValue, minValue, maxValue);
                    }
                }
            }
            return newValue;
        }
         
    }
}
