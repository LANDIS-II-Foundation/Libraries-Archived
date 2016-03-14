using Edu.Wisc.Forest.Flel.Util;
using Edu.Wisc.Forest.Flel.Util.PlugIns;
using System.Collections.Generic;

namespace Landis.PlugIns
{
	public class DatabaseParser
		: Landis.TextParser< IList<IInfo> >
	{
		public override string LandisDataValue
		{
			get {
				return "Plug-ins Database";
			}
		}

		//---------------------------------------------------------------------

		public DatabaseParser()
		{
		}

		//---------------------------------------------------------------------

		protected override IList<IInfo> Parse()
		{
			ReadLandisDataVar();

			List<IInfo> plugIns = new List<IInfo>();

			Dictionary <string, int> lineNumbers = new Dictionary<string, int>();

			InputVar<string> name = new InputVar<string>("Name");
			InputVar<string> type = new InputVar<string>("Type");
			InputVar<string> implName = new InputVar<string>("Implementation Name");

			while (! AtEndOfInput) {
				StringReader currentLine = new StringReader(CurrentLine);

				int lineNumber;

				ReadValue(name, currentLine);
				if (lineNumbers.TryGetValue(name.Value.Actual, out lineNumber))
					throw new InputValueException(name.Value.String,
					                              "The name \"{0}\" was previously used on line {1}",
					                              name.Value.Actual, lineNumber);
				else
					lineNumbers[name.Value.Actual] = LineNumber;

				ReadValue(type, currentLine);
				System.Type interfaceType;
				string typeLower = type.Value.Actual.ToLower();
				if (typeLower == "succession")
					interfaceType = typeof(ISuccession);
				else if (typeLower == "disturbance")
					interfaceType = typeof(IDisturbance);
				else if (typeLower == "output")
					interfaceType = typeof(IOutput);
				else
					throw new InputValueException(type.Value.String,
					                              "Valid plug-in types are succession, disturbance and output.");

				ReadValue(implName, currentLine);

				CheckNoDataAfter("the " + implName.Name + " column",
				                 currentLine);

				plugIns.Add(new Info(name.Value.Actual, interfaceType,
				                     implName.Value.Actual));

				GetNextLine();
			}

			return plugIns;
		}
	}
}
