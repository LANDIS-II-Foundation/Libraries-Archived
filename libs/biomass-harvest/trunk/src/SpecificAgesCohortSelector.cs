/*
 * Copyright 2008 Green Code LLC
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
using Landis.Biomass;
using Landis.Harvest;
using Landis.Species;
using System.Collections.Generic;

using AgeCohorts = Landis.AgeCohort;

namespace Landis.Extension.BiomassHarvest
{
    /// <summary>
    /// Selects specific ages and ranges of ages among a species' cohorts
    /// for harvesting.
    /// </summary>
    public class SpecificAgesCohortSelector
    {
        private static Percentage defaultPercentage;

        private IList<ushort> ages;
        private IList<AgeRange> ranges;
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
            this.ages = new List<ushort>(ages);
            this.ranges = new List<AgeRange>(ranges);
            this.percentages = new Dictionary<ushort, Percentage>(percentages);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects which of a species' cohorts are harvested.
        /// </summary>
        public void SelectCohorts(AgeCohorts.ISpeciesCohorts         cohorts,
                                  AgeCohorts.ISpeciesCohortBoolArray isHarvested)
        {
            int i = 0;
            foreach (ICohort cohort in ((ISpeciesCohorts) cohorts)) {
                bool cohortSelected = false;
                ushort ageToLookUp = 0;
                if (ages.Contains(cohort.Age)) {
                    cohortSelected = true;
                    ageToLookUp = cohort.Age;
                }
                else {
                    foreach (AgeRange range in ranges) {
                        if (range.Contains(cohort.Age)) {
                            cohortSelected = true;
                            ageToLookUp = range.Start;
                            break;
                        }
                    }
                }
                if (cohortSelected) {
                    Percentage percentage;
                    if (! percentages.TryGetValue(ageToLookUp, out percentage))
                        percentage = defaultPercentage;
                    int reduction = (int) System.Math.Round(cohort.Biomass * percentage);
                    if (reduction < cohort.Biomass)
                        PartialHarvestDisturbance.RecordBiomassReduction(cohort, reduction);
                    else
                    {
                        isHarvested[i] = true;
                        PartialHarvestDisturbance.RecordBiomassReduction(cohort, reduction);
                    }
                }
                i++;
            }
        }
    }
}
