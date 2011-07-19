using System;
using System.Reflection;

namespace Landis
{
	class MainClass
	{
		private static string succPlugInName;
		private static string outPlugInName;
		private static string disturbPlugInName;

		//---------------------------------------------------------------------

		public static int Main(string[] args)
		{
			try {
				ProcessCmdLine(args);
			}
			catch (ApplicationException e) {
				Console.WriteLine("Error using program:");
				Console.WriteLine("  {0}", e.Message);
				return 1;
			}
			string category = "";
			try {
				PlugIn.Manager manager = new PlugIn.Manager();

				Console.WriteLine("Loading Succession plug-in {0}",
				                  succPlugInName);
				category = "succession";
				Succession.IPlugIn succPlugIn =
					manager.LoadPlugIn<Succession.IPlugIn>(succPlugInName);
				succPlugIn.Initialize(new Succession.Settings(5));

				Console.WriteLine("Loading Output plug-in {0}",
				                  outPlugInName);
				category = "output";
				Output.IPlugIn outPlugIn =
					manager.LoadPlugIn<Output.IPlugIn>(outPlugInName);
				outPlugIn.Initialize(new Landis.Output.Settings());

				Disturbance.IPlugIn disturbPlugIn = null;
				if (disturbPlugInName != null) {
					Console.WriteLine("Loading Disturbance plug-in {0}",
					                  disturbPlugInName);
					category = "disturbance";
					disturbPlugIn =
						manager.LoadPlugIn<Disturbance.IPlugIn>(disturbPlugInName);
					disturbPlugIn.Initialize();
				}

				string[] rows = new string[] { "........",
											   ".XXXXXX.",
											   ".....X..",
											   "....X...",
											   "...X....",
											   "..X.....",
											   ".XXXXXX.",
											   "........" };
				bool[,] array = Util.Bool.Make2DimArray(rows, "X");
				Landscape.DataGrid<bool> grid = new Landscape.DataGrid<bool>(array);
				Landscape.Landscape landscape = new Landscape.Landscape(grid);

				succPlugIn.AddSiteVars(landscape);
				outPlugIn.AddSiteVars(landscape);
				if (disturbPlugIn != null)
					disturbPlugIn.AddSiteVars(landscape);

				for (int timestep = 1; timestep < 50; ++timestep) {
					if (succPlugIn.NextTimestep == timestep) {
						Console.WriteLine("timestep = {0}", timestep);
						succPlugIn.Initialize(timestep);
						succPlugIn.Run(timestep, landscape);
					}
					if (outPlugIn.NextTimestep == timestep) {
						outPlugIn.Run(timestep);
					}
					if (disturbPlugIn != null &&
					    		disturbPlugIn.NextTimestep == timestep) {
						disturbPlugIn.Run(timestep);
					}
				}

				return 0;
			}
			catch (PlugIn.NotInstalledException) {
				Console.WriteLine("  No plug-in is installed with that name.");
			}
			catch (PlugIn.CategoryException) {
				Console.WriteLine("  The plug-in is not a {0} plug-in.",
				                  category);
			}
			catch (PlugIn.Exception e) {
				Console.WriteLine("  {0}", e.Message);
			}
			catch (System.Exception e) {
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
			return 1;
		}

		//---------------------------------------------------------------------

		private static void ProcessCmdLine(string[] args)
		{
			if (args.Length == 0)
				throw new ApplicationException("No succession plug-in named");
			succPlugInName = args[0];

			if (args.Length == 1)
				throw new ApplicationException("No output plug-in named");
			outPlugInName = args[1];

			if (args.Length == 2)
				disturbPlugInName = null;
			else if (args.Length == 3)
				disturbPlugInName = args[2];
			else if (args.Length > 3)
				throw new ApplicationException("Too many arguments");
		}
	}
}
