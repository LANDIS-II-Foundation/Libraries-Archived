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

        public int ReduceOrKillMarkedCohort(ICohort cohort)
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

        //---------------------------------------------------------------------
        public int ChangeForage(ICohort cohort)
        {
            return 0;
        }
        //---------------------------------------------------------------------
        public void UpdateForage(ActiveSite site)
        {
        }
        //---------------------------------------------------------------------
        public int ChangeForageInReach(ICohort cohort)
        {
            return 0;
        }
        //---------------------------------------------------------------------
        public void UpdateForageInReach(ActiveSite site)
        {
        }
        //---------------------------------------------------------------------
        public double ChangeLastBrowseProp(ICohort cohort)
        {
            return 0;
        }
        //---------------------------------------------------------------------
        public void UpdateLastBrowseProp(ActiveSite site)
        {
        }
        //---------------------------------------------------------------------
    }
}
