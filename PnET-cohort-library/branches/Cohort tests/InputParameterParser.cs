//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using System.Collections.Generic;
using System.Text;

namespace Landis.Extension.Insects
{
    /// <summary>
    /// A parser that reads the extension parameters from text input.
    /// </summary>
    public class InputParameterParser
        : TextParser<IInputParameters>
    {

        //---------------------------------------------------------------------
        public InputParameterParser()
        {
        }

        //---------------------------------------------------------------------
        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }
        }
        protected override IInputParameters Parse()
        {
            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != PlugIn.ExtensionName)
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", PlugIn.ExtensionName);

            InputParameters parameters = new InputParameters();

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            //----------------------------------------------------------
            // Read in Map and Log file names.

            InputVar<string> mapNames = new InputVar<string>("MapNames");
            ReadVar(mapNames);
            parameters.MapNamesTemplate = mapNames.Value;

            InputVar<string> logFile = new InputVar<string>("LogFile");
            ReadVar(logFile);
            parameters.LogFileName = logFile.Value;

            //----------------------------------------------------------
            // Last, read in Insect File names,
            // then parse the data in those files into insect parameters.

            InputVar<string> insectFileName = new InputVar<string>("InsectInputFiles");
            ReadVar(insectFileName);

            List<IInsect> insectParameterList = new List<IInsect>();
            InsectParser insectParser = new InsectParser();

            IInsect insectParameters =  Landis.Data.Load<IInsect>(insectFileName.Value,insectParser);
            insectParameterList.Add(insectParameters);

            while (!AtEndOfInput) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(insectFileName, currentLine);

                insectParameters =  Landis.Data.Load<IInsect>(insectFileName.Value,insectParser);

                insectParameterList.Add(insectParameters);

                GetNextLine();

            }

            foreach(IInsect activeInsect in insectParameterList)
            {
                if(insectParameters == null)
                    PlugIn.ModelCore.UI.WriteLine("   Biomass Insect:  Insect Parameters NOT loading correctly.");
                else
                    PlugIn.ModelCore.UI.WriteLine("Name of Insect = {0}", insectParameters.Name);

            }
            parameters.ManyInsect = insectParameterList;

            return parameters; 

        }
    }
}
