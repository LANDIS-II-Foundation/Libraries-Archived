//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Ben Sulman

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
//using Landis.Library.Succession;
using System.Collections.Generic;
using System;

namespace Landis.Library.Climate
{
    /// <summary>
    /// A parser that reads biomass succession parameters from text input.
    /// </summary>
    public class InputParametersParser
        : TextParser<IInputParameters>
    {
        private string landisDataValue;

        public override string LandisDataValue
        {
            get
            {
                return landisDataValue;  //"Climate Config";
            }
        }


        public static class Names
        {
            //public const string Timestep = "Timestep";


            public const string LandisData = "LandisData";
            public const string ClimateConfigFile = "ClimateConfigFile";
            public const string ClimateTimeSeries = "ClimateTimeSeries";
            public const string ClimateFile = "ClimateFile";
            public const string ClimateFileFormat = "ClimateFileFormat";
            public const string SpinUpClimateTimeSeries = "SpinUpClimateTimeSeries";
            public const string SpinUpClimateFile = "SpinUpClimateFile";
            public const string SpinUpClimateFileFormat = "SpinUpClimateFileFormat";

           
            
        }

        //---------------------------------------------------------------------

        //private IEcoregionDataset ecoregionDataset;
        //private ISpeciesDataset speciesDataset;
        //private Dictionary<string, int> speciesLineNums;
        //private InputVar<string> speciesName;

        //---------------------------------------------------------------------

        static InputParametersParser()
        {
            //InputParametersParser.Names.LandisData = "Climate Config";
             //LandisDataValue = "Climate Config";
            //SeedingAlgorithmsUtil.RegisterForInputValues();
            //RegisterForInputValues();

        }

        //---------------------------------------------------------------------

        public InputParametersParser()
        {
            this.landisDataValue = "Climate Config";

           
            //this.ecoregionDataset = PlugIn.ModelCore.Ecoregions;
            //this.speciesDataset = PlugIn.ModelCore.Species;
            //this.speciesLineNums = new Dictionary<string, int>();
            //this.speciesName = new InputVar<string>("Species");

            //Dynamic.InputValidation.Initialize();
        }

        //---------------------------------------------------------------------

        protected override IInputParameters Parse()
        {
            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            //if (landisData.Value.Actual != PlugIn.ExtensionName)
            //    throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", PlugIn.ExtensionName);
            if (landisData.Value.Actual != "Climate Config")
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", "Climate Config");

//            ReadLandisDataVar();

//            int numLitterTypes = 4;
//            int numFunctionalTypes = 25;

            //Parameters parameters = new Parameters(ecoregionDataset, speciesDataset, numLitterTypes, numFunctionalTypes);
            InputParameters parameters = new InputParameters();

            //InputVar<int> timestep = new InputVar<int>(Names.Timestep);
            //ReadVar(timestep);
            //parameters.Timestep = timestep.Value;

            //HashSet<string> climateTimeSeries_PossibleValues = new HashSet<string>(new string[] { "MothlyHistRandom", "DailyHistRandom", "MonthlyHistAverage", "DailyHistAverage", "MonthlyStandard", "DailyGCM" });
            string climateTimeSeries_PossibleValues = "MonthlyRandom, MonthlyAverage, DailyHistRandom, DailyHistAverage, MonthlyStandard, DailyGCM";

            //InputVar<string> climateConfigFile = new InputVar<string>(Names.ClimateConfigFile);
            //ReadVar(climateConfigFile);
            //parameters.ClimateConfigFile = climateConfigFile.Value;

            InputVar<string> climateTimeSeries = new InputVar<string>(Names.ClimateTimeSeries);
            ReadVar(climateTimeSeries);
            parameters.ClimateTimeSeries = climateTimeSeries.Value;


            InputVar<string> climateFile = new InputVar<string>(Names.ClimateFile);
            ReadVar(climateFile);
            parameters.ClimateFile = climateFile.Value;

            InputVar<string> climateFileFormat = new InputVar<string>(Names.ClimateFileFormat);
            ReadVar(climateFileFormat);
            parameters.ClimateFileFormat = climateFileFormat.Value;


            

            InputVar<string> spinUpClimateTimeSeries = new InputVar<string>(Names.SpinUpClimateTimeSeries);
            ReadVar(spinUpClimateTimeSeries);
            parameters.SpinUpClimateTimeSeries = spinUpClimateTimeSeries.Value;

            InputVar<string> spinUpClimateFile = new InputVar<string>(Names.SpinUpClimateFile);
            InputVar<string> spinUpClimateFileFormat = new InputVar<string>(Names.SpinUpClimateFileFormat);
            if (spinUpClimateTimeSeries.Value != "no")
            {
                ReadVar(spinUpClimateFile);
                parameters.SpinUpClimateFile = spinUpClimateFile.Value;

                ReadVar(spinUpClimateFileFormat);
                parameters.SpinUpClimateFileFormat = spinUpClimateFileFormat.Value;
            }
            else
            {
                GetNextLine();
                GetNextLine();
            }

            if (!climateTimeSeries_PossibleValues.ToLower().Contains(parameters.ClimateTimeSeries.ToLower()) || !climateTimeSeries_PossibleValues.ToLower().Contains(parameters.SpinUpClimateTimeSeries.ToLower()))
            {
                //Climate.ModelCore.UI.WriteLine("Error in parsing climate-generator input file: invalid value for ClimateTimeSeries provided. Possible values dould be: " + climateTimeSeries_PossibleValues);
                throw new ApplicationException("Error in parsing climate-generator input file: invalid value for ClimateTimeSeries provided. Possible values are: " + climateTimeSeries_PossibleValues);
            }
            
            return parameters; //.GetComplete();


        }
    }



}
