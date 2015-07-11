using Edu.Wisc.Forest.Flel.Util;
using Edu.Wisc.Forest.Flel.Util.PlugIns;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Main
{
	[TestFixture]
	public class ScenarioParser_Test
	{
		private ScenarioParser parser;
		private LineReader reader;

		private const string dataDirPlaceholder = "{data folder}";

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			parser = new ScenarioParser();

			List<IInfo> plugIns = new List<IInfo>();
			plugIns.Add(new Info("Age-only succession",
			                     typeof(PlugIns.ISuccession),
			                     null));
			string[] disturbancePlugIns = new string[] {"Null.Disturbance",
			                                            "Age-only.Wind",
			                                            "Age-only.Fire",
			                                            "Harvest" };
			foreach (string name in disturbancePlugIns)
				plugIns.Add(new Info(name, typeof(PlugIns.IDisturbance), null));
			string[] outputPlugIns = new string[] {"Test.DumpEcoregions",
			                                       "Test.DumpSpecies" };
			foreach (string name in outputPlugIns)
				plugIns.Add(new Info(name, typeof(PlugIns.IOutput), null));
			PlugIns.Manager.Initialize(plugIns);

			Data.Output.WriteLine("{0} = \"{1}\"", dataDirPlaceholder, Data.Directory);
			Data.Output.WriteLine();
		}

		//---------------------------------------------------------------------

		private FileLineReader OpenFile(string filename)
		{
			string path = System.IO.Path.Combine(Data.Directory, filename);
			return Landis.Data.OpenTextFile(path);
		}

		//---------------------------------------------------------------------

		private void TryParse(string filename,
		                      int    errorLineNum)
		{
			try {
				reader = OpenFile(filename);
				IScenario scenario = parser.Parse(reader);
			}
			catch (System.Exception e) {
				Data.Output.WriteLine(e.Message.Replace(Data.Directory, dataDirPlaceholder));
				LineReaderException lrExc = e as LineReaderException;
				if (lrExc != null)
					Assert.AreEqual(errorLineNum, lrExc.LineNumber);
				Data.Output.WriteLine();
				throw;
			}
			finally {
				reader.Close();
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Empty()
		{
			TryParse("empty.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LandisData_WrongName()
		{
			TryParse("LandisData-WrongName.txt", 3);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LandisData_NoValue()
		{
			TryParse("LandisData-NoValue.txt", 3);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LandisData_MissingQuote()
		{
			TryParse("LandisData-MissingQuote.txt", 3);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LandisData_WrongValue()
		{
			TryParse("LandisData-WrongValue.txt", 3);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Duration_Missing()
		{
			TryParse("Duration-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Duration_WrongName()
		{
			TryParse("Duration-WrongName.txt", 6);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Duration_MissingValue()
		{
			TryParse("Duration-MissingValue.txt", 6);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Duration_BadValue()
		{
			TryParse("Duration-BadValue.txt", 6);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Duration_Negative()
		{
			TryParse("Duration-Negative.txt", 6);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Duration_Extra()
		{
			TryParse("Duration-Extra.txt", 6);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Species_Missing()
		{
			TryParse("Species-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Species_WrongName()
		{
			TryParse("Species-WrongName.txt", 8);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Species_MissingValue()
		{
			TryParse("Species-MissingValue.txt", 8);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Species_Empty()
		{
			TryParse("Species-Empty.txt", 8);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Species_Whitespace()
		{
			TryParse("Species-Whitespace.txt", 8);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Species_Extra()
		{
			TryParse("Species-Extra.txt", 8);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Ecoregions_Missing()
		{
			TryParse("Ecoregions-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Ecoregions_WrongName()
		{
			TryParse("Ecoregions-WrongName.txt", 10);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Ecoregions_MissingValue()
		{
			TryParse("Ecoregions-MissingValue.txt", 10);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Ecoregions_Empty()
		{
			TryParse("Ecoregions-Empty.txt", 10);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Ecoregions_Whitespace()
		{
			TryParse("Ecoregions-Whitespace.txt", 10);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Ecoregions_Extra()
		{
			TryParse("Ecoregions-Extra.txt", 10);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void EcoregionsMap_Missing()
		{
			TryParse("EcoregionsMap-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void EcoregionsMap_WrongName()
		{
			TryParse("EcoregionsMap-WrongName.txt", 11);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void EcoregionsMap_MissingValue()
		{
			TryParse("EcoregionsMap-MissingValue.txt", 11);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void EcoregionsMap_Empty()
		{
			TryParse("EcoregionsMap-Empty.txt", 11);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void EcoregionsMap_Whitespace()
		{
			TryParse("EcoregionsMap-Whitespace.txt", 11);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void EcoregionsMap_Extra()
		{
			TryParse("EcoregionsMap-Extra.txt", 11);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunities_Missing()
		{
			TryParse("InitCommunities-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunities_WrongName()
		{
			TryParse("InitCommunities-WrongName.txt", 13);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunities_MissingValue()
		{
			TryParse("InitCommunities-MissingValue.txt", 13);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunities_Empty()
		{
			TryParse("InitCommunities-Empty.txt", 13);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunities_Whitespace()
		{
			TryParse("InitCommunities-Whitespace.txt", 13);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunities_Extra()
		{
			TryParse("InitCommunities-Extra.txt", 13);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunitiesMap_Missing()
		{
			TryParse("InitCommunitiesMap-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunitiesMap_WrongName()
		{
			TryParse("InitCommunitiesMap-WrongName.txt", 14);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunitiesMap_MissingValue()
		{
			TryParse("InitCommunitiesMap-MissingValue.txt", 14);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunitiesMap_Empty()
		{
			TryParse("InitCommunitiesMap-Empty.txt", 14);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunitiesMap_Whitespace()
		{
			TryParse("InitCommunitiesMap-Whitespace.txt", 14);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void InitCommunitiesMap_Extra()
		{
			TryParse("InitCommunitiesMap-Extra.txt", 14);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Succession_Missing()
		{
			TryParse("Succession-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Succession_BadString()
		{
			TryParse("Succession-BadString.txt", 20);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Succession_InvalidName()
		{
			TryParse("Succession-InvalidName.txt", 20);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Succession_Unknown()
		{
			TryParse("Succession-Unknown.txt", 20);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Succession_MissingFile()
		{
			TryParse("Succession-MissingFile.txt", 20);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Succession_Extra()
		{
			TryParse("Succession-Extra.txt", 20);
		}

		//---------------------------------------------------------------------

		private IScenario ParseFile(string filename)
		{
			reader = OpenFile(filename);
			IScenario scenario = parser.Parse(reader);
			reader.Close();
			return scenario;
		}

		//---------------------------------------------------------------------

		[Test]
		public void Disturbances_Zero()
		{
			IScenario scenario = ParseFile("Disturbances-Zero.txt");
			Assert.AreEqual(0, scenario.Disturbances.Length);
			Assert.IsFalse(scenario.DisturbancesRandomOrder);
			Assert.AreEqual(2, scenario.Outputs.Length);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Disturbances_ZeroRandom()
		{
			IScenario scenario = ParseFile("Disturbances-ZeroRandom.txt");
			Assert.AreEqual(0, scenario.Disturbances.Length);
			Assert.IsTrue(scenario.DisturbancesRandomOrder);
			Assert.AreEqual(2, scenario.Outputs.Length);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Disturbances_SuccessionName()
		{
			TryParse("Disturbances-SuccessionName.txt", 24);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Disturbances_RepeatedName()
		{
			TryParse("Disturbances-RepeatedName.txt", 27);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Outputs_None()
		{
			TryParse("Outputs-None.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Outputs_DisturbanceName()
		{
			TryParse("Outputs-DisturbanceName.txt", 26);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Outputs_RepeatedName()
		{
			TryParse("Outputs-RepeatedName.txt", 30);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ManyPlugIns()
		{
			IScenario scenario = ParseFile("ManyPlugIns.txt");
			Assert.AreEqual(4, scenario.Disturbances.Length);
			Assert.IsFalse(scenario.DisturbancesRandomOrder);
			Assert.AreEqual(2, scenario.Outputs.Length);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ManyPlugIns_Random()
		{
			IScenario scenario = ParseFile("ManyPlugIns-Random.txt");
			Assert.AreEqual(4, scenario.Disturbances.Length);
			Assert.IsTrue(scenario.DisturbancesRandomOrder);
			Assert.AreEqual(2, scenario.Outputs.Length);
		}
	}
}
