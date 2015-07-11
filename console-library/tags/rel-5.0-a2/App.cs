using System;

namespace Landis
{
	public static class App
	{
		public static int Main(string[] args)
		{
			try {
				Console.WriteLine("Landis-II 5.0 alpha-2");
				Console.WriteLine("Copyright 2004-2005 University of Wisconsin");
				Console.WriteLine();
	
				if (args.Length == 0) {
					Console.Error.WriteLine("Error: No scenario file specified.");
					return 1;
				}
				if (args.Length > 1) {
					Console.Error.WriteLine("Error: Extra argument(s) on command line:");
					Console.Error.Write(    " ");
					for (int i = 1; i < args.Length; ++i)
						Console.Error.Write(" {0}", args[i]);
					Console.Error.WriteLine();
					return 1;
				}

				Landis.Model.Run(args[0]);
				return 0;
			}
			catch (ApplicationException exc) {
				Console.Error.WriteLine(exc.Message);
				return 1;
			}
			catch (Exception exc) {
				Console.Error.WriteLine("Internal error occurred within the program:");
				Console.Error.WriteLine("  {0}", exc.Message);
				Console.Error.WriteLine();
				Console.Error.WriteLine("Stack trace:");
				Console.Error.WriteLine(exc.StackTrace);
				return 1;
			}
		}
	}
}
