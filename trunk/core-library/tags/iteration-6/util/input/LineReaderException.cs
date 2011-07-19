using System.Text;

namespace Landis.Util
{
	public class LineReaderException
		: MultiLineException
	{
		private int line;
		private string source;

		//---------------------------------------------------------------------

		/// <summary>
		/// The name of the input source (e.g., file name).
		/// </summary>
		public string SourceName
		{
			get {
				return source;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The line number in the input where the exception occurred.
		/// </summary>
		/// <remarks>
		/// If the exception occurred at the end of the input, then this
		/// property equals LineReader.EndOfInput.
		/// </remarks>
		public int LineNumber
		{
			get {
				return line;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance from a specified reader and message.
		/// </summary>
		public LineReaderException(LineReader reader,
		                           string     message)
			: base(MakeLocationMessage(reader), message)
		{
			SetLocation(reader);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance from a specified reader and a formatted
		/// message.
		/// </summary>
		public LineReaderException(LineReader      reader,
		                           string          message,
		                           params object[] mesgArgs)
			: base(MakeLocationMessage(reader), string.Format(message, mesgArgs))
		{
			SetLocation(reader);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance from a specified reader and an inner
		/// exception.
		/// </summary>
		public LineReaderException(LineReader       reader,
		                           System.Exception exception)
			: base(MakeLocationMessage(reader), exception)
		{
			SetLocation(reader);
		}

		//---------------------------------------------------------------------

		private static string MakeLocationMessage(LineReader reader)
		{
			StringBuilder message = new StringBuilder();
			message.Append("Error at ");
			if (reader.LineNumber == LineReader.EndOfInput)
				message.Append("end");
			else
				message.Append(string.Format("line {0}", reader.LineNumber));
			if (! string.IsNullOrEmpty(reader.SourceName))
				message.Append(" of " + reader.SourceName);
			return message.ToString();
		}

		//---------------------------------------------------------------------

		private void SetLocation(LineReader reader)
		{
			this.line = reader.LineNumber;
			this.source = reader.SourceName;
		}
	}
}
