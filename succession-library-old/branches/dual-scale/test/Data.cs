using Edu.Wisc.Forest.Flel.Util;
using System.IO;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Test.Succession
{
    public static class Data
    {
        private static NUnitInfo myNUnitInfo = new NUnitInfo();

        //---------------------------------------------------------------------

        public static readonly string Directory = myNUnitInfo.GetDataDir();
        public const string DirPlaceholder = "{data folder}";

        public static string MakeInputPath(string filename)
        {
            return Path.Combine(Directory, filename);
        }

        //---------------------------------------------------------------------

        static Data()
        {
            Output.WriteLine("{0} = \"{1}\"", DirPlaceholder, Directory);
        }

        //---------------------------------------------------------------------

        private static TextWriter writer = myNUnitInfo.GetTextWriter();

        public static TextWriter Output
        {
            get {
                return writer;
            }
        }

        //---------------------------------------------------------------------

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
