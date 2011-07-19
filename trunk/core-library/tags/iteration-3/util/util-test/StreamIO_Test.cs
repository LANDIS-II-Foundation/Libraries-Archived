using Landis.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Util
{
	[TestFixture]
	public class StreamIO_Test
	{
		private string dataDir;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			dataDir = System.IO.Path.Combine(Data.Directory, "StreamReader");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentException))]
		public void EmptyPath()
		{
			StreamReader sr = new StreamReader("");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void NullPath()
		{
			StreamReader sr = new StreamReader(null);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IO.FileNotFoundException))]
		public void FileNotFound()
		{
			string filename = System.IO.Path.Combine(dataDir,
			                               "file-that-should-not-exist.txt");
			System.IO.File.Delete(filename);
			StreamReader sr = new StreamReader(filename);
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
			StreamReader sr = new StreamReader(filename);
		}

		//---------------------------------------------------------------------

		private StreamReader MakeReader(string filename)
		{
			string path = System.IO.Path.Combine(dataDir, filename);
			return new StreamReader(path);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void CommentLineMarker_Empty()
		{
			try {
				StreamReader strmReader = MakeReader("empty.txt");
				strmReader.CommentLineMarker = "";
			}
			catch (System.ApplicationException e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void CommentLineMarker_1stCharWhitespace()
		{
			try {
				StreamReader strmReader = MakeReader("empty.txt");
				strmReader.CommentLineMarker = " #";
			}
			catch (System.ApplicationException e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void EOLCommentMarker_Empty()
		{
			try {
				StreamReader strmReader = MakeReader("empty.txt");
				strmReader.EOLCommentMarker = "";
			}
			catch (System.ApplicationException e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void EOLCommentMarker_1stCharWhitespace()
		{
			try {
				StreamReader strmReader = MakeReader("empty.txt");
				strmReader.EOLCommentMarker = " //";
			}
			catch (System.ApplicationException e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		private void CheckExpectedLines(StreamReader strmReader,
		                                string		 filename)
		{
			string path = System.IO.Path.Combine(dataDir, filename);
			List<ExpectedLine> lines = ExpectedLine.ReadLines(path);
			string line;
			foreach (ExpectedLine expectedLine in lines) {
				line = strmReader.ReadLine();
				Assert.IsNotNull(line);
				Assert.AreEqual(expectedLine.Number, strmReader.LineNumber);
				Assert.AreEqual(expectedLine.Text, line);
			}
			line = strmReader.ReadLine();
			Assert.IsNull(line);
			Assert.AreEqual(StreamReader.EndOfStream, strmReader.LineNumber);
		}

		//---------------------------------------------------------------------

		[Test]
		public void EmptyFile()
		{
			StreamReader strmReader = MakeReader("empty.txt");
			CheckExpectedLines(strmReader, "empty_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void BlankLines()
		{
			StreamReader strmReader = MakeReader("blankLines.txt");
			CheckExpectedLines(strmReader, "blankLines_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void BlankLines_SkipBlank()
		{
			StreamReader strmReader = MakeReader("blankLines.txt");
			strmReader.SkipBlankLines = true;
			CheckExpectedLines(strmReader, "blankLines_skipBlank.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void CommentLines()
		{
			StreamReader strmReader = MakeReader("commentLines.txt");
			CheckExpectedLines(strmReader, "commentLines_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void CommentLines_SkipComment()
		{
			StreamReader strmReader = MakeReader("commentLines.txt");
			strmReader.SkipCommentLines = true;
			CheckExpectedLines(strmReader, "commentLines_skipComment.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void EOLComments()
		{
			StreamReader strmReader = MakeReader("eolComments.txt");
			CheckExpectedLines(strmReader, "eolComments_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void EOLComments_TrimEOL()
		{
			StreamReader strmReader = MakeReader("eolComments.txt");
			strmReader.TrimEOLComments = true;
			CheckExpectedLines(strmReader, "eolComments_trimEOL.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void BlankAndEOLComments()
		{
			StreamReader strmReader = MakeReader("blank+EOL.txt");
			CheckExpectedLines(strmReader, "blank+EOL_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void BlankAndEOLComments_SkipBlank_TrimEOL()
		{
			StreamReader strmReader = MakeReader("blank+EOL.txt");
			strmReader.SkipBlankLines = true;
			strmReader.TrimEOLComments = true;
			CheckExpectedLines(strmReader, "blank+EOL_skipBlank_trimEOL.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput()
		{
			StreamReader strmReader = MakeReader("mixed.txt");
			CheckExpectedLines(strmReader, "mixed_expected.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_SkipBlank()
		{
			StreamReader strmReader = MakeReader("mixed.txt");
			strmReader.SkipBlankLines = true;
			CheckExpectedLines(strmReader, "mixed_skipBlank.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_SkipComment()
		{
			StreamReader strmReader = MakeReader("mixed.txt");
			strmReader.SkipCommentLines = true;
			CheckExpectedLines(strmReader, "mixed_skipComment.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_SkipBoth()
		{
			StreamReader strmReader = MakeReader("mixed.txt");
			strmReader.SkipBlankLines = true;
			strmReader.SkipCommentLines = true;
			CheckExpectedLines(strmReader, "mixed_skipBoth.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_TrimEOL()
		{
			StreamReader strmReader = MakeReader("mixed.txt");
			strmReader.TrimEOLComments = true;
			CheckExpectedLines(strmReader, "mixed_trimEOL.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_SkipAndTrim()
		{
			StreamReader strmReader = MakeReader("mixed.txt");
			strmReader.SkipBlankLines = true;
			strmReader.SkipCommentLines = true;
			strmReader.TrimEOLComments = true;
			CheckExpectedLines(strmReader, "mixed_skipAndTrim.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_CommentLineMarker()
		{
			StreamReader strmReader = MakeReader("mixed.txt");
			strmReader.SkipBlankLines = true;
			strmReader.SkipCommentLines = true;
			strmReader.CommentLineMarker = "//";
			strmReader.TrimEOLComments = true;
			CheckExpectedLines(strmReader, "mixed_commentLineMarker.txt");
		}

		//---------------------------------------------------------------------

		[Test]
		public void MixedInput_EOLCommentMarker()
		{
			StreamReader strmReader = MakeReader("mixed.txt");
			strmReader.SkipBlankLines = true;
			strmReader.SkipCommentLines = true;
			strmReader.TrimEOLComments = true;
			strmReader.EOLCommentMarker = "//";
			CheckExpectedLines(strmReader, "mixed_eolCommentMarker.txt");
		}
	}
}
