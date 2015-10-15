using Edu.Wisc.Forest.Flel.Util;
using Landis.Cohorts;
using System.Collections.Generic;

namespace Landis.InitialCommunities
{
	/// <summary>
	/// A parser that reads a dataset of initial communities from text input.
	/// </summary>
	public class DatasetParser
		: Landis.TextParser<IDataset>
	{
		private int successionTimestep;
		private Species.IDataset speciesDataset;

		//---------------------------------------------------------------------

		public override string LandisDataValue
		{
			get {
				return "Initial Communities";
			}
		}

		//---------------------------------------------------------------------

		public DatasetParser(int              successionTimestep,
		                     Species.IDataset speciesDataset)
		{
			this.successionTimestep = successionTimestep;
			this.speciesDataset = speciesDataset;
		}

		//---------------------------------------------------------------------

		protected override IDataset Parse()
		{
			ReadLandisDataVar();

			Dataset dataset = new Dataset();

			InputVar<ushort> mapCode = new InputVar<ushort>("MapCode");
			InputVar<string> speciesName = new InputVar<string>("Species");
			InputVar<ushort> age = new InputVar<ushort>("Age");

			Dictionary <ushort, int> mapCodeLineNumbers = new Dictionary<ushort, int>();

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
				List<ISpeciesCohorts<AgeCohort.ICohort>> speciesCohortsList;
				speciesCohortsList = new List<ISpeciesCohorts<AgeCohort.ICohort>>();
				Dictionary <string, int> speciesLineNumbers = new Dictionary<string, int>();
				while (! AtEndOfInput && CurrentName != mapCode.Name) {
					StringReader currentLine = new StringReader(CurrentLine);

					ReadValue(speciesName, currentLine);
					Species.ISpecies species = speciesDataset[speciesName.Value.Actual];
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
						if (age.Value.Actual == 0)
							throw new InputValueException(age.Value.String,
							                              "Ages must be > 0.");
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

					ages = BinAges(ages);
					speciesCohortsList.Add(new AgeCohort.SpeciesCohorts(species, ages));
					
					GetNextLine();
				}

				dataset.Add(new Community(mapCode.Value.Actual,
				                          new AgeCohort.SiteCohorts(speciesCohortsList)));
			}

			return dataset;
		}

		//---------------------------------------------------------------------

		private List<ushort> BinAges(List<ushort> ages)
		{
			if (successionTimestep <= 0)
				return ages;

			ages.Sort();
			for (int i = 0; i < ages.Count; i++) {
				ushort age = ages[i];
				if (age % successionTimestep != 0)
					ages[i] = (ushort) (((age / successionTimestep) + 1) * successionTimestep);
			}

			//	Remove duplicates, by going backwards through list from last
			//	item to the 2nd item, comparing each item with the one before
			//	it.
			for (int i = ages.Count - 1; i >= 1; i--) {
				if (ages[i] == ages[i-1])
					ages.RemoveAt(i);
			}

			return ages;
		}
	}
}
