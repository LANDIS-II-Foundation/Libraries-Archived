//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Ben Sulman

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
//using Landis.Library.Succession;
using System.Collections.Generic;
using System.Diagnostics;

namespace Landis.Library.Climate
{
    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public class InputParameters
         : IInputParameters
       
    {
        private int timestep;
//        private SeedingAlgorithms seedAlg;

        private string climateConfigFile;
        private string climateFileFormat;
        private string climateFile;
        private string spinUpClimateFileFormat;
        private string spinUpClimateFile;



        //---------------------------------------------------------------------
        /// <summary>
        /// Timestep (years)
        /// </summary>
        //public int Timestep
        //{
        //    get {
        //        return timestep;
        //    }
        //    set {
        //        if (value < 0)
        //            throw new InputValueException(value.ToString(), "Timestep must be > or = 0");
        //        timestep = value;
        //    }
        //}

 

        //---------------------------------------------------------------------
        public string ClimateConfigFile
        {
            get
            {
                return climateConfigFile;
            }
            set
            {

                climateConfigFile = value;
            }
        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Path to the required file with climatedata.
        /// </summary>
        /// 

        public string ClimateFileFormat			
        {
            get
            {
                return climateFileFormat;
            }
            set
            {
                
                climateFileFormat = value;
            }
        }
        
        public string ClimateFile
        {
            get {
                return climateFile;
            }
            set {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                climateFile = value;
            }
        }

        public string SpinUpClimateFileFormat  
        {
            get
            {
                return spinUpClimateFileFormat;
            }
            set
            {

                spinUpClimateFileFormat = value;
            }
        }

        public string SpinUpClimateFile			
        {
            get
            {
                return spinUpClimateFile;
            }
            set
            {
                string path = value;
                if (spinUpClimateFileFormat != "no" && path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                spinUpClimateFile = value;
            }
        }
        //---------------------------------------------------------------------
       

    }
}
