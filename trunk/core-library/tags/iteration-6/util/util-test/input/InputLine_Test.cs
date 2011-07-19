using Landis.Util;
using NUnit.Framework;

namespace Landis.Test.Util
{
	[TestFixture]
	public class InputLine_Test
	{
		private InputVar<int> intVar;
		private InputVar<string> strVar;
		string longExtraText;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			intVar = new InputVar<int>("IntegerVariable");
			strVar = new InputVar<string>("String_Var");
			longExtraText = "1234567890 abcedefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ\n";
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void NullArgForCtor()
		{
			InputLine line = new InputLine(null);
		}

		//---------------------------------------------------------------------

		private void AssertLineAtEnd(InputLine line)
		{
			Assert.IsFalse(line);
			Assert.AreEqual(LineReader.EndOfInput, line.Number);
			Assert.IsNull(line.VariableName);
		}

		//---------------------------------------------------------------------

		private void AssertNoMoreLines(InputLine line)
		{
			Assert.IsFalse(line.GetNext());
			AssertLineAtEnd(line);
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

		private InputLine MakeInputLine(params string[] lines)
		{
			TextLineReader reader = MakeReader(lines);
			return new InputLine(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		public void NullInput()
		{
			InputLine line = MakeInputLine(null);

			Assert.AreEqual("", line.ToString());
			AssertLineAtEnd(line);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		private void AssertLineHasValue(InputLine line,
		                                int       lineNumber,
		                                string    varName)
		{
			Assert.IsTrue(line);
			Assert.AreEqual(lineNumber, line.Number);
			Assert.AreEqual(varName, line.VariableName);
		}

		//---------------------------------------------------------------------

		[Test]
		public void BlankLine()
		{
			InputLine line = MakeInputLine("");
			AssertLineHasValue(line, 1, "");
			Assert.AreEqual("", line.ToString());
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MatchName()
		{
			string varName = "Variable-Name";
			InputLine line = MakeInputLine(varName);
			AssertLineHasValue(line, 1, varName);
			Assert.AreEqual(varName, line.ToString());
			line.MatchName(varName);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MatchName_WhitespaceBefore()
		{
			string varName = "Variable-Name";
			InputLine line = MakeInputLine(" \t " + varName);
			AssertLineHasValue(line, 1, varName);
			line.MatchName(varName);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MatchName_WhitespaceAfter()
		{
			string varName = "Variable-Name";
			InputLine line = MakeInputLine(varName + " \n");
			AssertLineHasValue(line, 1, varName);
			line.MatchName(varName);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MatchName_Whitespace()
		{
			string varName = "Variable-Name";
			InputLine line = MakeInputLine(" \t " + varName + " \r\n");
			AssertLineHasValue(line, 1, varName);
			line.MatchName(varName);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		private void TryMatchName(InputLine line,
		                          string    name)
		{
			try {
				line.MatchName(name);
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MatchName_AtEnd()
		{
			InputLine line = MakeInputLine(null);
			AssertLineAtEnd(line);
			TryMatchName(line, "expected.variable.name");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MatchName_WrongName()
		{
			string varName = "Variable-Name";
			InputLine line = MakeInputLine(varName);
			AssertLineHasValue(line, 1, varName);
			TryMatchName(line, "expected.variable.name");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MatchName_ExtraText()
		{
			string varName = "Variable-Name";
			InputLine line = MakeInputLine(varName + " \t some extra text\n");
			AssertLineHasValue(line, 1, varName);
			TryMatchName(line, varName);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MatchName_LongExtraText()
		{
			string varName = "Variable-Name";
			InputLine line = MakeInputLine(varName + " " + longExtraText);
			AssertLineHasValue(line, 1, varName);
			TryMatchName(line, varName);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MatchName_WrongName_OptBefore()
		{
			string varName = "Variable-Name";
			InputLine line = MakeInputLine(varName);
			AssertLineHasValue(line, 1, varName);

			Assert.IsFalse(line.MatchOptionalName("optional.variable"));

			TryMatchName(line, "required.variable.name");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MatchName_WrongName_3OptBefore()
		{
			string varName = "Variable-Name";
			InputLine line = MakeInputLine(varName);
			AssertLineHasValue(line, 1, varName);

			Assert.IsFalse(line.MatchOptionalName("optional.var.1"));
			Assert.IsFalse(line.MatchOptionalName("optional.var.2"));
			Assert.IsFalse(line.MatchOptionalName("optional.var.3"));

			TryMatchName(line, "required.variable.name");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MatchOptName()
		{
			string varName = "Optional-Variable-Name";
			InputLine line = MakeInputLine(varName);
			AssertLineHasValue(line, 1, varName);
			Assert.IsTrue(line.MatchOptionalName(varName));
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MatchOptName_WhitespaceBefore()
		{
			string varName = "Optional-Variable-Name";
			InputLine line = MakeInputLine(" \t " + varName);
			AssertLineHasValue(line, 1, varName);
			Assert.IsTrue(line.MatchOptionalName(varName));
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MatchOptName_WhitespaceAfter()
		{
			string varName = "Optional-Variable-Name";
			InputLine line = MakeInputLine(varName + " \n");
			AssertLineHasValue(line, 1, varName);
			Assert.IsTrue(line.MatchOptionalName(varName));
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MatchOptName_Whitespace()
		{
			string varName = "Optional-Variable-Name";
			InputLine line = MakeInputLine(" \t " + varName + " \r\n");
			AssertLineHasValue(line, 1, varName);
			Assert.IsTrue(line.MatchOptionalName(varName));
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MatchOptName_AtEnd()
		{
			InputLine line = MakeInputLine(null);
			AssertLineAtEnd(line);
			Assert.IsFalse(line.MatchOptionalName("expected.variable.name"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void MatchOptName_Missing()
		{
			string varName = "Variable-Name";
			InputLine line = MakeInputLine(varName);
			AssertLineHasValue(line, 1, varName);
			Assert.IsFalse(line.MatchOptionalName("optional.variable"));
			line.MatchName(varName);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		private void TryMatchOptName(InputLine line,
		                             string    name)
		{
			try {
				line.MatchOptionalName(name);
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MatchOptName_ExtraText()
		{
			string varName = "Optional-Variable-Name";
			InputLine line = MakeInputLine(varName + " \t some extra text\n");
			AssertLineHasValue(line, 1, varName);
			TryMatchOptName(line, varName);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void MatchOptName_LongExtraText()
		{
			string varName = "Optional-Variable-Name";
			InputLine line = MakeInputLine(varName + " " + longExtraText);
			AssertLineHasValue(line, 1, varName);
			TryMatchOptName(line, varName);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadIntVar()
		{
			string valueAsStr = "345";
			InputLine line = MakeInputLine(intVar.Name + " " + valueAsStr);
			line.ReadVar(intVar);
			Assert.AreEqual(345, intVar.Value.Actual);
			Assert.AreEqual(valueAsStr, intVar.Value.String);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadIntVar_Comma()
		{
			string valueAsStr = "12,345,678";
			InputLine line = MakeInputLine(intVar.Name + " " + valueAsStr);
			line.ReadVar(intVar);
			Assert.AreEqual(12345678, intVar.Value.Actual);
			Assert.AreEqual(valueAsStr, intVar.Value.String);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadIntVar_WhitespaceBefore()
		{
			string valueAsStr = "345";
			InputLine line = MakeInputLine(" \t " + intVar.Name + " " + valueAsStr);
			line.ReadVar(intVar);
			Assert.AreEqual(345, intVar.Value.Actual);
			Assert.AreEqual(valueAsStr, intVar.Value.String);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadIntVar_WhitespaceAfter()
		{
			string valueAsStr = "345";
			InputLine line = MakeInputLine(intVar.Name + " " + valueAsStr + "\r \n");
			line.ReadVar(intVar);
			Assert.AreEqual(345, intVar.Value.Actual);
			Assert.AreEqual(valueAsStr, intVar.Value.String);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadIntVar_Whitespace()
		{
			string valueAsStr = "345";
			InputLine line = MakeInputLine(" \t " + intVar.Name + " " + valueAsStr + "\r \n");
			line.ReadVar(intVar);
			Assert.AreEqual(345, intVar.Value.Actual);
			Assert.AreEqual(valueAsStr, intVar.Value.String);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		private void TryReadVar(InputLine     line,
		                        InputVariable var)
		{
			try {
				line.ReadVar(var);
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadIntVar_AtEnd()
		{
			InputLine line = MakeInputLine(null);
			AssertLineAtEnd(line);
			TryReadVar(line, intVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadIntVar_BlankLine()
		{
			InputLine line = MakeInputLine("");
			TryReadVar(line, intVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadIntVar_WrongName()
		{
			InputLine line = MakeInputLine("WrongVar");
			TryReadVar(line, intVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadIntVar_MissingValue()
		{
			InputLine line = MakeInputLine(intVar.Name + " ");
			TryReadVar(line, intVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadIntVar_ExtraText()
		{
			InputLine line = MakeInputLine(intVar.Name + " -78 some extra text");
			TryReadVar(line, intVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadIntVar_LongExtraText()
		{
			InputLine line = MakeInputLine(intVar.Name + " -78 " + longExtraText);
			TryReadVar(line, intVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadIntVar_BadValue()
		{
			InputLine line = MakeInputLine(intVar.Name + " -7X");
			TryReadVar(line, intVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadIntVar_OutOfRange()
		{
			InputLine line = MakeInputLine(intVar.Name + " -999,999,999,999");
			TryReadVar(line, intVar);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadStrVar_Word()
		{
			string valueAsStr = "a.word_E=mc^2";
			InputLine line = MakeInputLine(strVar.Name + " " + valueAsStr);
			line.ReadVar(strVar);
			Assert.AreEqual(valueAsStr, strVar.Value.Actual);
			Assert.AreEqual(valueAsStr, strVar.Value.String);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		private void ReadStrVar_Quoted(string valueAsStr,
		                               string quotedValue)
		{
			string lineAsStr = strVar.Name + " " + quotedValue;
			InputLine line = MakeInputLine(lineAsStr);
			Assert.AreEqual(lineAsStr, line.ToString());

			line.ReadVar(strVar);
			Assert.AreEqual(valueAsStr, strVar.Value.Actual);
			Assert.AreEqual(quotedValue, strVar.Value.String);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		private void ReadStrVar_Quote(char quote)
		{
			string valueAsStr = "Four score and seven years ...";
			string quotedValue = string.Format("{0}{1}{0}", quote, valueAsStr);
			ReadStrVar_Quoted(valueAsStr, quotedValue);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadStrVar_DoubleQuote()
		{
			ReadStrVar_Quote('"');
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadStrVar_SingleQuote()
		{
			ReadStrVar_Quote('\'');
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadStrVar_DoubleQuoteEscape()
		{
			string valueAsStr = "And then he said \"Hi!\" to me";
			string quotedValue = "\"And then he said \\\"Hi!\\\" to me\"";
			ReadStrVar_Quoted(valueAsStr, quotedValue);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadStrVar_SingleQuoteEscape()
		{
			string valueAsStr = "And then he said 'Hi!' to me";
			string quotedValue = "'And then he said \\'Hi!\\' to me'";
			ReadStrVar_Quoted(valueAsStr, quotedValue);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadStrVar_AtEnd()
		{
			InputLine line = MakeInputLine(null);
			AssertLineAtEnd(line);
			TryReadVar(line, strVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadStrVar_BlankLine()
		{
			InputLine line = MakeInputLine("");
			TryReadVar(line, strVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadStrVar_WrongName()
		{
			InputLine line = MakeInputLine("WrongVar");
			TryReadVar(line, strVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadStrVar_MissingValue()
		{
			InputLine line = MakeInputLine(strVar.Name + " ");
			TryReadVar(line, strVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadStrVar_Word_ExtraText()
		{
			InputLine line = MakeInputLine(strVar.Name + " a-word-value  some extra text");
			TryReadVar(line, strVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadStrVar_Word_LongExtraText()
		{
			InputLine line = MakeInputLine(strVar.Name + " E=mc^2 " + longExtraText);
			TryReadVar(line, strVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadStrVar_DoubleQuote_ExtraText()
		{
			InputLine line = MakeInputLine(strVar.Name + " \"Four score and seven years...\"  some extra text");
			TryReadVar(line, strVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadStrVar_SingleQuote_ExtraText()
		{
			InputLine line = MakeInputLine(strVar.Name + " 'Four score and seven years...'  some extra text");
			TryReadVar(line, strVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadStrVar_Double_NoEndQuote()
		{
			InputLine line = MakeInputLine(strVar.Name + " \"Four score and seven years...");
			TryReadVar(line, strVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadStrVar_Single_NoEndQuote()
		{
			InputLine line = MakeInputLine(strVar.Name + " 'Four score and seven years...");
			TryReadVar(line, strVar);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadOptVar()
		{
			string valueAsStr = "345";
			InputLine line = MakeInputLine(intVar.Name + " " + valueAsStr);
			Assert.IsTrue(line.ReadOptionalVar(intVar));
			Assert.AreEqual(345, intVar.Value.Actual);
			Assert.AreEqual(valueAsStr, intVar.Value.String);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadOptVar_Missing()
		{
			string valueAsStr = "345";
			InputLine line = MakeInputLine(intVar.Name + " " + valueAsStr);

			Assert.IsFalse(line.ReadOptionalVar(strVar));

			line.ReadVar(intVar);
			Assert.AreEqual(345, intVar.Value.Actual);
			Assert.AreEqual(valueAsStr, intVar.Value.String);
			AssertNoMoreLines(line);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void ReadOptVar_MissingReqdVar()
		{
			InputLine line = MakeInputLine("NotIntOrStrVar");
			Assert.IsFalse(line.ReadOptionalVar(strVar));
			TryReadVar(line, intVar);
		}
	}
}
