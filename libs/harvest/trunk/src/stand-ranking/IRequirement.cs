// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A requirement that a stand must meet in order to be eligible for
    /// ranking.
    /// </summary>
    public interface IRequirement
    {
        /// <summary>
        /// Does a stand meet the requirement?
        /// </summary>
        bool MetBy(Stand stand);
    }
}
