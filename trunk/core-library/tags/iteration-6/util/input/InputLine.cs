using System.Collections.Generic;
using System.Text;

namespace Landis.Util
{
	/// <summary>
	/// A line that is read from a LineReader and that has an InputVariable.
	/// </summary>
	public class InputLine
	{
		private LineReader reader;
		private string line;
		private string varName;
		private string textAfterName;
		private List<string> expectedNames;

		//---------------------------------------------------------------------

		/// <summary>
		/// The variable name on the current input line.
		/// </summary>
		public string VariableName
		{
			get {
				if (line != null && varName == null)
					SetVariableName();
				return varName;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The line number of the current input line.
		/// </summary>
		public int Number
		{
			get {
				return reader.LineNumber;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Creates a new InputLine for a particular LineReader.
		/// </summary>
		public InputLine(LineReader reader)
		{
			Require.ArgumentNotNull(reader);
			this.reader = reader;
			this.expectedNames = new List<string>();
			GetNext();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Get the next input line.
		/// </summary>
		public bool GetNext()
		{
			expectedNames.Clear();
			line = reader.ReadLine();
			varName = null;
			textAfterName = null;
			return line != null;
		}

		//---------------------------------------------------------------------

		private void SetVariableName()
		{
			System.IO.StringReader strReader = new System.IO.StringReader(line);
			TextReader.SkipWhitespace(strReader);
			varName = TextReader.ReadWord(strReader);
			TextReader.SkipWhitespace(strReader);
			textAfterName = strReader.ReadToEnd();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Matches the name of an InputVariable.
		/// </summary>
		/// <remarks>
		/// The line is expected to have only the given variable name.
		/// </remarks>
		/// <exception cref="LineReaderException">
		/// Thrown when the line is empty (has no variable name; LineReader
		/// is configured to return blank lines!).  Or when name on the line
		/// does
		/// not match the specified name.  Or when there's additional text
		/// after the name.
		/// </exception>
		public void MatchName(string name)
		{
			MatchVariableName(name, false);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Matches the name of an optional InputVariable.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		///   <paramref name="name"></paramref> is null.
		/// </exception>
		public bool MatchOptionalName(string name)
		{
			return MatchVariableName(name, true);
		}

		//---------------------------------------------------------------------

		private bool MatchVariableName(string name,
		                               bool   optional)
		{
			if (VariableNameMatches(name, optional)) {
				if (textAfterName.Length == 0)
					return true;
				throw ExtraTextException(string.Format("Extra text after the name \"{0}\"", name),
				                         textAfterName);
			}
			return false;
		}

		//---------------------------------------------------------------------

		public LineReaderException ExtraTextException(string message,
		                                              string text)
		{
			//	For readability, trim the text down to first 60 characters
			bool trimmed = false;
			const int displayLength = 60;
			if (text.Length > displayLength) {
				text = text.Substring(0, displayLength);
				trimmed = true;
			}

			//  Also for readability, trim off from 1st newline or return char.
			int index = text.IndexOfAny(new char[]{'\n', '\r'});
			if (index >= 0) {
				text = text.Substring(0, index);
				trimmed = true;
			}

			StringBuilder extraText = new StringBuilder();
			extraText.AppendFormat("\"{0}\"", text);
			if (trimmed)
				extraText.Append("...");

			MultiLineException innerException = new MultiLineException(message,
			                                                           extraText.ToString());
			return new LineReaderException(reader, innerException);
		}

		//---------------------------------------------------------------------

		private bool VariableNameMatches(string name,
		                                 bool   optional)
		{
			Require.ArgumentNotNull(name);
			if (VariableName == name)
				return true;

			expectedNames.Add(name);
			if (optional)
				return false;

			StringBuilder message = new StringBuilder();
			if (string.IsNullOrEmpty(VariableName))
				message.Append("Expected ");
			else
				message.AppendFormat("Found the name \"{0}\" but expected ",
				                     VariableName);
			if (expectedNames.Count == 1) {
				if (string.IsNullOrEmpty(VariableName))
					message.AppendFormat("the name \"{0}\"", expectedNames[0]);
				else
					message.AppendFormat("\"{0}\"", expectedNames[0]);
				throw new LineReaderException(reader, message.ToString());
			}
			else {
				message.Append("one of these names");
				throw new LineReaderException(reader,
				                              new MultiLineException(message.ToString(),
				                                                     new MultiLineText(expectedNames)));
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Reads the name and value for an InputVariable.
		/// </summary>
		public void ReadVar(InputVariable var)
		{
			ReadVariable(var, false);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Reads the name and value for an optional InputVariable.
		/// </summary>
		public bool ReadOptionalVar(InputVariable var)
		{
			return ReadVariable(var, true);
		}

		//---------------------------------------------------------------------

		private bool ReadVariable(InputVariable var,
		                          bool          optional)
		{
			if (var == null)
				throw new System.ArgumentNullException();
			if (VariableNameMatches(var.Name, optional)) {
				//  Read the variable's value
				StringReader strReader = new StringReader(textAfterName);
				try {
					var.ReadValue(strReader);
				}
				catch (InputVariableException exc) {
					throw new LineReaderException(reader, exc);
				}
				TextReader.SkipWhitespace(strReader);
				string textAfterValue = strReader.ReadToEnd();
				if (textAfterValue.Length == 0)
					return true;
				string message = string.Format("Extra text after the value for \"{0}\"",
				                               var.Name);
				throw ExtraTextException(message, textAfterValue);
			}
			return false;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Converts an InputLine to a boolean value.  
		/// </summary>
		/// <returns>
		/// <b>true</b> if the input line has a value, i.e., its reader has
		/// not reached the end of its input.
		/// </returns>
		public static implicit operator bool(InputLine inputLine)
		{
			return inputLine != null && inputLine.line != null;
		}

		//---------------------------------------------------------------------

		public override string ToString()
		{
			if (line == null)
				return "";
			else
				return line;
		}
	}
}
