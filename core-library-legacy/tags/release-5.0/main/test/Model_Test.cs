using Edu.Wisc.Forest.Flel.Util;
using Edu.Wisc.Forest.Flel.Util.PlugIns;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Landis.Test.Main
{
	[TestFixture]
	public class Model_Test
	{
		private string originalWorkingDir;
		private TextWriter originalUIwriter;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			originalWorkingDir = Environment.CurrentDirectory;

			originalUIwriter = UI.TextWriter;
			UI.TextWriter = Data.Output;

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
		}

		//---------------------------------------------------------------------

		private void TryRun(string runFolder)
		{
			try {
				string runFolderPath = Data.MakeInputPath(Path.Combine("model-run",
				                                                       runFolder));
				Environment.CurrentDirectory = runFolderPath;
				Data.Output.WriteLine();
				Data.Output.WriteLine("Current directory: {0}",
				                      runFolderPath.Replace(Data.Directory, Data.DirPlaceholder));
				Model.Run("scenario.txt");
			}
			catch (System.Exception e) {
				Data.Output.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void ScenarioFile_NotExist()
		{
			TryRun("ScenarioFile_NotExist");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void SpeciesFile_NotExist()
		{
			TryRun("SpeciesFile_NotExist");
		}

		//---------------------------------------------------------------------

		private float ComputeCellArea(float cellLength)
		{
			return (float) (cellLength * cellLength / 10000);
		}

		//---------------------------------------------------------------------

		private void AssertCellLength(float cellLength)
		{
			Assert.AreEqual(cellLength, Model.CellLength);
			Assert.AreEqual(ComputeCellArea(cellLength), Model.CellArea);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void CellLength_WidthNotEqualHeight()
		{
			TryRun("CellLength_WidthNot=Height");
		}

		//---------------------------------------------------------------------

		[Test]
		public void CellLength_WidthNotEqualHeight_Warn()
		{
			TryRun("CellLength_WidthNot=Height_Warn");
			AssertCellLength(30);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void CellLength_Zero()
		{
			TryRun("CellLength_Zero");
		}

		//---------------------------------------------------------------------

		[Test]
		public void CellLength_Zero_Warn()
		{
			TryRun("CellLength_Zero_Warn");
			AssertCellLength(30);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void CellLength_Negative()
		{
			TryRun("CellLength_Negative");
		}

		//---------------------------------------------------------------------

		[Test]
		public void CellLength_Negative_Warn()
		{
			TryRun("CellLength_Negative_Warn");
			AssertCellLength(30);
		}

		//---------------------------------------------------------------------

		[Test]
		public void CellLength_NoUnits()
		{
			TryRun("CellLength_NoUnits");
			AssertCellLength(25);
		}

		//---------------------------------------------------------------------

		[Test]
		public void CellLength_Map25m()
		{
			TryRun("CellLength_Map25m");
			AssertCellLength(25);
		}

		//---------------------------------------------------------------------

		[Test]
		public void CellLength_MapAndScenarioSame()
		{
			TryRun("CellLength_MapAndScenarioSame");
			AssertCellLength(25);
		}

		//---------------------------------------------------------------------

		[Test]
		public void CellLength_MapAndScenarioDiffer()
		{
			TryRun("CellLength_MapAndScenarioDiffer");
			AssertCellLength(35);
		}

		//---------------------------------------------------------------------

		[Test]
		public void CellLength_Map100ft()
		{
			TryRun("CellLength_Map100ft");
			float cellLength = (float) (100.0 * 1200.0 / 3937);
			AssertCellLength(cellLength);
		}

		//---------------------------------------------------------------------

		[TestFixtureTearDown]
		public void TearDown()
		{
			UI.TextWriter = originalUIwriter;
			Environment.CurrentDirectory = originalWorkingDir;
		}
	}
}
