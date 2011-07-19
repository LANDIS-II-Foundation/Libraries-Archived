using Landis.Species;
using Landis.Util;
using NUnit.Framework;

namespace Landis.Test.Species
{
	[TestFixture]
	public class DatasetParser_Test
	{
		private DatasetParser parser;
		private LineReader reader;
		private StringReader currentLine;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			parser = new DatasetParser();
		}

		//---------------------------------------------------------------------

		private FileLineReader OpenFile(string filename)
		{
			string path = System.IO.Path.Combine(Data.Directory, filename);
			return Landis.Data.OpenTextFile(path);
		}

		//---------------------------------------------------------------------

		private void TryParse(string filename)
		{
			try {
				reader = OpenFile(filename);
				IDataset dataset = parser.Parse(reader);
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
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
			TryParse("empty.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LandisData_WrongName()
		{
			TryParse("LandisData-WrongName.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LandisData_NoValue()
		{
			TryParse("LandisData-NoValue.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LandisData_MissingQuote()
		{
			TryParse("LandisData-MissingQuote.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LandisData_WrongValue()
		{
			TryParse("LandisData-WrongValue.txt");
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
			foreach (ISpecies species in dataset) {
				Assert.AreEqual(expectedIndex, species.Index);
				expectedIndex++;

				Assert.IsTrue(inputLine.GetNext());
				currentLine = new StringReader(inputLine.ToString());

				Assert.AreEqual(ReadValue<string>(), species.Name);
				Assert.AreEqual(ReadValue<int>(),    species.Longevity);
				Assert.AreEqual(ReadValue<int>(),    species.Maturity);
				Assert.AreEqual(ReadValue<byte>(),   species.ShadeTolerance);
				Assert.AreEqual(ReadValue<byte>(),   species.FireTolerance);
				Assert.AreEqual(ReadValue<byte>(),   species.WindTolerance);
				Assert.AreEqual(ReadValue<int>(),    species.EffectiveSeedDist);
				Assert.AreEqual(ReadValue<int>(),    species.MaxSeedDist);
				Assert.AreEqual(ReadValue<float>(),  species.VegReprodProb);
				Assert.AreEqual(ReadValue<int>(),    species.MaxSproutAge);
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
		public void NameRepeated()
		{
			TryParse("NameRepeated.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LongevityMissing()
		{
			TryParse("LongevityMissing.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LongevityInvalid()
		{
			TryParse("LongevityInvalid.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LongevityNegative()
		{
			TryParse("LongevityNegative.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MaturityMissing()
		{
			TryParse("MaturityMissing.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MaturityInvalid()
		{
			TryParse("MaturityInvalid.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MaturityNegative()
		{
			TryParse("MaturityNegative.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MaturityTooBig()
		{
			TryParse("MaturityTooBig.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ShadeMissing()
		{
			TryParse("ShadeMissing.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ShadeInvalid()
		{
			TryParse("ShadeInvalid.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ShadeNegative()
		{
			TryParse("ShadeNegative.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ShadeZero()
		{
			TryParse("ShadeZero.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ShadeTooBig()
		{
			TryParse("ShadeTooBig.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void FireMissing()
		{
			TryParse("FireMissing.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void FireInvalid()
		{
			TryParse("FireInvalid.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void FireNegative()
		{
			TryParse("FireNegative.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void FireZero()
		{
			TryParse("FireZero.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void FireTooBig()
		{
			TryParse("FireTooBig.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void EffSeedMissing()
		{
			TryParse("EffSeedMissing.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void EffSeedInvalid()
		{
			TryParse("EffSeedInvalid.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void EffSeedNegative()
		{
			TryParse("EffSeedNegative.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MaxSeedMissing()
		{
			TryParse("MaxSeedMissing.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MaxSeedNegative()
		{
			TryParse("MaxSeedNegative.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MaxSeedLessThanEff()
		{
			TryParse("MaxSeedLessThanEff.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReprodProbMissing()
		{
			TryParse("ReprodProbMissing.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReprodProbNegative()
		{
			TryParse("ReprodProbNegative.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReprodProbTooBig()
		{
			TryParse("ReprodProbTooBig.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MaxSproutMissing()
		{
			TryParse("MaxSproutMissing.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MaxSproutNegative()
		{
			TryParse("MaxSproutNegative.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MaxSproutMoreThanLongevity()
		{
			TryParse("MaxSproutMoreThanLongevity.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ExtraAfterMaxSprout()
		{
			TryParse("ExtraAfterMaxSprout.txt");
		}
	}
}
