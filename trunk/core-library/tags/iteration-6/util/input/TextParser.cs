using System.Diagnostics;

namespace Landis.Util
{
	/// <summary>
	/// Base class for parsers that parse text input.
	/// </summary>
	public abstract class TextParser<T>
		: ITextParser<T>
	{
		private LineReader reader;
		private InputLine inputLine;
		private InputVariable currentVar;

		//---------------------------------------------------------------------

		/// <summary>
		/// The current input line.
		/// </summary>
		protected string CurrentLine
		{
			get {
				Debug.Assert( inputLine != null );
				return inputLine.ToString();
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The line number of the current line.
		/// </summary>
		protected int LineNumber
		{
			get {
				Debug.Assert( inputLine != null );
				return inputLine.Number;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The variable name at the start of the current line.
		/// </summary>
		protected string CurrentName
		{
			get {
				Debug.Assert( inputLine != null );
				return inputLine.VariableName;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// At end of input?
		/// </summary>
		protected bool AtEndOfInput
		{
			get {
				return (inputLine == null);
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance of TextParser.
		/// </summary>
		public TextParser()
		{
			reader = null;
			inputLine = null;
			currentVar = null;
		}

		//---------------------------------------------------------------------

		public T Parse(LineReader reader)
		{
			Require.ArgumentNotNull(reader);
			this.reader = reader;
			this.inputLine = new InputLine(reader);
			currentVar = null;

			try {
				return Parse();
			}
			catch (InputValueException valueExc) {
				if (currentVar == null) {
					throw new LineReaderException(reader, valueExc);
				}
				else {
					string message = string.Format("Error with the input value for {0}",
					                               currentVar.Name);
					InputVariableException varExc = new InputVariableException(currentVar,
					                                                           message,
					                                                           valueExc);
					throw new LineReaderException(reader, varExc);
				}
			}
			catch (InputVariableException varExc) {
				throw new LineReaderException(reader, varExc);
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Parses text input and returns an instance of T.
		/// </summary>
		/// <remarks>
		/// Subclasses should implement this method using the protected members
		/// of this base class to read the text input.
		/// </remarks>
		protected abstract T Parse();

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets the next line from the text input.
		/// </summary>
		/// <returns>
		/// true if there are no more lines in the input.
		/// </returns>
		protected bool GetNextLine()
		{
			Debug.Assert( inputLine != null );
			bool gotLine = inputLine.GetNext();
			if (! gotLine)
				inputLine = null;
			return gotLine;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Reads the name and value of an input variable from the current
		/// line.
		/// </summary>
		/// <exception cref="LineReaderException">
		/// </exception>
		protected void ReadVar(InputVariable var)
		{
			Debug.Assert( inputLine != null );
			inputLine.ReadVar(var);
			currentVar = var;
			GetNextLine();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Reads the name and value of an optional input variable from the
		/// current line.
		/// </summary>
		/// <returns>
		/// true if the name and value were read; false if the name on the
		/// current line does not match the variable's name.
		/// </returns>
		/// <exception cref="LineReaderException">
		/// </exception>
		protected bool ReadOptionalVar(InputVariable var)
		{
			Debug.Assert( inputLine != null );
			if (inputLine.ReadOptionalVar(var)) {
				currentVar = var;
				GetNextLine();
				return true;
			}
			else
				return false;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Reads a name from the current input line.
		/// </summary>
		/// <remarks>
		/// The line is expected to have only the given name.
		/// </remarks>
		/// <exception cref="LineReaderException">
		/// Thrown when the line is empty (has no name; LineReader
		/// is configured to return blank lines!).  Or when name on the line
		/// does not match the specified name.  Or when there's additional text
		/// after the name.
		/// </exception>
		protected void ReadName(string name)
		{
			Debug.Assert( inputLine != null );
			inputLine.MatchName(name);
			GetNextLine();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Reads an optional name from the current input line.
		/// </summary>
		/// <returns>
		/// true if the name on the current line matches the specified name;
		/// false if it does not match.
		/// </returns>
		/// <exception cref="LineReaderException">
		/// When the name on the current line matches the specified name, but
		/// there's additional text after the name.
		/// </exception>
		protected bool ReadOptionalName(string name)
		{
			Debug.Assert( inputLine != null );
			if (inputLine.MatchOptionalName(name)) {
				GetNextLine();
				return true;
			}
			else
				return false;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Reads the value of an input variable from the current line.
		/// </summary>
		/// <exception cref="InputValueException">
		/// </exception>
		/// <remarks>
		/// This method is used for reading column values from a table.  The
		/// current line is a row in the table.
		/// </remarks>
		protected void ReadValue(InputVariable var,
		                         StringReader  currentLine)
		{
			Require.ArgumentNotNull(var);
			Require.ArgumentNotNull(currentLine);
			var.ReadValue(currentLine);
			currentVar = var;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Checks to see if there are extra data after the last expected data
		/// of the text input.
		/// </summary>
		/// <exception cref="LineReaderException">
		/// If a non-blank line is found before the end of the input.
		/// </exception>
		protected void CheckNoDataAfter(string lastData)
		{
			while (! AtEndOfInput) {
				string currentLine = CurrentLine.TrimStart(null);
				if (currentLine.Length == 0)
					GetNextLine();
				else
					throw inputLine.ExtraTextException("Found extra data after " + lastData,
					                                   currentLine);
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Checks to see if there are extra data after the last expected
		/// column data on the current line.
		/// </summary>
		/// <exception cref="LineReaderException">
		/// If a non-blank line is found before the end of the input.
		/// </exception>
		protected void CheckNoDataAfter(string       lastData,
		                                StringReader currentLine)
		{
			TextReader.SkipWhitespace(currentLine);
			string extraText = currentLine.ReadToEnd();
			if (extraText.Length > 0) {
				throw inputLine.ExtraTextException("Found extra data after " + lastData,
				                                   extraText);
			}
		}
	}
}
