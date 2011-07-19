namespace Landis.Util
{
	/// <summary>
	/// Wrapper for ParseMethod so it can used as a ReadMethod.
	/// </summary>
	public class ParseMethodWrapper<T>
	{
		private ParseMethod<T> parseMethod;

		//---------------------------------------------------------------------

		public ParseMethodWrapper(ParseMethod<T> method)
		{
			this.parseMethod = method;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Read a value from a TextReader.
		/// </summary>
		/// <remarks>
		/// If the wrapped parse method throws an exception, this method
		/// catches the exception, modifies its Data property, and then
		/// rethrows it.  A new key/value pair is set in the Data property:
		/// the key is "ParseMethod.Word", and the value is the word that
		/// was passed as a parameter to the parse method.
		/// </remarks>
		public InputValue<T> Read(StringReader reader,
		                          out int      index)
		{
			//  Read word from reader.  A word is a sequence of 1 or more
			//	non-whitespace characters.
			TextReader.SkipWhitespace(reader);
			if (reader.Peek() == -1)
				throw new InputValueException();

			index = reader.Index;
			string word = TextReader.ReadWord(reader);
			try {
				return new InputValue<T>(parseMethod(word), word);
			}
			catch (System.OverflowException) {
				string format = string.Format("{{0:{0}}}",
				                              InputValues.GetFormat<T>());
				string min = string.Format(format, InputValues.GetMinValue<T>());
				string max = string.Format(format, InputValues.GetMinValue<T>());
				throw new InputValueException(word,
				                              "{0} is not between {1} and {2}",
				                              word, min, max);
			}
			catch (System.Exception) {
				throw new InputValueException(word,
				                              "\"{0}\" is not a valid {1}",
				                              word,
				                              InputValues.GetDescription<T>());
			}
		}
	}
}
