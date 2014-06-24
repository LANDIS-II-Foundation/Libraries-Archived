// This file is part of the Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest/trunk/

namespace Landis.Library.Harvest
{
    public static class Main
    {
        /// <summary>
        /// Initialize the library for use by client code.
        /// </summary>
        public static void InitializeLib()
        {
            AgeRangeParsing.InitializeClass();
        }
    }
}
