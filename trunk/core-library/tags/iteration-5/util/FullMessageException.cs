using System.Diagnostics;

namespace Landis.Util
{
	/// <summary>
	/// An exception with an additional property FullMessage that contains the
	/// exception's message along with the message of its inner exception.
	/// </summary>
	public class FullMessageException
		: System.ApplicationException
	{
		public static string indent;

		//---------------------------------------------------------------------

		/// <summary>
		/// The string to indent each line in the inner exception's message.
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

		static FullMessageException()
		{
			indent = "  ";
		}

		//---------------------------------------------------------------------

		private MultiLineText fullMessage;

		//---------------------------------------------------------------------

		/// <summary>
		/// The exception's message along with its inner exception' message if
		/// it has an inner exception.
		/// </summary>
		public MultiLineText FullMessage
		{
			get {
				if (fullMessage == null)
					fullMessage = MakeFullMessage();
				return fullMessage;
			}
		}

		//---------------------------------------------------------------------

		public FullMessageException(string 			 message,
		                            System.Exception innerException)
			: base(message, innerException)
		{
		}

		//---------------------------------------------------------------------

		public FullMessageException(string message)
			: base(message)
		{
		}

		//---------------------------------------------------------------------

		private MultiLineText MakeFullMessage()
		{
			MultiLineText message;
			if (InnerException == null)
				message = new MultiLineText(Message);
			else {
				message = new MultiLineText(Message + ":");
				FullMessageException inner = InnerException as FullMessageException;
				if (inner != null)
					foreach (string line in inner.FullMessage)
						message.Add(Indent + line);
				else
					message.Add(Indent + InnerException.Message);
			}
			return message;
		}
	}
}
