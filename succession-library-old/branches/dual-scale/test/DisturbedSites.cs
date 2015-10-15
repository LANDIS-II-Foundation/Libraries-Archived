using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Grids;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

using Location = Wisc.Flel.GeospatialModeling.Landscapes.DualScale.Location;

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
            int rowCount = array.GetLength(0);
            int colCount = array.GetLength(1);
            DataGrid<EcoregionCode> grid = new DataGrid<EcoregionCode>(rowCount, colCount);
            for (int row = 1; row <= rowCount; row++) {
                for (int col = 1; col <= colCount; col++) {
                    if (array[row-1, col-1])
                        grid[row, col] = new EcoregionCode(1, true);
                    else    
                        grid[row, col] = new EcoregionCode(0, false);
                }
            }
            mixedLandscape = new Landscape(grid, 1);

            List<Location> locList = new List<Location>();
            foreach (ActiveSite site in mixedLandscape) {
                int row = site.Location.Row;
                int column = site.Location.Column;
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
