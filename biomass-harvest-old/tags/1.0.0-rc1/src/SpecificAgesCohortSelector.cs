/*
 * Copyright 2008 Green Code LLC
 * Copyright 2014 University of Notre Dame
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.BiomassCohorts;
using Landis.Library.SiteHarvest;
using System.Collections.Generic;

namespace Landis.Library.BiomassHarvest
{
    /// <summary>
    /// Selects specific ages and ranges of ages among a species' cohorts
    /// for harvesting.
    /// </summary>
    public class SpecificAgesCohortSelector
    {
        private static Percentage defaultPercentage;

        private AgesAndRanges agesAndRanges;
        private IDictionary<ushort, Percentage> percentages;

        //---------------------------------------------------------------------

        static SpecificAgesCohortSelector()
        {
            defaultPercentage = Percentage.Parse("100%");
        }

        //---------------------------------------------------------------------

        public SpecificAgesCohortSelector(IList<ushort>                   ages,
                                          IList<AgeRange>                 ranges,
                                          IDictionary<ushort, Percentage> percentages)
        {
            agesAndRanges = new AgesAndRanges(ages, ranges);
            this.percentages = new Dictionary<ushort, Percentage>(percentages);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects which cohorts are harvested.
        /// </summary>
        /// <returns>
        /// true if the given cohort is to be harvested.  The cohort's biomass
        /// should be reduced by the percentage returned in the second
        /// parameter.
        /// </returns>
        public bool Selects(ICohort cohort, out Percentage percentage)
        {
                ushort ageToLookUp = 0;
                AgeRange? containingRange;
                if (agesAndRanges.Contains(cohort.Age, out containingRange))
                {
                    if (! containingRange.HasValue)
                        ageToLookUp = cohort.Age;
                    else {
                        ageToLookUp = containingRange.Value.Start;
                    }
                    if (! percentages.TryGetValue(ageToLookUp, out percentage))
                        percentage = defaultPercentage;
                    return true;
                }
                percentage = null;
                return false;
        }
    }
}
