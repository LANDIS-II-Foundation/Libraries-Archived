//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using System.Collections.Generic;

namespace Landis.Extension.Insects
{
    /// <summary>
    /// Methods for working with the template for map filenames.
    /// </summary>
    public static class MapNames
    {
        public const string InsectNameVar = "insectName";
        public const string TimestepVar = "timestep";

        private static IDictionary<string, bool> knownVars;
        private static IDictionary<string, string> varValues;

        //---------------------------------------------------------------------
        static MapNames()
        {
            knownVars = new Dictionary<string, bool>();
            knownVars[InsectNameVar] = true;
            knownVars[TimestepVar] = true;

            varValues = new Dictionary<string, string>();
        }

        //---------------------------------------------------------------------
        public static void CheckTemplateVars(string template)
        {
            OutputPath.CheckTemplateVars(template, knownVars);
        }

        //---------------------------------------------------------------------
        public static string ReplaceTemplateVars(string template,
                                                 string insectName,
                                                 int    timestep)
        {
            varValues[InsectNameVar] = insectName;
            varValues[TimestepVar] = timestep.ToString();
            return OutputPath.ReplaceTemplateVars(template, varValues);
        }
        //---------------------------------------------------------------------
        public static string ReplaceTemplateVars(string template,
                                                 string insectName)
        {
            varValues[InsectNameVar] = insectName;
            return OutputPath.ReplaceTemplateVars(template, varValues);
        }
    }
}
