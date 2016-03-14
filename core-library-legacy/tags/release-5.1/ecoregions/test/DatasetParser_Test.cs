using Landis.Ecoregions;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;

namespace Landis.Test.Ecoregions
{
	[TestFixture]
	public class DatasetParser_Test
	{
		private DatasetParser parser;
		private LineReader reader;
		private StringReader currentLine;

		private const string dataDirPlaceholder = "{data folder}";

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			parser = new DatasetParser();

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
				IDataset dataset = parser.Parse(reader);
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

		private IDataset ParseFile(string filename)
		{
			reader = OpenFile(filename);
			IDataset dataset = parser.Parse(reader);
			reader.Close();
			return dataset;
		}

		//---------------------------------------------------------------------

		[Test]
		public void EmptyTable()
		{
			IDataset dataset = ParseFile("EmptyTable.txt");
			Assert.AreEqual(0, dataset.Count);
		}

		//---------------------------------------------------------------------

		private void CompareDatasetAndFile(IDataset dataset,
		                                   string filename)
		{
			FileLineReader file = OpenFile(filename);
			InputLine inputLine = new InputLine(file);

			InputVar<string> LandisData = new InputVar<string>(Landis.Data.InputVarName);
			inputLine.ReadVar(LandisData);

			int expectedIndex = 0;
			foreach (IEcoregion ecoregion in dataset) {
				Assert.AreEqual(expectedIndex, ecoregion.Index);
				expectedIndex++;

				Assert.IsTrue(inputLine.GetNext());
				currentLine = new StringReader(inputLine.ToString());

				Assert.AreEqual(ReadValue<bool>(),   ecoregion.Active);
				Assert.AreEqual(ReadValue<byte>(),   ecoregion.MapCode);
				Assert.AreEqual(ReadValue<string>(), ecoregion.Name);
				Assert.AreEqual(ReadValue<string>(), ecoregion.Description);
			}
			Assert.IsFalse(inputLine.GetNext());
			file.Close();
		}

		//---------------------------------------------------------------------

		private T ReadValue<T>()
		{
			ReadMethod<T> method = InputValues.GetReadMethod<T>();
			int index;
			return method(currentLine, out index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void FullTable()
		{
			string filename = "FullTable.txt";
			IDataset dataset = ParseFile(filename);
			CompareDatasetAndFile(dataset, filename);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Active_InvalidYes()
		{
			TryParse("Active-InvalidYes.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Active_InvalidNo()
		{
			TryParse("Active-InvalidNo.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MapCode_Missing()
		{
			TryParse("MapCode-Missing.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MapCode_Invalid()
		{
			TryParse("MapCode-Invalid.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MapCode_Negative()
		{
			TryParse("MapCode-Negative.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MapCode_TooBig()
		{
			TryParse("MapCode-TooBig.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MapCode_Repeated()
		{
			TryParse("MapCode-Repeated.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Name_Missing()
		{
			TryParse("Name-Missing.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Name_Repeated()
		{
			TryParse("Name-Repeated.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Description_Missing()
		{
			TryParse("Description-Missing.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Description_NoEndQuote()
		{
			TryParse("Description-NoEndQuote.txt", 12);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ExtraText()
		{
			TryParse("ExtraText.txt", 12);
		}
	}
}
