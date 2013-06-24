//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Ben Sulman

//using Landis.Library.Succession;
using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Library.Climate
{
    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public interface IInputParameters
    {
        //int Timestep{ get;set;}
        //string LandisDataValue { }

        string ClimateFileFormat { get; set; }
        string ClimateFile { get; set; }
        string SpinUpClimateFileFormat { get; set; }
        string SpinUpClimateFile { get; set; }

 


        //---------------------------------------------------------------------



        
    }
}
