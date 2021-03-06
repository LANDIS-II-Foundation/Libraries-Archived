namespace Landis
{
	/// <summary>
	/// Methods related to logging.
	/// </summary>
	public static class Log
	{
		/// <summary>
		/// Writes an informational message into the log.
		/// </summary>
		/// <param name="message">
		/// Message to write into the log.  It may contain placeholders for
		/// optional arguments using the "{n}" notation used by the
		/// System.String.Format method.
		/// </param>
		/// <param name="mesgArgs">
		/// Optional arguments for the message.
		/// </param>
		public static void Info(string          message,
		                        params object[] mesgArgs)
		{
			System.Console.WriteLine(message, mesgArgs);
		}
	}
}
