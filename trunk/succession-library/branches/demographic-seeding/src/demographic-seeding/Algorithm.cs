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
        }

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
