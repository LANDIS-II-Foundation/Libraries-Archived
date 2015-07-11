//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;


namespace Landis.Extension.Insects
{
    /// <summary>
    /// Parameters for the extension.
    /// </summary>
    public interface IInputParameters
    {
        /// <summary>
        /// Timestep (years)
        /// </summary>
        int Timestep
        {
            get;set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        string MapNamesTemplate
        {
            get;set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Name of log file.
        /// </summary>
        string LogFileName
        {
            get;set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// List of Insect Files
        /// </summary>
        List<IInsect> ManyInsect
        {
            get;set;
        }
    }
}

namespace Landis.Extension.Insects
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public class InputParameters
        : IInputParameters
    {
        private int timestep;
        private string mapNamesTemplate;
        private string logFileName;
        private List<IInsect> manyInsect;

        //---------------------------------------------------------------------
        /// <summary>
        /// Timestep (years)
        /// </summary>
        public int Timestep
        {
            get {
                return timestep;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        public string MapNamesTemplate
        {
            get {
                return mapNamesTemplate;
            }
            set {
                MapNames.CheckTemplateVars(value);
                mapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Name of log file.
        /// </summary>
        public string LogFileName
        {
            get {
                return logFileName;
            }
            set {
                // FIXME: check for null or empty path (value.Actual);
                logFileName = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// List of Insect Files
        /// </summary>
        public List<IInsect> ManyInsect
        {
            get {
                return manyInsect;
            }
            set {
                manyInsect = value;
            }
        }

        //---------------------------------------------------------------------
        public InputParameters()
        {
        }
    }
}
