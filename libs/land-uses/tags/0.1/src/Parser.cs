using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using System.Collections.Generic;

namespace Landis.Library.LandUses
{
    /// <summary>
    /// A parser that reads a table of land uses.
    /// </summary>
    public class Parser
        : Landis.TextParser<IList<LandUse>>
    {
        public override string LandisDataValue
        {
            get {
                return "Land Uses";
            }
        }

        //---------------------------------------------------------------------

        public Parser()
        {
        }

        //---------------------------------------------------------------------

        protected override IList<LandUse> Parse()
        {
            ReadLandisDataVar();

            List<LandUse> landUses = new List<LandUse>();

            Dictionary <string, int> nameLineNumbers = new Dictionary<string, int>();

            InputVar<string> name = new InputVar<string>("Name");
            InputVar<bool> allowsHarvest = new InputVar<bool>("Allows Harvesting");

            while (! AtEndOfInput) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(name, currentLine);
                int lineNumber;
                if (nameLineNumbers.TryGetValue(name.Value.Actual, out lineNumber))
                    throw new InputValueException(name.Value.String,
                                                  "The name \"{0}\" was previously used on line {1}",
                                                  name.Value.Actual, lineNumber);
                else
                    nameLineNumbers[name.Value.Actual] = LineNumber;

                ReadValue(allowsHarvest, currentLine);

                CheckNoDataAfter("the " + allowsHarvest.Name + " column",
                                 currentLine);

                LandUse landUse = new LandUse(name.Value, allowsHarvest.Value);
                landUses.Add(landUse);

                GetNextLine();
            }

            return landUses;
        }
    }
}
