using Edu.Wisc.Forest.Flel.Util;
using Landis;
using System.Collections.Generic;

namespace Landis.InitialCommunities
{
	/// <summary>
	/// A parser that reads a dataset of initial communities from text input.
	/// </summary>
	public class DatasetParser
		: Landis.TextParser<IDataset>
	{
		public override string LandisDataValue
		{
			get {
				return "Initial Communities";
			}
		}

		//---------------------------------------------------------------------

		public DatasetParser()
		{
		}

		//---------------------------------------------------------------------

		protected override IDataset Parse()
		{
			ReadLandisDataVar();

			Dataset dataset = new Dataset();

			InputVar<byte> mapCode = new InputVar<byte>("MapCode");
			InputVar<string> speciesName = new InputVar<string>("Species");
			InputVar<ushort> age = new InputVar<ushort>("Age");

			Dictionary <byte, int> mapCodeLineNumbers = new Dictionary<byte, int>();

			while (! AtEndOfInput) {
				//  Read initial community
				int mapCodeLineNum = LineNumber;
				ReadVar(mapCode);
				int lineNumber;
				if (mapCodeLineNumbers.TryGetValue(mapCode.Value.Actual, out lineNumber))
					throw new InputValueException(mapCode.Value.String,
					                              "The map code {0} was previously used on line {1}",
					                              mapCode.Value.Actual, lineNumber);
				else
					mapCodeLineNumbers[mapCode.Value.Actual] = mapCodeLineNum;

				//  Read species and their ages
				List<ISpeciesCohorts<AgeOnly.ICohort>> speciesCohortsList;
				speciesCohortsList = new List<ISpeciesCohorts<AgeOnly.ICohort>>();
				Dictionary <string, int> speciesLineNumbers = new Dictionary<string, int>();
				while (! AtEndOfInput && CurrentName != mapCode.Name) {
					StringReader currentLine = new StringReader(CurrentLine);

					ReadValue(speciesName, currentLine);
					Species.ISpecies species = Model.Species[speciesName.Value.Actual];
					if (species == null)
						throw new InputValueException(speciesName.Value.String,
						                              "{0} is not a species name.",
						                              speciesName.Value.String);
					if (speciesLineNumbers.TryGetValue(species.Name, out lineNumber))
						throw new InputValueException(speciesName.Value.String,
						                              "The species {0} was previously used on line {1}",
						                              speciesName.Value.String, lineNumber);
					else
						speciesLineNumbers[species.Name] = LineNumber;

					//  Read ages
					List<ushort> ages = new List<ushort>();
					TextReader.SkipWhitespace(currentLine);
					while (currentLine.Peek() != -1) {
						ReadValue(age, currentLine);
						if (ages.Contains(age.Value.Actual))
							throw new InputValueException(age.Value.String,
							                              "The age {0} appears more than once.",
							                              age.Value.String);
						if (age.Value.Actual > species.Longevity)
							throw new InputValueException(age.Value.String,
							                              "The age {0} is more than longevity ({1}).",
							                              age.Value.String, species.Longevity);
						ages.Add(age.Value.Actual);
						TextReader.SkipWhitespace(currentLine);
					}
					if (ages.Count == 0)
						//  Try reading age which will throw exception
						ReadValue(age, currentLine);

					speciesCohortsList.Add(new AgeOnly.SpeciesCohorts(species, ages));
					
					GetNextLine();
				}

				dataset.Add(new Community(mapCode.Value.Actual,
				                          new AgeOnly.SiteCohorts(speciesCohortsList)));
			}

			return dataset;
		}
	}
}
