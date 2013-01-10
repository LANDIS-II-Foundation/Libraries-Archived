using System.IO;

namespace Landis.Library.LandUses.UnitTests
{
    public static class Data
    {
        public const string DirPlaceholder = "{data folder}";

        private static string directory;

        /// <summary>
        /// The directory with the test data.
        /// </summary>
        public static string Directory
        {
            get {
                return directory;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize the path to the directory with the test data.
        /// </summary>
        static Data()
        {
            directory = Path.Combine(ProjectInfo.Directory, "data");

            Output.WriteLine("{0} = \"{1}\"", DirPlaceholder, Directory);
            Output.WriteLine();
        }

        //---------------------------------------------------------------------

        public static string MakeInputPath(string filename)
        {
            return Path.Combine(Directory, filename);
        }

        //---------------------------------------------------------------------

        public static TextWriter Output
        {
            get {
                return System.Console.Out;
            }
        }
    }
}
