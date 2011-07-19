using Landis.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Util
{
	[TestFixture]
	public class FileLineReader_Test
	{
		private string dataDir;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			dataDir = System.IO.Path.Combine(Data.Directory, "LineReader");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentException))]
		public void EmptyPath()
		{
			FileLineReader sr = new FileLineReader("");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void NullPath()
		{
			FileLineReader sr = new FileLineReader(null);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IO.FileNotFoundException))]
		public void FileNotFound()
		{
			string filename = System.IO.Path.Combine(dataDir,
			                               "file-that-should-not-exist.txt");
			System.IO.File.Delete(filename);
			FileLineReader sr = new FileLineReader(filename);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
		public void DirNotFound()
		{
			string subDir = System.IO.Path.Combine(dataDir,
			                               "subdir-that-should-not-exist");
			if (System.IO.Directory.Exists(subDir))
				System.IO.Directory.Delete(subDir, true);
			string filename = System.IO.Path.Combine(subDir, "filename.txt");
			FileLineReader sr = new FileLineReader(filename);
		}

		//---------------------------------------------------------------------

		private FileLineReader MakeReader(string filename)
		{
			string path = System.IO.Path.Combine(dataDir, filename);
			FileLineReader reader = new FileLineReader(path);
			Assert.AreEqual(path, reader.Path);
			return reader;
		}

		//---------------------------------------------------------------------

		private void CheckExpectedLines(LineReader reader,
		                                string     expectedLinesFile)
		{
			string path = System.IO.Path.Combine(dataDir, expectedLinesFile);
			List<ExpectedLine> lines = ExpectedLine.ReadLines(path);

			foreach (ExpectedLine expectedLine in lines) {
				string line = reader.ReadLine();
				Assert.IsNotNull(line);
				Assert.AreEqual(expectedLine.Number, reader.LineNumber);
				Assert.AreEqual(expectedLine.Text, line);
			}
			Assert.IsNull(reader.ReadLine());
			Assert.AreEqual(LineReader.EndOfInput, reader.LineNumber);
		}

		//---------------------------------------------------------------------

		[Test]
		public void EmptyFile()
		{
			FileLineReader reader = MakeReader("empty.txt");
			CheckExpectedLines(reader, "empty_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void BlankLines()
		{
			FileLineReader reader = MakeReader("blankLines.txt");
			CheckExpectedLines(reader, "blankLines_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void BlankLines_SkipBlank()
		{
			FileLineReader reader = MakeReader("blankLines.txt");
			reader.SkipBlankLines = true;
			CheckExpectedLines(reader, "blankLines_skipBlank.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void CommentLines()
		{
			FileLineReader reader = MakeReader("commentLines.txt");
			CheckExpectedLines(reader, "commentLines_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void CommentLines_SkipComment()
		{
			FileLineReader reader = MakeReader("commentLines.txt");
			reader.SkipCommentLines = true;
			CheckExpectedLines(reader, "commentLines_skipComment.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void EndComments()
		{
			FileLineReader reader = MakeReader("eolComments.txt");
			CheckExpectedLines(reader, "eolComments_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void EndComments_TrimEOL()
		{
			FileLineReader reader = MakeReader("eolComments.txt");
			reader.TrimEndComments = true;
			CheckExpectedLines(reader, "eolComments_trimEOL.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void BlankAndEndComments()
		{
			FileLineReader reader = MakeReader("blank+EOL.txt");
			CheckExpectedLines(reader, "blank+EOL_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void BlankAndEndComments_SkipBlank_TrimEOL()
		{
			FileLineReader reader = MakeReader("blank+EOL.txt");
			reader.SkipBlankLines = true;
			reader.TrimEndComments = true;
			CheckExpectedLines(reader, "blank+EOL_skipBlank_trimEOL.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput()
		{
			FileLineReader reader = MakeReader("mixed.txt");
			CheckExpectedLines(reader, "mixed_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_SkipBlank()
		{
			FileLineReader reader = MakeReader("mixed.txt");
			reader.SkipBlankLines = true;
			CheckExpectedLines(reader, "mixed_skipBlank.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_SkipComment()
		{
			FileLineReader reader = MakeReader("mixed.txt");
			reader.SkipCommentLines = true;
			CheckExpectedLines(reader, "mixed_skipComment.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_SkipBoth()
		{
			FileLineReader reader = MakeReader("mixed.txt");
			reader.SkipBlankLines = true;
			reader.SkipCommentLines = true;
			CheckExpectedLines(reader, "mixed_skipBoth.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_TrimEOL()
		{
			FileLineReader reader = MakeReader("mixed.txt");
			reader.TrimEndComments = true;
			CheckExpectedLines(reader, "mixed_trimEOL.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_SkipAndTrim()
		{
			FileLineReader reader = MakeReader("mixed.txt");
			reader.SkipBlankLines = true;
			reader.SkipCommentLines = true;
			reader.TrimEndComments = true;
			CheckExpectedLines(reader, "mixed_skipAndTrim.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_CommentLineMarker()
		{
			FileLineReader reader = MakeReader("mixed.txt");
			reader.SkipBlankLines = true;
			reader.SkipCommentLines = true;
			reader.CommentLineMarker = "//";
			reader.TrimEndComments = true;
			CheckExpectedLines(reader, "mixed_commentLineMarker.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_EndCommentMarker()
		{
			FileLineReader reader = MakeReader("mixed.txt");
			reader.SkipBlankLines = true;
			reader.SkipCommentLines = true;
			reader.TrimEndComments = true;
			reader.EndCommentMarker = "//";
			CheckExpectedLines(reader, "mixed_eolCommentMarker.txt");
		}
	}
}
