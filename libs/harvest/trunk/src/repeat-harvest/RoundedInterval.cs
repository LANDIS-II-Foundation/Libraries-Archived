// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// The interval for a repeat-harvest that has been rounded up to the next
    /// multiple of the harvest timestep.
    /// </summary>
    public struct RoundedInterval
    {
        /// <summary>
        /// The interval before it was rounded up.
        /// </summary>
        public int Original;

        //---------------------------------------------------------------------

        /// <summary>
        /// The interval after it was rounded up.
        /// </summary>
        public int Adjusted;

        //---------------------------------------------------------------------

        /// <summary>
        /// The line number in the text input where the interval was read.
        /// </summary>
        public int LineNumber;

        //---------------------------------------------------------------------

        public RoundedInterval(int originalInterval,
                               int adjustedInterval,
                               int lineNumber)
        {
            this.Original = originalInterval;
            this.Adjusted = adjustedInterval;
            this.LineNumber = lineNumber;
        }
    }
}
