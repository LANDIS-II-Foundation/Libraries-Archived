using Landis.Util;
using NUnit.Framework;

namespace Landis.Test.Util
{
	[TestFixture]
	public class String_Test
	{
		private void PrintException(StringReader reader)
		{
			try {
				int index;
				InputValue<string> val = String.Read(reader, out index);
			}
			catch (InputValueException exc) {
				System.Console.WriteLine(exc.Message);
				throw exc;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void Read_EmptyString()
		{
			StringReader reader = new StringReader("");
			PrintException(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void Read_Whitespace()
		{
			StringReader reader = new StringReader("\t \n\r");
			PrintException(reader);
		}

		//---------------------------------------------------------------------

		private void CheckReadResults(string readerInitVal,
		                              string expectedReadResult,
		                              string readerValAfterRead)
		{
			StringReader reader = new StringReader(readerInitVal);
			int index;
			InputValue<string> val = String.Read(reader, out index);
			Assert.AreEqual(expectedReadResult, val.Actual);
			Assert.AreEqual(readerValAfterRead, reader.ReadToEnd());
		}

		//---------------------------------------------------------------------

		private void CheckReadResults(string readerInitVal,
		                              string expectedReadResult)
		{
			StringReader reader = new StringReader(readerInitVal);
			int index;
			InputValue<string> val = String.Read(reader, out index);
			Assert.AreEqual(expectedReadResult, val.Actual);
			Assert.AreEqual(-1, reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_JustWord()
		{
			CheckReadResults("ABCs",
			                 "ABCs");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_WhitespaceWord()
		{
			CheckReadResults("   \t 987",
			                 "987");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_WordWhitespace()
		{
			CheckReadResults("hello\n",
			                 "hello",
			                 "\n");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_WhitespaceWordWhitespace()
		{
			CheckReadResults("\r \t hello\n",
			                 "hello",
			                 "\n");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_MultipleWords()
		{
			string[] words = new string[] { "987.01", ".'.", "x-y*z^2", @"C:\some\Path\to\a\file.ext" };
			StringReader reader = new StringReader(string.Join(" ", words));
			foreach (string word in words) {
				int index;
				InputValue<string> str = String.Read(reader, out index);
				Assert.AreEqual(word, str.Actual);
				Assert.AreEqual(index + word.Length, reader.Index);
			}
			Assert.AreEqual(-1, reader.Peek());
		}

		//---------------------------------------------------------------------
		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void Read_DoubleQuote_NoEnd()
		{
			StringReader reader = new StringReader("\"");
			PrintException(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void Read_DoubleQuote_TextNoEnd()
		{
			StringReader reader = new StringReader("\"Four score and ");
			PrintException(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_DoubleQuotes_Empty()
		{
			CheckReadResults("\"\"",
			                 "");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_DoubleQuotes_EmptyWhitespace()
		{
			CheckReadResults("\"\"\n ",
			                 "",
			                 "\n ");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_DoubleQuotes_WhitespaceEmpty()
		{
			CheckReadResults(" \t  \"\"",
			                 "");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_DoubleQuotes_WhitespaceEmptyWhitespace()
		{
			CheckReadResults(" \t  \"\"\r ",
			                "",
			                "\r ");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_DoubleQuotes_Str()
		{
			CheckReadResults("\"Hello world!\"",
			                 "Hello world!");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_DoubleQuotes_WhitespaceStr()
		{
			CheckReadResults(" \t \"Hello world!\"",
			                 "Hello world!");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_DoubleQuotes_StrEscape()
		{
			CheckReadResults(" \t \"It went \\\"Boom!\\\"\" ",
			                 "It went \"Boom!\"",
			                 " ");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_DoubleQuotes_StrEscapeOtherQuote()
		{
			CheckReadResults(" \t \"It went \\\'Boom!\\\'\" ",
			                 "It went 'Boom!'",
			                 " ");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_DoubleQuotes_StrOtherQuote()
		{
			CheckReadResults(" \t \"It went 'Boom!'\" ",
			                 "It went 'Boom!'",
			                 " ");
		}

		//---------------------------------------------------------------------
		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void Read_SingleQuote_NoEnd()
		{
			StringReader reader = new StringReader("'");
			PrintException(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void Read_SingleQuote_TextNoEnd()
		{
			StringReader reader = new StringReader("'Four score and ");
			PrintException(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_SingleQuote_Empty()
		{
			CheckReadResults("''",
			                 "");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_SingleQuote_EmptyWhitespace()
		{
			CheckReadResults("''\n ",
			                 "",
			                 "\n ");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_SingleQuote_WhitespaceEmpty()
		{
			CheckReadResults(" \t  ''",
			                 "");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_SingleQuote_WhitespaceEmptyWhitespace()
		{
			CheckReadResults(" \t  ''\r ",
			                 "",
			                 "\r ");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_SingleQuote_Str()
		{
			CheckReadResults("'Hello world!'",
			                 "Hello world!");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_SingleQuote_WhitespaceStr()
		{
			CheckReadResults(" \t 'Hello world!'",
			                 "Hello world!");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_SingleQuote_StrEscape()
		{
			CheckReadResults(" \t 'It went \\'Boom!\\'' ",
			                 "It went 'Boom!'",
			                 " ");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_SingleQuote_StrEscapeOtherQuote()
		{
			CheckReadResults(" \t 'It went \\\"Boom!\\\"' ",
			                 "It went \"Boom!\"",
			                 " ");
		}

		//---------------------------------------------------------------------

		[Test]
		public void Read_SingleQuote_StrOtherQuote()
		{
			CheckReadResults(" \t 'It went \"Boom!\"' ",
			                 "It went \"Boom!\"",
			                 " ");
		}
	}
}
