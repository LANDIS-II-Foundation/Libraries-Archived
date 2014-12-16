﻿// Copyright 2014 University of Notre Dame
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

using Landis.Core;
using Landis.SpatialModeling;
using Seed_Dispersal;

namespace Landis.Library.Succession.DemographicSeeding
{
    public class Algorithm
    {
        private Seed_Dispersal.Map seedingData;

        /// <summary>
        /// Initializes the demographic seeding algorithm
        /// </summary>
        /// <param name="successionTimestep">
        /// The length of the succession extension's timestep (units: years).
        /// </param>
        public Algorithm(int successionTimestep)
        {
            int numTimeSteps;  // the number of succession time steps to loop over
            int maxCohortAge;  // maximum age allowed for any species, in years

            numTimeSteps = (Model.Core.EndTime - Model.Core.StartTime) / successionTimestep;
            maxCohortAge = 0;
            foreach (ISpecies species in Model.Core.Species)
                if (species.Longevity > maxCohortAge)
                    maxCohortAge = species.Longevity;

            // The library's code comments say max_age_steps represents
            // "maximum age allowed for any species, in years", but it's
            // used in the code as though it represents the maximum age of
            // any species' cohort IN NUMBER OF SUCCESSION TIMESTEPS.  So if
            // the oldest species' Longevity is 2,000 years, and the succession
            // timestep is 10 years, then max_age_steps is 200 timesteps.
            int max_age_steps = maxCohortAge / successionTimestep;

            seedingData = new Seed_Dispersal.Map(
                Model.Core.Landscape.Columns,
                Model.Core.Landscape.Rows,
                Model.Core.Species.Count,
                numTimeSteps,
                successionTimestep,
                Model.Core.Ecoregions.Count,
                max_age_steps);
            seedingData.pixel_size = Model.Core.CellLength;

            // Initialize some species parameters from the core.
            foreach (ISpecies species in Model.Core.Species)
            {
                seedingData.all_species[species.Index].shade_tolerance = species.ShadeTolerance;
                seedingData.all_species[species.Index].reproductive_age = species.Maturity;
            }

            // Load user-specified parameters
            string path = "demographic-seeding.txt";  // hard-wired for now, so no changes required to succession extensions
            Model.Core.UI.WriteLine("Reading demographic seeding parameters from {0} ...", path);
            ParameterParser parser = new ParameterParser(Model.Core.Species);
            Parameters parameters = Landis.Data.Load<Parameters>(path, parser);

            seedingData.dispersal_model  = parameters.Kernel;
            seedingData.mc_draws         = parameters.MonteCarloDraws;
            seedingData.max_leaf_area    = parameters.MaxLeafArea;
            seedingData.cohort_threshold = parameters.CohortThreshold;

            foreach (ISpecies species in Model.Core.Species)
            {
                SpeciesParameters speciesParameters = parameters.SpeciesParameters[species.Index]; 
                seedingData.all_species[species.Index].min_seed  = speciesParameters.MinSeedsProduced;
                seedingData.all_species[species.Index].max_seed  = speciesParameters.MaxSeedsProduced;
                seedingData.all_species[species.Index].leaf_area = speciesParameters.LeafArea;
                for (int i = 0; i < speciesParameters.DispersalParameters.Length; i++)
                    seedingData.all_species[species.Index].dispersal_parameters[i] = speciesParameters.DispersalParameters[i];
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Seeding algorithm: determines if a species seeds a site.
        /// <param name="species"></param>
        /// <param name="site">Site that may be seeded.</param>
        /// <returns>true if the species seeds the site.</returns>
        public bool DoesSpeciesSeedSite(ISpecies   species,
                                        ActiveSite site)
        {
            throw new System.NotImplementedException();
        }
    }
}