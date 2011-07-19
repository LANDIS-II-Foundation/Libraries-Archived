using Landis.Util;
using NUnit.Framework;
using IO = System.IO;

namespace Landis.Test.Util
{
	[TestFixture]
	public class TextReader_Test
	{
		[Test]
		public void ReadChar_NullReader()
		{
			char? ch = TextReader.ReadChar(IO.TextReader.Null);
			Assert.IsFalse(ch.HasValue);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadChar_EmptyString()
		{
			IO.StringReader reader = new IO.StringReader("");
			char? ch = TextReader.ReadChar(reader);
			Assert.IsFalse(ch.HasValue);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadChar_NonEmptyString()
		{
			string s = "Four score and seven years ago ...";
			IO.StringReader reader = new IO.StringReader(s);
			char? actualChar;
			foreach (char expectedChar in s) {
				actualChar = TextReader.ReadChar(reader);
				Assert.IsTrue(actualChar.HasValue);
				Assert.AreEqual(expectedChar, actualChar.Value);
			}
			actualChar = TextReader.ReadChar(reader);
			Assert.IsFalse(actualChar.HasValue);
		}

		//---------------------------------------------------------------------

		[Test]
		public void SkipWhitespace_NullReader()
		{
			TextReader.SkipWhitespace(IO.TextReader.Null);
		}

		//---------------------------------------------------------------------

		[Test]
		public void SkipWhitespace_EmptyString()
		{
			IO.StringReader reader = new IO.StringReader("");
			TextReader.SkipWhitespace(reader);
			Assert.AreEqual(-1, reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void SkipWhitespace_JustWhitespace()
		{
			IO.StringReader reader = new IO.StringReader("\t \n\r");
			TextReader.SkipWhitespace(reader);
			Assert.AreEqual(-1, reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void SkipWhitespace_JustWord()
		{
			IO.StringReader reader = new IO.StringReader("ABCs");
			TextReader.SkipWhitespace(reader);
			Assert.AreEqual((int) 'A', reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void SkipWhitespace_LeadingWhitespace()
		{
			IO.StringReader reader = new IO.StringReader("   \t 987  ");
			TextReader.SkipWhitespace(reader);
			Assert.AreEqual((int) '9', reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadWord_NullReader()
		{
			string word = TextReader.ReadWord(IO.TextReader.Null);
			Assert.AreEqual("", word);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadWord_EmptyString()
		{
			IO.StringReader reader = new IO.StringReader("");
			string word = TextReader.ReadWord(reader);
			Assert.AreEqual("", word);
			Assert.AreEqual(-1, reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadWord_Whitespace()
		{
			IO.StringReader reader = new IO.StringReader("\t \n\r");
			string word = TextReader.ReadWord(reader);
			Assert.AreEqual("", word);
			Assert.AreEqual((int) '\t', reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadWord_JustWord()
		{
			IO.StringReader reader = new IO.StringReader("ABCs");
			string word = TextReader.ReadWord(reader);
			Assert.AreEqual("ABCs", word);
			Assert.AreEqual(-1, reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadWord_LeadingWhitespace()
		{
			IO.StringReader reader = new IO.StringReader(" \t987");
			string word = TextReader.ReadWord(reader);
			Assert.AreEqual("", word);
			Assert.AreEqual((int) ' ', reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadWord_TrailingWhitespace()
		{
			IO.StringReader reader = new IO.StringReader("<-o->\n \t");
			string word = TextReader.ReadWord(reader);
			Assert.AreEqual("<-o->", word);
			Assert.AreEqual((int) '\n', reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void MultipleWords()
		{
			string[] colors = new string[] { "red", "blue", "green", "white" };
			IO.StringReader reader = new IO.StringReader(string.Join(" ", colors));
			foreach (string color in colors) {
				TextReader.SkipWhitespace(reader);
				Assert.AreEqual(color, TextReader.ReadWord(reader));
			}
			Assert.AreEqual(-1, reader.Peek());
		}
	}
}
