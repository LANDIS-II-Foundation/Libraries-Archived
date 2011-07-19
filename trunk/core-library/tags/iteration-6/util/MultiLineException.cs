using System.Diagnostics;

namespace Landis.Util
{
	/// <summary>
	/// An exception with an multiline text message.
	/// </summary>
	public class MultiLineException
		: System.ApplicationException
	{
		public static string indent;

		//---------------------------------------------------------------------

		/// <summary>
		/// The string to indent each line in the inner portion of an
		/// exception's message.
		/// </summary>
		public static string Indent
		{
			get {
				return indent;
			}

			set {
				Debug.Assert( value != null );
				indent = value;
			}
		}

		//---------------------------------------------------------------------

		static MultiLineException()
		{
			indent = "  ";
		}

		//---------------------------------------------------------------------

		private MultiLineText multiLineMessage;

		//---------------------------------------------------------------------

		/// <summary>
		/// The exception's message with its separate lines.
		/// </summary>
		public MultiLineText MultiLineMessage
		{
			get {
				return multiLineMessage;
			}
		}

		//---------------------------------------------------------------------

		public override string Message
		{
			get {
				return multiLineMessage.ToString();
			}
		}

		//---------------------------------------------------------------------

		public MultiLineException(string message)
			: base(message)
		{
			EnsureMessageNotNull(message);
			SetMultiLineMessage(message, null);
		}

		//---------------------------------------------------------------------

		public MultiLineException(string        message,
		                          MultiLineText innerMessage)
			: base(message)
		{
			EnsureMessageNotNull(message);
			SetMultiLineMessage(message + ":", innerMessage);
		}

		//---------------------------------------------------------------------

		public MultiLineException(string 		   message,
		                          System.Exception innerException)
			: base(message, innerException)
		{
			EnsureMessageNotNull(message);
			if (innerException == null)
				SetMultiLineMessage(message, null);
			else {
				string myMessage = message + ":";
				MultiLineException inner = innerException as MultiLineException;
				if (inner != null)
					SetMultiLineMessage(myMessage, inner.MultiLineMessage);
				else
					SetMultiLineMessage(myMessage, innerException.Message);
			}
		}

		//---------------------------------------------------------------------

		private void EnsureMessageNotNull(string message)
		{
			if (message == null)
				throw new System.ArgumentNullException();
		}

		//---------------------------------------------------------------------

		private void SetMultiLineMessage(string        message,
		                                 MultiLineText innerMessage)
		{
			multiLineMessage = new MultiLineText(message);
			if (innerMessage != null) {
				foreach (string line in innerMessage)
					multiLineMessage.Add(Indent + line);
			}
		}
	}
}
