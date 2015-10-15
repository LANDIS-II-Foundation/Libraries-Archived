using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Library.Succession.DemographicSeeding
{
    /// <summary>
    /// Methods for working with the template for the paths of output maps.
    /// </summary>
    public static class MapPaths
    {
        public const string SpeciesVar  = "species";
        public const string TimestepVar = "timestep";

        private static IDictionary<string, bool> knownVars;
        private static IDictionary<string, string> varValues;

        //---------------------------------------------------------------------

        static MapPaths()
        {
            knownVars = new Dictionary<string, bool>();
            knownVars[SpeciesVar] = true;
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
                                                 int    timestep,
                                                 string species)
        {
            varValues[SpeciesVar] = species;
            varValues[TimestepVar] = timestep.ToString();
            return OutputPath.ReplaceTemplateVars(template, varValues);
        }
    }
}
