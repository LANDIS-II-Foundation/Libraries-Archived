using System;
using System.Configuration;

namespace Landis.Test
{
    /// <summary>
    /// Information related NUnit testing framework.  This is a workaround for
    /// the Edu.Wisc.Forest.Flel.Util.NUnitInfo class, so as not to require
    /// any environment variables to be set.
    /// </summary>
    public class NUnitInfo
    {

        /// <summary>
        /// Gets the directory with data files for the test assembly.  This is
        /// specified in the assembly's configuration file in the setting
        /// called "test data directory".
        /// </summary>
        public string GetDataDir()
        {
			string dataDir = ConfigurationManager.AppSettings["test data directory"];
            if (dataDir == null)
                throw new ApplicationException("Cannot retrieve data-directory setting from configuration file");
            return dataDir;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets a System.IO.TextWriter to which NUnit tests can write console
        /// output.
        /// </summary>
        /// <remarks>
        /// The typical usage scenarios are to run the unit tests from inside
        /// an IDE (e.g., MonoDevelop) or with the NUnit GUI (on Windows).
        /// In both situations, there is a separate panel for
        /// System.Console.Out, so that is returned.
        /// </remarks>
        public System.IO.TextWriter GetTextWriter()
        {
            return Console.Out;
        }
    }
}
