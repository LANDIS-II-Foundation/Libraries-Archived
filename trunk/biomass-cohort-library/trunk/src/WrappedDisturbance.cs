//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using Landis.Core;

namespace Landis.Library.BiomassCohorts
{
    /// <summary>
    /// A wrapped age-cohort disturbance so it works with biomass cohorts.
    /// </summary>
    public class WrappedDisturbance
        : IDisturbance
    {
        private AgeOnlyCohorts.ICohortDisturbance ageCohortDisturbance;

        //---------------------------------------------------------------------

        public WrappedDisturbance(AgeOnlyCohorts.ICohortDisturbance ageCohortDisturbance)
        {
            this.ageCohortDisturbance = ageCohortDisturbance;
        }

        //---------------------------------------------------------------------

        public ExtensionType Type
        {
            get {
                return ageCohortDisturbance.Type;
            }
        }

        //---------------------------------------------------------------------

        public ActiveSite CurrentSite
        {
            get {
                return ageCohortDisturbance.CurrentSite;
            }
        }

        //---------------------------------------------------------------------

        public int RemoveMarkedCohort(ICohort cohort)
        {
            if (ageCohortDisturbance.MarkCohortForDeath(cohort)) {
                Cohort.KilledByAgeOnlyDisturbance(this, cohort,
                                                  ageCohortDisturbance.CurrentSite,
                                                  ageCohortDisturbance.Type);
                return cohort.Biomass;
            }
            else
                return 0;
        }
    }
}
