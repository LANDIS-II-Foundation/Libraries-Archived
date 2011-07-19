using System.Text;

namespace Landis.Util
{
	/// <summary>
	/// Class with methods for working with strings.
	/// </summary>
	public static class String
	{
		/// <summary>
		/// Read a string from a StringReader.
		/// </summary>
		/// <remarks>
		/// This method reads a string from a StringReader.  The string is
		/// either an unquoted word or a quoted string.  A word is one or more
		/// adjacent non-whitespace characters.  A quoted string is zero or
		/// more characters surrounded by a pair of quotes.  The quotes may be
		/// a pair of double quotes (") or a pair of single quotes (').  A
		/// quote character can be included inside a quoted string by escaping
		/// it with a backslash (\).
		/// <example>
		/// Here are some valid strings:
		/// <code>
		///   foo
		///   a-brief-phrase
		///   C:\some\path\to\a\file.ext
		///   ""
		///   "Four score and seven years ago ..."
		///   "That's incredulous!"
		///   'That\'s incredulous!'
		///   ''
		///   'He said "Boo."'
		///   "He said \"Boo.\""
		/// </code>
		/// </example>
		/// Whitespace preceeding the word or the quoted string is skipped.
		/// The delimiting quotes of a quoted string are removed.
		/// </remarks>
		/// <exception cref="">System.IO.EndOfStreamException</exception>
		public static InputValue<string> Read(StringReader reader,
		                                      out int      index)
		{
			TextReader.SkipWhitespace(reader);
			if (reader.Peek() == -1)
				throw new InputValueException();

			index = reader.Index;
			char nextChar = (char) reader.Peek();
			if (nextChar == '\'' || nextChar == '"') {
				//  Quoted string
				char startQuote = (char) reader.Read();
				StringBuilder quotedStr = new StringBuilder();
				quotedStr.Append(startQuote);
				StringBuilder actualStr = new StringBuilder();
				bool endQuoteFound = false;
				while (! endQuoteFound) {
					char? ch = TextReader.ReadChar(reader);
					if (! ch.HasValue)
						throw new InputValueException(quotedStr.ToString(),
						                              "Missing end quote: ({0})",
						                              quotedStr.ToString());
					if (ch.Value == startQuote) {
						endQuoteFound = true;
						quotedStr.Append(ch.Value);
					}
					else {
						if (ch.Value == '\\') {
							//  Get the next character if it's a quote.
							nextChar = (char) reader.Peek();
							if (nextChar == '\'' || nextChar == '"') {
								quotedStr.Append(ch.Value);
								ch = TextReader.ReadChar(reader);
							}
						}
						actualStr.Append(ch.Value);
						quotedStr.Append(ch.Value);
					}
				}
				return new InputValue<string>(actualStr.ToString(),
				                              quotedStr.ToString());
			}
			else {
				string word = TextReader.ReadWord(reader);
				return new InputValue<string>(word, word);
			}
		}
	}
}
