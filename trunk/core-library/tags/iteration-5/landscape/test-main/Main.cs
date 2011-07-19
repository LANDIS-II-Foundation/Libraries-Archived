using Landis.Landscape;
using Landis.Util;
using System;

namespace Landis.Test
{
	class MainClass
	{
		private static bool[,] activeSites;
		private static DataGrid<bool> grid;
		private static Landis.Landscape.Landscape landscape;

		//---------------------------------------------------------------------

		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			string[] rows = new string[] {	"....X..",
											"...XX.X",
											".......",
											"...XXX.",
											"X.X.X.X",
											"XXXXXXX" };
			activeSites = Bool.Make2DimArray(rows, "X");
			grid = new DataGrid<bool>(activeSites);
			landscape = new Landscape.Landscape(grid);
			foreach (ActiveSite site in landscape) {
				System.Console.WriteLine("{0} : index = {1}",
				                         site.Location, site.DataIndex);
			}
		}
	}
}
