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
using Landis.Core;

namespace Landis.Library.Succession.DemographicSeeding
{
    /// <summary>
    /// A parser that reads the extension's input and output parameters from
    /// a text file.
    /// </summary>
    public class ParameterParser
        : TextParser<Parameters>
    {
        static ParameterParser()
        {
            ParsingUtils.RegisterForInputValues();
        }

        //---------------------------------------------------------------------

        public override string LandisDataValue
        {
            get {
                return "Demographic Seeding";
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ParameterParser()
        {
        }

        //---------------------------------------------------------------------

        protected override Parameters Parse()
        {
            ReadLandisDataVar();

            Parameters parameters = new Parameters();

            InputVar<Seed_Dispersal.Dispersal_Model> kernel = new InputVar<Seed_Dispersal.Dispersal_Model>("Kernel");
            ReadVar(kernel);
            parameters.Kernel = kernel.Value;

            InputVar<int> monteCarloDraws = new InputVar<int>("MonteCarloDraws");
            ReadVar(monteCarloDraws);
            parameters.MonteCarloDraws = monteCarloDraws.Value;

            InputVar<double> maxLeafArea = new InputVar<double>("MaxLeafArea");
            if (ReadOptionalVar(maxLeafArea))
                parameters.MaxLeafArea = maxLeafArea.Value;
            else
                parameters.MaxLeafArea = Model.Core.CellArea;

            InputVar<int> cohortThreshold = new InputVar<int>("CohortThreshold");
            ReadVar(cohortThreshold);
            parameters.CohortThreshold = cohortThreshold.Value;

            return parameters;
        }
    }
}