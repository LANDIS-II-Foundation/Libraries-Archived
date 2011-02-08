using System;
using System.Reflection;
using System.Text;

namespace Landis
{
	public static class App
	{
		public static int Main(string[] args)
		{
			try {
				UI.TextWriter = Console.Out;

				Version version = Assembly.GetExecutingAssembly().GetName().Version;
				string release = "";
				if (version.Revision >= 2000)
					release = string.Format(" (beta release {0})", version.Revision - 2000);
				else if (version.Revision >= 1000)
					release = string.Format(" (alpha release {0})", version.Revision - 1000);
				UI.WriteLine("Landis-II {0}.{1}{2}", version.Major, version.Minor, release);
				UI.WriteLine("Copyright 2004-2005 University of Wisconsin");
				UI.WriteLine();
	
				if (args.Length == 0) {
					UI.WriteLine("Error: No scenario file specified.");
					return 1;
				}
				if (args.Length > 1) {
					UI.WriteLine("Error: Extra argument(s) on command line:");
					StringBuilder argsList = new StringBuilder();
					argsList.Append(" ");
					for (int i = 1; i < args.Length; ++i)
						argsList.AppendFormat(" {0}", args[i]);
					UI.WriteLine(argsList.ToString());
					return 1;
				}

				Landis.Model.Run(args[0]);
				return 0;
			}
			catch (ApplicationException exc) {
				UI.WriteLine(exc.Message);
				return 1;
			}
			catch (Exception exc) {
				UI.WriteLine("Internal error occurred within the program:");
				UI.WriteLine("  {0}", exc.Message);
				if (exc.InnerException != null) {
					UI.WriteLine("  {0}", exc.InnerException.Message);
				}
				UI.WriteLine();
				UI.WriteLine("Stack trace:");
				UI.WriteLine(exc.StackTrace);
				return 1;
			}
		}
	}
}
