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
    /// Species parameters for demographic seeding that the user can specify.
    /// </summary>
    public class SpeciesParameters
    {
        private int minSeedsProduced;
        private int maxSeedsProduced;
        private double leafArea;
        public double[] DispersalParameters { get; private set; }
        public double[] EmergenceProbabilities { get; private set; }
        public double[] SurvivalProbabilities { get; private set; }

        // Indexes for dispersal parameters
        public const int DoubleExponential_Mean1   = 0;
        public const int DoubleExponential_Mean2   = 1;
        public const int DoubleExponential_Weight1 = 2;

        //---------------------------------------------------------------------

        public SpeciesParameters()
        {
            DispersalParameters = new double[3];
            EmergenceProbabilities = new double[Model.Core.Ecoregions.Count];
            SurvivalProbabilities = new double[Model.Core.Ecoregions.Count];
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Minimum number of seeds produced by an occupied cell in a year.
        /// </summary>
        public int MinSeedsProduced
        {
            get
            {
                return minSeedsProduced;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                                                  "Minimum seeds produced must be = or > 0");
                minSeedsProduced = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum number of seeds produced by an occupied cell in a year.
        /// </summary>
        public int MaxSeedsProduced
        {
            get
            {
                return maxSeedsProduced;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                                                  "Maximum seeds produced must be = or > 0");
                maxSeedsProduced = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Projected canopy area of a “typical” seedling.
        /// </summary>
        public double LeafArea
        {
            get
            {
                return leafArea;
            }
            set
            {
                if (value <= 0)
                    throw new InputValueException(value.ToString(),
                                                  "Seedling leaf area must be > 0");
                leafArea = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Mean1 parameter for 2-component exponential dispersal kernel
        /// </summary>
        public double DispersalMean1
        {
            get
            {
                return DispersalParameters[DoubleExponential_Mean1];
            }
            set
            {
                if (value < 0.0)
                    throw new InputValueException(value.ToString(),
                                                  "Dispersal Mean1 must be = or > 0.0");
                DispersalParameters[DoubleExponential_Mean1] = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Mean2 parameter for 2-component exponential dispersal kernel
        /// </summary>
        public double DispersalMean2
        {
            get
            {
                return DispersalParameters[DoubleExponential_Mean2];
            }
            set
            {
                if (value < 0.0)
                    throw new InputValueException(value.ToString(),
                                                  "Dispersal Mean2 must be = or > 0.0");
                DispersalParameters[DoubleExponential_Mean2] = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Weight1 parameter for 2-component exponential dispersal kernel
        /// </summary>
        public double DispersalWeight1
        {
            get
            {
                return DispersalParameters[DoubleExponential_Weight1];
            }
            set
            {
                if (value < 0.0)
                    throw new InputValueException(value.ToString(),
                                                  "Dispersal Weight1 must be = or > 0.0");
                DispersalParameters[DoubleExponential_Weight1] = value;
            }
        }
    }
}
