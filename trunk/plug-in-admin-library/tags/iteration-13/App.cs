using Edu.Wisc.Forest.Flel.Util;
using System;
using System.Reflection;
using System.Text;

namespace Landis.PlugIns.Admin
{
	public static class App
	{
		public static int Main(string[] args)
		{
			try {
				Assembly plugInAssembly = Assembly.GetAssembly(typeof(Landis.PlugIns.IDataset));
				Console.WriteLine("Landis-II {0}", GetVersionAndRelease(plugInAssembly));
				
				Assembly myAssembly = Assembly.GetExecutingAssembly();
				Console.WriteLine("Extensions Administration Tool {0}", GetVersionAndRelease(myAssembly));

				Console.WriteLine("Copyright 2005 University of Wisconsin");
				Console.WriteLine();

				ICommand command = ParseArgs(args);
				if (command != null)
					command.Execute();
				return 0;
			}
			catch (ApplicationException exc) {
				Console.WriteLine(exc.Message);
				return 1;
			}
			catch (Exception exc) {
				Console.WriteLine("Internal error occurred within the program:");
				Console.WriteLine("  {0}", exc.Message);
				if (exc.InnerException != null) {
					Console.WriteLine("  {0}", exc.InnerException.Message);
				}
				Console.WriteLine();
				Console.WriteLine("Stack trace:");
				Console.WriteLine(exc.StackTrace);
				return 1;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets an assembly's version and release info.
		/// </summary>
		private static string GetVersionAndRelease(Assembly assembly)
		{
			Version version = assembly.GetName().Version;

			string release = AssemblyInfo.GetRelease(assembly);
			if (release == "official")
				release = "";
			else if (release.StartsWith("candidate"))
				release = string.Format(" (release {0})", release);
			else
				release = string.Format(" ({0} release)", release);

			return string.Format("{0}.{1}{2}", version.Major, version.Minor, release);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Parses the arguments passed to the program on the command line.
		/// </summary>
		/// <returns>
		/// A ICommand object which will perform the command (action) specified
		/// by the arguments.
		/// </returns>
		private static ICommand ParseArgs(string[] args)
		{
			if (args.Length == 0)
				throw UsageException("Expected one of these: list, add, remove");

			if (args[0] == "list") {
				if (args.Length > 1)
					throw ExtraArgsException(args, 1);
				return new ListCommand();
			}

			if (args[0] == "add") {
				if (args.Length == 1)
					throw UsageException("No filename for \"add\" command");
				if (args.Length > 2)
					throw ExtraArgsException(args, 2);
				return new AddCommand(args[1]);
			}

			if (args[0] == "remove") {
				if (args.Length == 1)
					throw UsageException("No extension name for \"remove\" command");
				if (args.Length > 2)
					throw ExtraArgsException(args, 2);
				return new RemoveCommand(args[1]);
			}

			throw UsageException("Unknown argument: {0} -- expected one of these: list, add, remove", args[0]);
		}

		//---------------------------------------------------------------------

		private static MultiLineException UsageException(string          message,
		                                                 params object[] mesgArgs)
		{
			string[] lines = { 
				"Error with arguments on command line:",
				"  " + string.Format(message, mesgArgs),
				"",
				"Usage:",
				"  landis-ii-extensions list", 
				"  landis-ii-extensions add {extension-info-file}",
				"  landis-ii-extensions remove {extension-name}"
			};
			return new MultiLineException(lines);
		}

		//---------------------------------------------------------------------

		private static MultiLineException ExtraArgsException(string[] args,
		                                                     int      firstExtraArgIndex)
		{
			StringBuilder message = new StringBuilder();
			if (firstExtraArgIndex == args.Length - 1)
				message.Append("Extra argument:");
			else
				message.Append("Extra arguments:");
			for (int i = firstExtraArgIndex; i < args.Length; ++i)
				message.AppendFormat(" {0}", args[i]);
			return UsageException(message.ToString());
		}
	}
}
