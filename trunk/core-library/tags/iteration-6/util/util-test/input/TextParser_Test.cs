using Landis.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Util
{
	[TestFixture]
	public class TextParser_Test
	{
		// Local class that represents the result of test parser class
		public class ParseResult
		{
			public readonly int IntValue;
			public readonly float FloatValue;
			public readonly string StringValue;

			public ParseResult(int    i,
			                   float  f,
			                   string s)
			{
				this.IntValue = i;
				this.FloatValue = f;
				this.StringValue = s;
			}

			public override string ToString()
			{
				return string.Format("{0}    {1}   {2}",
				                     StringValue, FloatValue, IntValue);
			}
		}

		//---------------------------------------------------------------------

		// Local parser class for testing purposes
		//
		// Format of input:
		//
		//		IntVariable  {value}
		//		FloatVariable  {value}   << optional
		//		StringVariable  {value}
		//
		public class Parser
			: TextParser<ParseResult>
		{
			protected override ParseResult Parse()
			{
				InputVar<int> intVar = new InputVar<int>("IntVariable");
				ReadVar(intVar);

				float f = 0;
				InputVar<float> floatVar = new InputVar<float>("FloatVariable");
				if (ReadOptionalVar(floatVar))
					f = floatVar.Value.Actual;

				InputVar<string> strVar = new InputVar<string>("StringVariable");
				ReadVar(strVar);

				CheckNoDataAfter("the parameter \"" + strVar.Name + "\"");

				return new ParseResult(intVar.Value.Actual, f,
				                       strVar.Value.Actual);
			}
		}

		//---------------------------------------------------------------------

		private TextLineReader reader;
		private Parser parser;
		private TableParser tableParser;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			reader = null;
			parser = new Parser();
			tableParser = new TableParser();
		}

		//---------------------------------------------------------------------

		private TextLineReader MakeReader(params string[] lines)
		{
			if (lines == null)
				return new TextLineReader(null);
			else
				return new TextLineReader(lines);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntFloatStr()
		{
			reader = MakeReader("IntVariable -78",
			                    "FloatVariable  .099",
			                    "StringVariable /usr/local/bin");
			ParseResult result = parser.Parse(reader);
			Assert.IsNotNull(result);
			Assert.AreEqual(-78, result.IntValue);
			Assert.AreEqual(.099, result.FloatValue, .00001);
			Assert.AreEqual("/usr/local/bin", result.StringValue);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntStr()
		{
			reader = MakeReader("IntVariable -78",
			                    "StringVariable /usr/local/bin");
			ParseResult result = parser.Parse(reader);
			Assert.IsNotNull(result);
			Assert.AreEqual(-78, result.IntValue);
			Assert.AreEqual(0, result.FloatValue);
			Assert.AreEqual("/usr/local/bin", result.StringValue);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntFloatStr_BlankLines()
		{
			reader = MakeReader("IntVariable -78",
			                    "FloatVariable  .099",
			                    "StringVariable /usr/local/bin",
			                    "",
			                    " \t \n",
			                    "\n");
			ParseResult result = parser.Parse(reader);
			Assert.IsNotNull(result);
			Assert.AreEqual(-78, result.IntValue);
			Assert.AreEqual(.099, result.FloatValue, .00001);
			Assert.AreEqual("/usr/local/bin", result.StringValue);
		}

		//---------------------------------------------------------------------

		private void TryParse(params string[] lines)
		{
			reader = MakeReader(lines);
			try {
				ParseResult result = parser.Parse(reader);
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void NoLines()
		{
			TryParse(null);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void BlankLine()
		{
			TryParse(" ");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void FloatFirst()
		{
			TryParse("FloatVariable");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void WrongNameForStr()
		{
			TryParse("IntVariable 123",
			         "FloatVariable 1,234e-9",
			         "WrongName 0");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void WrongNameForStr_NoFloat()
		{
			TryParse("IntVariable 123",
			         "WrongName 0");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ExtraData()
		{
			TryParse("IntVariable -78",
			         "FloatVariable  .099",
			         "StringVariable /usr/local/bin",
			         "",
			         " \t \n",
			         "\n",
			         "  extra data \n",
			         "");
		}

		//---------------------------------------------------------------------

		// Another local parser class (for table)
		//
		// Format of input:
		//
		//		TableName
		//		>  StringVar    FloatVar    IntVar
		//		    {value}      {value}    {value}
		//			...
		//
		public class TableParser
			: TextParser<ParseResult[]>
		{
			protected override ParseResult[] Parse()
			{
				List<ParseResult> result = new List<ParseResult>();

				ReadName("TableName");

				InputVar<string> strVar = new InputVar<string>("StringVar");
				InputVar<float> floatVar = new InputVar<float>("FloatVar");
				InputVar<int> intVar = new InputVar<int>("IntVar");

				while (! AtEndOfInput) {
					StringReader reader = new StringReader(CurrentLine);
					strVar.ReadValue(reader);
					floatVar.ReadValue(reader);
					intVar.ReadValue(reader);

					result.Add(new ParseResult(intVar.Value.Actual,
					                           floatVar.Value.Actual,
					                           strVar.Value.Actual));
	
					CheckNoDataAfter("the " + intVar.Name + " column", reader);
					GetNextLine();
				}

				return result.ToArray();
			}
		}

		//---------------------------------------------------------------------

		private TextLineReader MakeTableReader(params string[] lines)
		{
			TextLineReader reader = MakeReader(lines);
			reader.SkipBlankLines = true;
			reader.CommentLineMarker = ">";
			reader.SkipCommentLines = true;
			return reader;
		}

		//---------------------------------------------------------------------

		[Test]
		public void Table_Empty()
		{
			reader = MakeTableReader("TableName");
			ParseResult[] result = tableParser.Parse(reader);
			Assert.AreEqual(0, result.Length);
		}

		//---------------------------------------------------------------------

		private void TryParseTable(params string[] lines)
		{
			reader = MakeTableReader(lines);
			try {
				ParseResult[] result = tableParser.Parse(reader);
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Table_MissingName()
		{
			TryParseTable("\n",
			              "\n");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Table_WrongName()
		{
			TryParseTable("\n",
			              "NotTableName\n");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Table_ExtraTextAfterName()
		{
			TryParseTable("\n",
			              "TableName  some unexpected extra text\n");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Table_3Rows()
		{
			ParseResult[] expectedResult = new ParseResult[3];
			expectedResult[0] = new ParseResult(1, -123.45f, "Maine");
			expectedResult[1] = new ParseResult(2, 0.00098f, "Hawaii");
			expectedResult[2] = new ParseResult(3, 7e+8f, "Florida");

			reader = MakeTableReader("TableName",
			                         "",
			                         "> StrVar   FloatVar   IntVar",
			                         expectedResult[0].ToString(),
			                         expectedResult[1].ToString(),
			                         expectedResult[2].ToString());
			ParseResult[] result = tableParser.Parse(reader);
			Assert.AreEqual(expectedResult.Length, result.Length);
			for (int i = 0; i < expectedResult.Length; ++i) {
				Assert.AreEqual(expectedResult[i].IntValue, result[i].IntValue);
				Assert.AreEqual(expectedResult[i].FloatValue, result[i].FloatValue);
				Assert.AreEqual(expectedResult[i].StringValue, result[i].StringValue);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Table_MissingFloat()
		{
			TryParseTable("\n",
			              "TableName",
			              "",
			              "> StrVar   FloatVar   IntVar",
			              "   blue       1.0     -12,345",
			              "   white");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Table_MissingInt()
		{
			TryParseTable("\n",
			              "TableName",
			              "",
			              "> StrVar   FloatVar   IntVar",
			              "   blue       1.0     -12,345",
			              "   white      -.009   ");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Table_ExtraColumn()
		{
			TryParseTable("\n",
			              "TableName",
			              "",
			              "> StrVar   FloatVar   IntVar",
			              "   blue       1.0     -12,345",
			              "   white      -.009    99        what's this?");
		}
	}
}
