// This file is part of the Site Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/site-harvest/trunk/

using Landis.Core;
using System.Collections.Generic;

namespace Landis.Library.SiteHarvest
{
    /// <summary>
    /// Tracks the # of cohorts that have been cut for each species.
    /// </summary>
    public class CohortCounts
    {
        private IDictionary<ISpecies, int> counts;
        private int allSpecies;  // total # for all species

        /// <summary>
        /// Creates a new instance with all counts initialized to 0.
        /// </summary>
        public CohortCounts()
        {
            counts = new Dictionary<ISpecies, int>();
            Reset();
        }

        /// <summary>
        /// Current count for a species.
        /// </summary>
        public int this[ISpecies species]
        {
            get { return counts[species]; }
        }

        /// <summary>
        /// The current count for all species combined.
        /// </summary>
        public int AllSpecies
        {
            get { return allSpecies; }
        }

        /// <summary>
        /// Increment the count for a species by 1.
        /// </summary>
        public void IncrementCount(ISpecies species)
        {
            counts[species]++;
            allSpecies++;
        }

        /// <summary>
        /// Increment the counts for all the species.
        /// </summary>
        /// <param name="increments">
        /// The increment for each species.
        /// </param>
        public void IncrementCounts(CohortCounts increments)
        {
            foreach (ISpecies species in Model.Core.Species)
            {
                int increment = increments[species];
                counts[species] += increment;
                allSpecies += increment;
            }
        }

        /// <summary>
        /// Reset all the counts to 0.
        /// </summary>
        public void Reset()
        {
            foreach (ISpecies species in Model.Core.Species)
                counts[species] = 0;
            allSpecies = 0;
        }
    }
}
