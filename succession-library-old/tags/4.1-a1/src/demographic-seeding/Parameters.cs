// Copyright 2014 University of Notre Dame
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Library.Succession.DemographicSeeding
{
    /// <summary>
    /// Parameters for demographic seeding that the user can specify.
    /// </summary>
    public class Parameters
    {
        public int monteCarloDraws;
        private double maxLeafArea;
        private int cohortThreshold;

        //---------------------------------------------------------------------

        /// <summary>
        /// Identifies a particular dispersal kernel
        /// </summary>
        public Seed_Dispersal.Dispersal_Model Kernel { get; set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// Identifies a particular seed production model
        /// </summary>
        public Seed_Dispersal.Seed_Model SeedProductionModel { get; set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of Monte Carlo draws to use when estimating dispersal
        /// probabilities.
        /// </summary>
        public int MonteCarloDraws
        {
            get
            {
                return monteCarloDraws;
            }
            set
            {
                if (value <= 0)
                    throw new InputValueException(value.ToString(),
                                                  "Monte Carlo draws must be > 0");
                monteCarloDraws = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum projected seedling canopy area that can be supported in a
        /// cell (default to cellsize^2)
        /// </summary>
        public double MaxLeafArea
        {
            get
            {
                return maxLeafArea;
            }
            set
            {
                if (value <= 0.0 || value > Model.Core.CellArea)
                    throw new InputValueException(value.ToString(),
                                                  string.Format("Max leaf area must be > 0 and <= CellArea ({0})",
                                                                Model.Core.CellArea));
                maxLeafArea = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Minimum number of trees to establish a cohort under the default
        /// succession model.
        /// </summary>
        public int CohortThreshold
        {
            get
            {
                return cohortThreshold;
            }
            set
            {
                if (value <= 0)
                    throw new InputValueException(value.ToString(),
                                                  "Cohort threshold must be > 0");
                cohortThreshold = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Species parameters related to demographic seeding.
        /// </summary>
        public SpeciesParameters[] SpeciesParameters { get; set; }
    }
}
