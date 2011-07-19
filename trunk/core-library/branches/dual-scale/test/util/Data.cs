// Copyright 2007 University of Wisconsin
// Author: James Domingo, UW-Madison, Forest Landscape Ecology Lab

using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Test.Util
{
    public static class Data
    {
        // Generate a 2-D array of ecoregion codes from an 2-D int array.
        // Negative integers are considered inactive ecoregion codes (code =
        // absolute value of the negative integer).
        public static EcoregionCode[,] MakeEcoregionCodes(int[,] ecoregions)
        {
            int rows = ecoregions.GetLength(0);
            int columns = ecoregions.GetLength(1);
            EcoregionCode[,] codes = new EcoregionCode[rows, columns];
            for (int row = 0; row < rows; row++) {
                for (int column = 0; column < columns; column++) {
                    int code = ecoregions[row, column];
                    if (code < 0)
                        codes[row, column] = new EcoregionCode((ushort) -code, false);
                    else
                        codes[row, column] = new EcoregionCode((ushort) code, true);
                }
            }
            return codes;
        }
    }
}
