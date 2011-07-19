using Landis.Util;
using NUnit.Framework;

namespace Landis.Test.Util
{
	[TestFixture]
	public class Bool_Test
	{
		private string[] rows3x5AllTrue;
		private bool[,] array3x5AllTrue;

		private string[] rows3x5AllFalse;
		private bool[,] array3x5AllFalse;

		private string[] rowsMixed;
		private string rowsMixed_trueChars;
		private bool[,] arrayMixed;

		private string dataDir;

		//--------------------------------------------------------------------
		
		[SetUp]
		public void Init()
		{
			rows3x5AllTrue = new string[] { "TTTTT",
											"TTTTT",
											"TTTTT" };
			array3x5AllTrue = new bool[,] { {true, true, true, true, true},
											{true, true, true, true, true},
											{true, true, true, true, true} };
			rows3x5AllFalse = new string[] { "00000",
											 "00000",
											 "00000" };
			array3x5AllFalse = new bool[,] {
				{false, false, false, false, false},
				{false, false, false, false, false},
				{false, false, false, false, false}, };

									  //          1         2
                    				  // 12345678901234567890123456       row
			rowsMixed = new string[] {	"....|....|....|....|....|.",  //  1
										"....|XXXX|....|....|....|.",  //  2
										"...X+XXXX+....|XXXX+X...|.",  //  3
										"...X+XXXX+XX.X+XXXX+XX..|.",  //  4
										"....+XXXX+....|.XXX+X...|.",  //  5
										"....|....|....|....|....|.",  //  6
 										"....|.XXX+....|....|.....X",  //  7
										"....|XX..+X...|....|.XXX+X",  //  8
										"....|.X.X+XXXX+XXXX+XXXX|.",  //  9
										"....|.XXX+XX..+X..X+.XX.|.",  // 10
										"....|....|....|....|....|.",  // 11
										"....|....|....|....|....|.",  // 12
										"X...|....|....|....|....|.",  // 13
										".XXX+XXXX+XX..|....|...X|.",  // 14
										"XX.X+.XX.+XX..+X..X+.XXX+.",  // 15
										".XX.|...X+XXXX+XXXX+XX..|."}; // 16
			rowsMixed_trueChars = "X+";

			arrayMixed = new bool[rowsMixed.Length, rowsMixed[0].Length];
			for (int r = 0; r < rowsMixed.Length; ++r) {
				string row = rowsMixed[r];
				for (int c = 0; c < rowsMixed[0].Length; ++c)
					arrayMixed[r,c] = rowsMixed_trueChars.Contains(
														row.Substring(c,1));
			}

			dataDir = System.IO.Path.Combine(Data.Directory, "Bool");
		}

		//--------------------------------------------------------------------

		private void AssertDimensions(string[] rows,
		                              bool[,]  array)
		{
			int expectedRows = rows.Length;
			int expectedCols = rows.Length == 0 ? 0 : rows[0].Length;
			Assert.AreEqual(expectedRows, array.GetLength(0));
			Assert.AreEqual(expectedCols, array.GetLength(1));
		}

		//--------------------------------------------------------------------

		[Test]
		public void Make_0Rows()
		{
			string[] rows = new string[0];
			bool[,] array = Bool.Make2DimArray(rows, "");
			AssertDimensions(rows, array);
		}

		//--------------------------------------------------------------------
		
		[Test]
		public void Make_1Row0Columns()
		{
			string[] rows = new string[] { "" };
			bool[,] array = Bool.Make2DimArray(rows, "");
			AssertDimensions(rows, array);
		}

		//--------------------------------------------------------------------
		
		[Test]
		public void Make_1Row1Column_True()
		{
			string[] rows = new string[] { "T" };
			bool[,] array = Bool.Make2DimArray(rows, "TtYy1");
			AssertDimensions(rows, array);
			Assert.AreEqual(true, array[0,0]);
		}

		//--------------------------------------------------------------------
		
		[Test]
		public void Make_1Row1Column_False()
		{
			string[] rows = new string[] { "F" };
			bool[,] array = Bool.Make2DimArray(rows, "TtYy1");
			AssertDimensions(rows, array);
			Assert.AreEqual(false, array[0,0]);
		}

		//--------------------------------------------------------------------

		private void AssertArraysMatch(bool[,] expected,
		                               bool[,] actual)
		{
			Assert.AreEqual(expected.GetLength(0), actual.GetLength(0));
			Assert.AreEqual(expected.GetLength(1), actual.GetLength(1));

			for (int r = 0; r < expected.GetLength(0); ++r)
				for (int c = 0; c < expected.GetLength(1); ++c)
					Assert.AreEqual(expected[r,c], actual[r,c]);
		}

		//--------------------------------------------------------------------
		
		[Test]
		public void Make_3x5AllTrue()
		{
			string[] rows = rows3x5AllTrue;
			bool[,] array = Bool.Make2DimArray(rows, "TtYy1");
			AssertArraysMatch(array3x5AllTrue, array);
		}

		//--------------------------------------------------------------------
		
		[Test]
		public void Make_3x5AllFalse()
		{
			string[] rows = rows3x5AllFalse;
			bool[,] array = Bool.Make2DimArray(rows, "TtYy1");
			AssertArraysMatch(array3x5AllFalse, array);
		}

		//--------------------------------------------------------------------
		
		[Test]
		public void Make_MixedArray()
		{
			bool[,] array = Bool.Make2DimArray(rowsMixed, rowsMixed_trueChars);
			AssertArraysMatch(arrayMixed, array);
		}

		//--------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentException))]
		public void Make_DifferentRowLens()
		{
			try {
				string[] rows = new string[] { "ABC",
											   "123",
											   "abcdef" };
				bool[,] array = Bool.Make2DimArray(rows, "");
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		private bool[,] ReadBoolArray(string filename)
		{
			string path = System.IO.Path.Combine(dataDir, filename);
			return Bool.Read2DimArray(path);
		}

		//--------------------------------------------------------------------

		private void ReadWithPrintException(string filename)
		{
			try {
				bool[,] array = ReadBoolArray(filename);
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//--------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(StreamInputException))]
		public void Read_EmptyFile()
		{
			ReadWithPrintException("empty.txt");
		}

		//--------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(StreamInputException))]
		public void Read_NoTrueChars()
		{
			ReadWithPrintException("no-true-chars.txt");
		}

		//--------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(StreamInputException))]
		public void Read_NoTrueCharsB4FirstRow()
		{
			ReadWithPrintException("no-true-chars-b4-1st-row.txt");
		}

		//--------------------------------------------------------------------

		[Test]
		public void Read_0Rows()
		{
			bool[,] array = ReadBoolArray("no-rows.txt");
			Assert.AreEqual(0, array.GetLength(0));
			Assert.AreEqual(0, array.GetLength(1));
		}

		//--------------------------------------------------------------------

		[Test]
		public void Read_1Row0Columns()
		{
			bool[,] array = ReadBoolArray("1-row-0-columns.txt");
			Assert.AreEqual(1, array.GetLength(0));
			Assert.AreEqual(0, array.GetLength(1));
		}

		//--------------------------------------------------------------------

		[Test]
		public void Read_4Rows0Columns()
		{
			bool[,] array = ReadBoolArray("4-rows-0-columns.txt");
			Assert.AreEqual(4, array.GetLength(0));
			Assert.AreEqual(0, array.GetLength(1));
		}

		//--------------------------------------------------------------------

		[Test]
		public void Read_1x1True()
		{
			bool[,] array = ReadBoolArray("1x1-true.txt");
			Assert.AreEqual(1, array.GetLength(0));
			Assert.AreEqual(1, array.GetLength(1));
			Assert.AreEqual(true, array[0,0]);
		}

		//--------------------------------------------------------------------

		[Test]
		public void Read_1x1False()
		{
			bool[,] array = ReadBoolArray("1x1-false.txt");
			Assert.AreEqual(1, array.GetLength(0));
			Assert.AreEqual(1, array.GetLength(1));
			Assert.AreEqual(false, array[0,0]);
		}

		//--------------------------------------------------------------------
		
		[Test]
		public void Read_3x5AllTrue()
		{
			bool[,] array = ReadBoolArray("3x5-true.txt");
			AssertArraysMatch(array3x5AllTrue, array);
		}

		//--------------------------------------------------------------------
		
		[Test]
		public void Read_3x5AllTrue_RowNums()
		{
			bool[,] array = ReadBoolArray("3x5-true-row-#s.txt");
			AssertArraysMatch(array3x5AllTrue, array);
		}

		//--------------------------------------------------------------------
		
		[Test]
		public void Read_3x5AllFalse()
		{
			bool[,] array = ReadBoolArray("3x5-false.txt");
			AssertArraysMatch(array3x5AllFalse, array);
		}

		//--------------------------------------------------------------------
		
		[Test]
		public void Read_MixedArray()
		{
			bool[,] array = ReadBoolArray("mixed.txt");
			AssertArraysMatch(arrayMixed, array);
		}
	}
}
