using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using System.Collections.Generic;

namespace Landis.Test.Succession
{
    //  Data for testing disturbed sites
    public static class DisturbedSites
    {
        private static ILandscape mixedLandscape;
        private static Location[] locations;

        //---------------------------------------------------------------------

        static DisturbedSites()
        {
                            // columns:    123456789
            string[] rows = new string[]{ "---------",    // row 1
                                          "---aaDa--",    // row 2
                                          "--aDDaa--",    // row 3
                                          "--aaaaaa-",    // row 4
                                          "-aaa--DD-",    // row 5
                                          "-Da---aaa",    // row 6
                                          "--aa--D--"};   // row 7
            bool[,] array = Bool.Make2DimArray(rows, "aD");
            DataGrid<bool> grid = new DataGrid<bool>(array);
            mixedLandscape = new Landscape.Landscape(grid);

            List<Location> locList = new List<Location>();
            foreach (ActiveSite site in mixedLandscape) {
                int row = (int) (site.Location.Row);
                int column = (int) (site.Location.Column);
                if (rows[row-1][column-1] == 'D')
                    locList.Add(site.Location);
            }
            locations = locList.ToArray();
        }

        //---------------------------------------------------------------------

        public static ILandscape MixedLandscape
        {
            get {
                return mixedLandscape;
            }
        }

        //---------------------------------------------------------------------

        public static Location[] Locations
        {
            get {
                return locations;
            }
        }
    }
}
