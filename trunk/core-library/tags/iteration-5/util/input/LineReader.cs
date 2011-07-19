using System.Text.RegularExpressions;

namespace Landis.Util
{
	/// <summary>
	/// Represents a reader of lines from a text source.
	/// </summary>
	public abstract class LineReader
	{
		private int lineNumber;

		private string commentLineMarker;
		private string eolCommentMarker;

		/// <summary>
		/// Controls if blank lines are skipped by reader.
		/// </summary>
		public bool SkipBlankLines;

		/// <summary>
		/// Controls if comment lines are skipped by reader.
		/// </summary>
		public bool SkipCommentLines;

		/// <summary>
		/// Controls if end-of-line comments are trimmed by reader.
		/// </summary>
		public bool TrimEOLComments;

		//---------------------------------------------------------------------

		/// <summary>
		/// The value of a reader's LineNumber property if the reader has no
		/// more lines left.
		/// </summary>
		public static readonly int EndOfInput = -1;

		/// <summary>
		/// Default markers for comment lines and end-of-line comments.
		/// </summary>
		public static class Default
		{
			public static readonly string CommentLineMarker = "#";
			public static readonly string EOLCommentMarker  = "##";
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Name of the reader's source (for example, if the reader's source
		/// is a text file, this would be the file's name).
		/// </summary>
		public abstract string SourceName {
			get;
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
			if (char.IsWhiteSpace(marker, 0))
				throw new System.ApplicationException(
									"First character in " + markerName
									+ " cannot be whitespace.");
		}

		//---------------------------------------------------------------------

		public LineReader()
		{
			this.lineNumber = 0;
			this.SkipBlankLines = false;
			this.SkipCommentLines = false;
			this.TrimEOLComments = false;
			this.commentLineMarker = Default.CommentLineMarker;
			this.eolCommentMarker = Default.EOLCommentMarker;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Get the next line from the reader's source.
		/// </summary>
		/// <returns>
		/// The next line from the source or null if there are no lines
		/// left.
		/// </returns>
		protected abstract string GetNextLine();

		//---------------------------------------------------------------------

		/// <summary>
		/// Read the next line from the reader's source.
		/// </summary>
		/// <returns>
		/// The next line or null.
		/// </returns>
		public string ReadLine()
		{
			if (lineNumber == EndOfInput)
				return null;

			string line;
			while ((line = GetNextLine()) != null) {
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

			lineNumber = EndOfInput;
			return null;
		}

		//---------------------------------------------------------------------

		public StreamInputException MakeException(MultiLineText message)
		{
			return new StreamInputException(SourceName, lineNumber, message);
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
			if (lineNum == LineReader.EndOfInput)
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
