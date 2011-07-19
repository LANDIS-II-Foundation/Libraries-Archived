using System.Text.RegularExpressions;

namespace Landis.Util
{
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	public class StreamReader
	{
		private System.IO.StreamReader strmReader;
		int lineNumber;

		public readonly string Path;
		public static readonly int EndOfStream = -1;

		public bool SkipBlankLines;
		public bool SkipCommentLines;
		public bool TrimEOLComments;

		private string commentLineMarker;
		private string eolCommentMarker;

		public static class Default
		{
			public static readonly string CommentLineMarker = "#";
			public static readonly string EOLCommentMarker  = "##";
		}

		//---------------------------------------------------------------------

		public int LineNumber {
			get {
				return lineNumber;
			}
		}

		//---------------------------------------------------------------------

		public string CommentLineMarker {
			get {
				return commentLineMarker;
			}
			set {
				ValidateMarker(value, "CommentLineMarker");
				commentLineMarker = value;
			}
		}

		//---------------------------------------------------------------------

		public string EOLCommentMarker {
			get {
				return eolCommentMarker;
			}
			set {
				ValidateMarker(value, "EOLCommentMarker");
				eolCommentMarker = value;
			}
		}

		//---------------------------------------------------------------------

		private void ValidateMarker(string marker,
		                            string markerName)
		{
			if (marker.Length == 0)
				throw new System.ApplicationException(
									markerName + " cannot be empty string.");
			if (System.Char.IsWhiteSpace(marker, 0))
				throw new System.ApplicationException(
									"First character in " + markerName
									+ " cannot be whitespace.");
		}

		//---------------------------------------------------------------------

		public StreamReader(string path)
		{
			this.Path = path;
			this.strmReader = new System.IO.StreamReader(path);
			this.lineNumber = 0;
			this.SkipBlankLines = false;
			this.SkipCommentLines = false;
			this.TrimEOLComments = false;
			this.commentLineMarker = Default.CommentLineMarker;
			this.eolCommentMarker = Default.EOLCommentMarker;
		}

		//---------------------------------------------------------------------

		public string ReadLine()
		{
			if (strmReader == null)
				return null;

			string line;
			while ((line = strmReader.ReadLine()) != null) {
				lineNumber++;

				//	Trimming must occur before testing whether to skip blank
				//	lines because trimming a EOL comment may make a line blank.
				if (TrimEOLComments) {
					int indexOfMarker = line.IndexOf(eolCommentMarker);
					if (indexOfMarker > -1)
						line = line.Substring(0, indexOfMarker);
				}

				if (SkipBlankLines || SkipCommentLines) {
					string trimmedLine = Regex.Replace(line, @"^\s+", "");
					if (SkipBlankLines && (trimmedLine.Length == 0))
						continue;
					if (SkipCommentLines &&
					    	trimmedLine.StartsWith(commentLineMarker))
						continue;
				}

				return line;
			}

			strmReader.Close();
			strmReader = null;
			lineNumber = EndOfStream;
			return null;
		}

		//---------------------------------------------------------------------

		public StreamInputException MakeException(MultiLineText message)
		{
			return new StreamInputException(Path, lineNumber, message);
		}
	}

	//-------------------------------------------------------------------------

	//	Represents an exception with data read from input stream
	public class StreamInputException
		: System.ApplicationException
	{
		public readonly string Path;
		public readonly int Line;

		//---------------------------------------------------------------------

		public StreamInputException(string 		  path,
		                            int    		  line,
		                            MultiLineText message)
			: base(MakeMessage(path, line, message))
		{
			this.Path = path;
			this.Line = line;
		}

		//---------------------------------------------------------------------

		private static string MakeMessage(string 		path,
		                                  int    		lineNum,
		                                  MultiLineText message)
		{
			string location;
			if (lineNum == StreamReader.EndOfStream)
				location = "end of file";
			else
				location = string.Format("line {0}", lineNum);
			MultiLineText result = new MultiLineText();
			result += string.Format("At {0} of {1}:", location, path);
			foreach (string line in message)
				result += "  " + line;
			return result.ToString();
		}
	}
}
