using log4net;
//using log4net.Config;

// Configure log4net using the .config file
[assembly: log4net.Config.DOMConfigurator(Watch=true)]

namespace Landis
{
	/// <summary>
	/// Methods related to logging.
	/// </summary>
	public static class Log
	{
		private static ILog logger;

		//---------------------------------------------------------------------

		static Log()
		{
			//Hierarchy hierarchy = LogManager.GetLoggerRepository() as Hierarchy;
			//logger = hierarchy.Root;
			//log4net.Config.XmlConfigurator.Configure();
			logger = LogManager.GetLogger("Landis");
		}

		//---------------------------------------------------------------------

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
			logger.Info(string.Format(message, mesgArgs));
			//System.Console.WriteLine(message, mesgArgs);
		}
	}
}
