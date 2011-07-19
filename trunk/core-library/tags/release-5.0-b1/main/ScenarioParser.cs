using Edu.Wisc.Forest.Flel.Util;
using Landis.PlugIns;
using System.Collections.Generic;

namespace Landis
{
	/// <summary>
	/// A parser that reads a model scenario from text input.
	/// </summary>
	public class ScenarioParser
		: Landis.TextParser<IScenario>
	{
		private Dictionary<string, int> nameLineNumbers;

		//---------------------------------------------------------------------

		public override string LandisDataValue
		{
			get {
				return "Scenario";
			}
		}

		//---------------------------------------------------------------------

		public ScenarioParser()
		{
			nameLineNumbers = new Dictionary<string, int>();
		}

		//---------------------------------------------------------------------

		protected override IScenario Parse()
		{
			ReadLandisDataVar();

			IEditableScenario scenario = new EditableScenario();

			InputVar<int> duration = new InputVar<int>("Duration");
			ReadVar(duration);
			scenario.Duration = duration.Value;

			InputVar<string> species = new InputVar<string>("Species");
			ReadVar(species);
			scenario.Species = species.Value;

			InputVar<string> ecoregions = new InputVar<string>("Ecoregions");
			ReadVar(ecoregions);
			scenario.Ecoregions = ecoregions.Value;

			InputVar<string> ecoregionsMap = new InputVar<string>("EcoregionsMap");
			ReadVar(ecoregionsMap);
			scenario.EcoregionsMap = ecoregionsMap.Value;

			InputVar<float> cellLength = new InputVar<float>("CellLength");
			if (ReadOptionalVar(cellLength)) {
				scenario.CellLength = cellLength.Value;
			}

			InputVar<string> initCommunities = new InputVar<string>("InitialCommunities");
			ReadVar(initCommunities);
			scenario.InitialCommunities = initCommunities.Value;

			InputVar<string> communitiesMap = new InputVar<string>("InitialCommunitiesMap");
			ReadVar(communitiesMap);
			scenario.InitialCommunitiesMap = communitiesMap.Value;

            if (AtEndOfInput)
            	throw ExpectedPlugInException(typeof(ISuccession));
			ReadPlugIn(scenario.Succession);

            //  Disturbance plug-ins

            nameLineNumbers.Clear();  // Parse called more than once
			const string DisturbancesRandomOrder = "DisturbancesRandomOrder";

			while (! AtEndOfInput && CurrentName != DisturbancesRandomOrder
			                      && scenario.Outputs.Count == 0) {
				IEditablePlugIn<IDisturbance> disturbPlugIn;
				IEditablePlugIn<IOutput> outputPlugIn;
				ReadPlugIn(out disturbPlugIn, out outputPlugIn);
				if (disturbPlugIn != null)
					scenario.Disturbances.InsertAt(scenario.Disturbances.Count,
					                               disturbPlugIn);
				else
					scenario.Outputs.InsertAt(scenario.Outputs.Count,
					                          outputPlugIn);
			}

			//  Check for optional DisturbancesRandomOrder parameter
			InputVar<bool> randomOrder = new InputVar<bool>(DisturbancesRandomOrder);
			if (ReadOptionalVar(randomOrder))
				scenario.DisturbancesRandomOrder = randomOrder.Value;

            //  Output plug-ins

            if (scenario.Outputs.Count == 0)
            	nameLineNumbers.Clear();
            while (! AtEndOfInput) {
				IEditablePlugIn<IOutput> plugIn = new EditablePlugIn<IOutput>();
				ReadPlugIn(plugIn);
				scenario.Outputs.InsertAt(scenario.Outputs.Count, plugIn);
			}
            if (scenario.Outputs.Count == 0)
            	throw ExpectedPlugInException(typeof(IOutput));

            return scenario.GetComplete();
		}

		//---------------------------------------------------------------------

		private InputVariableException ExpectedPlugInException(System.Type plugInType)
		{
            return new InputVariableException(null, "Expected {0} plug-in.",
			                                  String.PrependArticle(Interface.GetName(plugInType)));
		}

		//---------------------------------------------------------------------

		private void ReadPlugIn<T>(IEditablePlugIn<T> plugIn)
			where T : Edu.Wisc.Forest.Flel.Util.PlugIns.IPlugIn
		{
			StringReader currentLine = new StringReader(CurrentLine);
			plugIn.Info = ReadPlugInName(currentLine);
			if (plugIn.Info.Actual.InterfaceType == typeof(PlugIns.IOutput))
				CheckForRepeatedName(plugIn.Info.Actual.Name);
			ReadInitFile(plugIn, currentLine);
			GetNextLine();
		}

		//---------------------------------------------------------------------

		private InputValue<Edu.Wisc.Forest.Flel.Util.PlugIns.Info> ReadPlugInName(StringReader currentLine)
		{
			InputVar<Edu.Wisc.Forest.Flel.Util.PlugIns.Info> plugIn = new InputVar<Edu.Wisc.Forest.Flel.Util.PlugIns.Info>("PlugIn");
			ReadValue(plugIn, currentLine);
			return plugIn.Value;
		}

		//---------------------------------------------------------------------

		private void ReadInitFile<T>(IEditablePlugIn<T> plugIn,
		                             StringReader       currentLine)
			where T : Edu.Wisc.Forest.Flel.Util.PlugIns.IPlugIn
		{
			InputVar<string> initFile = new InputVar<string>("InitializationFile");
			ReadValue(initFile, currentLine);
			plugIn.InitFile = initFile.Value;

			CheckNoDataAfter("the " + initFile.Name + " column",
			                 currentLine);
		}

		//---------------------------------------------------------------------

		private void ReadPlugIn(out IEditablePlugIn<IDisturbance> disturbPlugIn,
		                        out IEditablePlugIn<IOutput>      outputPlugIn)
		{
			disturbPlugIn = null;
			outputPlugIn = null;

			StringReader currentLine = new StringReader(CurrentLine);
			InputValue<Edu.Wisc.Forest.Flel.Util.PlugIns.Info> plugInInfo = ReadPlugInName(currentLine);
			if (plugInInfo.Actual.InterfaceType == typeof(IDisturbance)) {
				CheckForRepeatedName(plugInInfo.Actual.Name);
				disturbPlugIn = new EditablePlugIn<IDisturbance>();
				disturbPlugIn.Info = plugInInfo;
				ReadInitFile(disturbPlugIn, currentLine);
			}
			else if (plugInInfo.Actual.InterfaceType == typeof(IOutput)) {
				nameLineNumbers.Clear();
				CheckForRepeatedName(plugInInfo.Actual.Name);
				outputPlugIn = new EditablePlugIn<IOutput>();
				outputPlugIn.Info = plugInInfo;
				ReadInitFile(outputPlugIn, currentLine);
			}
			else
				throw new InputValueException(plugInInfo.String,
				                              "\"{0}\" is not a disturbance or output plug-in.",
				                              plugInInfo.Actual.Name);
			GetNextLine();
		}

		//---------------------------------------------------------------------

		private void CheckForRepeatedName(string name)
		{
			int lineNumber;
			if (nameLineNumbers.TryGetValue(name, out lineNumber))
				throw new InputValueException(name,
				                              "The plug-in \"{0}\" was previously used on line {1}",
				                              name, lineNumber);
			else
				nameLineNumbers[name] = LineNumber;
		}
	}
}
