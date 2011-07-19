namespace Landis.Util
{
	public class InputValueException
		: MultiLineException
	{
		private string val;

		//---------------------------------------------------------------------

		public string Value
		{
			get {
				return val;
			}
		}

		//---------------------------------------------------------------------

		public InputValueException(string inputValue,
		                           string message,
		                           params object[] messageArgs)
			: base(string.Format(message, messageArgs))
		{
			this.val = inputValue;
		}

		//---------------------------------------------------------------------

		public InputValueException(string        inputValue,
		                           string        message,
		                           MultiLineText innerMessage)
			: base(message, innerMessage)
		{
			this.val = inputValue;
		}

		//---------------------------------------------------------------------

		public InputValueException()
			: base("Missing value")
		{
			this.val = null;
		}
	}
}
