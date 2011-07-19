using Landis.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Util
{
	[TestFixture]
	public class TextLineReader_Test
	{
		private string[] array;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			array = new string[]{ "Four score and seven years ago",
			                      "See Spot run",
			                      "abcde fghij klmno pqrst uvwxy Z" };
		}

		//---------------------------------------------------------------------

		private void AssertReaderAtEnd(TextLineReader reader)
		{
			Assert.IsNull(reader.ReadLine());
			Assert.AreEqual(reader.LineNumber, LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		public void NullText()
		{
			TextLineReader reader = new TextLineReader(null);
			Assert.IsNull(reader.SourceName);
			AssertReaderAtEnd(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		public void NullSourceNameAndText()
		{
			TextLineReader reader = new TextLineReader(null, null);
			Assert.IsNull(reader.SourceName);
			AssertReaderAtEnd(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		public void SourceNameAndNullText()
		{
			string sourceName = "text input";
			TextLineReader reader = new TextLineReader(sourceName, null);
			Assert.AreEqual(sourceName, reader.SourceName);
			AssertReaderAtEnd(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		public void String()
		{
			string str = "Four score and seven years ago";
			TextLineReader reader = new TextLineReader(str);
			Assert.IsNull(reader.SourceName);

			Assert.AreEqual(str, reader.ReadLine());
			Assert.AreEqual(1, reader.LineNumber);

			AssertReaderAtEnd(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		public void StringArray()
		{
			TextLineReader reader = new TextLineReader(array);
			Assert.IsNull(reader.SourceName);

			int expectedLineNum = 0;
			foreach (string str in array) {
				Assert.AreEqual(str, reader.ReadLine());
				expectedLineNum++;
				Assert.AreEqual(expectedLineNum, reader.LineNumber);
			}

			AssertReaderAtEnd(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		public void StringList()
		{
			List<string> list = new List<string>(array.Length);
			foreach (string str in array)
				list.Add(str);

			TextLineReader reader = new TextLineReader(list);
			Assert.IsNull(reader.SourceName);

			int expectedLineNum = 0;
			foreach (string str in list) {
				Assert.AreEqual(str, reader.ReadLine());
				expectedLineNum++;
				Assert.AreEqual(expectedLineNum, reader.LineNumber);
			}

			AssertReaderAtEnd(reader);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MultiLineText()
		{
			MultiLineText text = new MultiLineText(array);

			TextLineReader reader = new TextLineReader(text);
			Assert.IsNull(reader.SourceName);

			int expectedLineNum = 0;
			foreach (string line in text) {
				Assert.AreEqual(line, reader.ReadLine());
				expectedLineNum++;
				Assert.AreEqual(expectedLineNum, reader.LineNumber);
			}

			AssertReaderAtEnd(reader);
		}
	}
}
