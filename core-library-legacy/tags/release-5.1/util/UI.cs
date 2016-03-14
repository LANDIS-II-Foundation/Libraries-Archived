using System;
using System.IO;

namespace Landis
{
	/// <summary>
	/// The user interface for Landis.
	/// </summary>
	public static class UI
	{
		private static TextWriter writer;

		//---------------------------------------------------------------------

		static UI()
		{
			writer = TextWriter.Null;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Access the user interface as a text writer.
		/// </summary>
		/// <remarks>
		/// Default value is TextWriter.Null.
		/// </remarks>
		public static TextWriter TextWriter
		{
			get {
				return writer;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("Use System.IO.TextWriter.Null instead of null");
				writer = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Writes a line terminator to the user interface and the application
		/// log.
		/// </summary>
		public static void WriteLine()
		{
			writer.WriteLine();
			Log.Info(string.Empty);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Writes a string followed by a line terminator to the user
		/// interface and the application log.
		/// </summary>
		public static void WriteLine(string text)
		{
			writer.WriteLine(text);
			Log.Info(text);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Writes a formatted string to the user interface and the application
		/// log, using the same semantics as System.String.Format.  A line
		/// terminator is written after the formatted string.
		/// </summary>
		public static void WriteLine(string          format,
		                             params object[] args)
		{
			writer.WriteLine(format, args);
			Log.Info(format, args);
		}
	}
}
