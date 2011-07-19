using System.Text;

namespace Landis.Util
{
	/// <summary>
	/// A class with method for working with System.IO.TextReader objects.
	/// </summary>
	public static class TextReader
	{
		/// <summary>
		/// Reads the next character from a System.IO.TextReader.
		/// </summary>
		public static char? ReadChar(System.IO.TextReader reader)
		{
			char? ch = null;
			int i = reader.Read();
			if (i != -1)
				ch = (char) i;
			return ch;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Reads and ignores whitespace characters from a
		/// System.IO.TextReader.
		/// </summary>
		public static void SkipWhitespace(System.IO.TextReader reader)
		{
			int i = reader.Peek();
			while (i != -1 && char.IsWhiteSpace((char) i)) {
				reader.Read();
				i = reader.Peek();
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Reads a word from a System.IO.TextReader.  A word is one or more
		/// adjacent non-whitespace characters.
		/// </summary>
		public static string ReadWord(System.IO.TextReader reader)
		{
			StringBuilder word = new StringBuilder();
			int i = reader.Peek();
			while (i != -1 && ! char.IsWhiteSpace((char) i)) {
				word.Append((char) reader.Read());
				i = reader.Peek();
			}
			return word.ToString();
		}
	}
}
