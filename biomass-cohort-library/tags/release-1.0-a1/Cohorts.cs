using Landis.Cohorts;

namespace Landis.Biomass
{
	/// <summary>
	/// Methods for biomass cohorts.
	/// </summary>
	public static class Cohorts
	{
	    private static int successionTimeStep;
	    private static CohortDeathMethod deathMethod;
	    private static ICalculator biomassCalculator;

        //---------------------------------------------------------------------

        /// <summary>
        /// The succession time step used by biomass cohorts.
        /// </summary>
        public static int SuccessionTimeStep
        {
            get {
                return successionTimeStep;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The method that's called when a cohort dies.
        /// </summary>
        public static CohortDeathMethod CohortDeath
        {
            get {
                return deathMethod;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The calculator for computing how a cohort's biomass changes.
        /// </summary>
        public static ICalculator BiomassCalculator
        {
            get {
                return biomassCalculator;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the biomass-cohorts module.
        /// </summary>
        /// <param name="successionTimeStep">
        /// The time step for the succession extension.  Unit: years
        /// </param>
        /// <param name="deathMethod">
        /// The method to call when a cohort dies.
        /// </param>
        /// <param name="biomassCalculator">
        /// The calculator for computing the change in a cohort's biomass due
        /// to growth and mortality.
        /// </param>
	    public static void Initialize(int               successionTimeStep,
                                      CohortDeathMethod deathMethod,
                                      ICalculator       biomassCalculator)
	    {
	        Cohorts.successionTimeStep = successionTimeStep;
	        Cohorts.deathMethod        = deathMethod;
	        Cohorts.biomassCalculator  = biomassCalculator;
	    }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the total biomass for all the cohorts at a site.
        /// </summary>
        public static int ComputeBiomass(ISiteCohorts<ICohort> siteCohorts)
        {
            int youngBiomass;
            return ComputeBiomass(siteCohorts, out youngBiomass);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the total biomass for all the cohorts at a site, and the
        /// total biomass for all the young cohorts.
        /// </summary>
        public static int ComputeBiomass(ISiteCohorts<ICohort> siteCohorts,
                                         out int               youngBiomass)
        {
            youngBiomass = 0;
            int totalBiomass = 0;
            foreach (ISpeciesCohorts<ICohort> speciesCohorts in siteCohorts) {
                foreach (ICohort cohort in speciesCohorts) {
                    totalBiomass += cohort.Biomass;
                    if (cohort.Age < successionTimeStep)
                        youngBiomass += cohort.Biomass;
                }
            }
            return totalBiomass;
        }
    }
}
