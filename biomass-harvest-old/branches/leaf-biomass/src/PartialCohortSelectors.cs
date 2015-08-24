// This file is part of the Biomass Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/biomass-harvest/trunk/

using Landis.Core;
using System.Collections.Generic;

namespace Landis.Library.BiomassHarvest
{
    /// <summary>
    /// A set of specific-ages cohort selectors with percentages.
    /// </summary>
    /// <remarks>
    /// A convenience class to improve code readability.
    /// </remarks>
    public class PartialCohortSelectors
        : Dictionary<ISpecies, SpecificAgesCohortSelector>
    {
        /// <summary>
        /// Creates a new instance with no selectors initially.
        /// </summary>
        public PartialCohortSelectors()
            : base()
        {
        }

        //----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance with a set of selectors.
        /// </summary>
        public PartialCohortSelectors(PartialCohortSelectors partialCohortSelectors)
            : base(partialCohortSelectors)
        {
        }
    }
}
