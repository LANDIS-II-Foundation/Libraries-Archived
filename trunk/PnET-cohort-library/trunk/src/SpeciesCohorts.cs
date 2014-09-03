//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;
using System.Collections;
using System.Collections.Generic;


namespace Landis.Library.BiomassCohortsPnET
{
    /// <summary>
    /// The cohorts for a particular species at a site.
    /// </summary>
    public class SpeciesCohorts : BiomassCohorts.ISpeciesCohorts, AgeOnlyCohorts.ISpeciesCohorts, IEnumerable<Cohort>
    {
        private static Landis.Library.AgeOnlyCohorts.SpeciesCohortBoolArray isSpeciesCohortDamaged;
       
        private List<Cohort> cohorts;

        public ISpecies Species { get; private set; }

        public int Count
        {
            get
            {
                return cohorts.Count;
            }
        }
       
 
        public void RemoveCohorts(Landis.Library.AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            isSpeciesCohortDamaged.SetAllFalse(Count);
            disturbance.MarkCohortsForDeath(this, isSpeciesCohortDamaged);

            //  Go backwards through list of ages, so the removal of an age
            //  doesn't mess up the loop.
             
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                if (isSpeciesCohortDamaged[i])
                {
                    cohorts.RemoveAt(i);
                    Cohort.Died(this, cohorts[i], disturbance.CurrentSite, disturbance.Type);
                                
                }
            }
        }
        
        public bool IsMaturePresent 
        {
            get
            {
                for (int i = 0; i < cohorts.Count; i++)
                {
                    Cohort Cohorts = cohorts[i];
                    if (Cohorts.Age > Species.Maturity)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        
       
        public SpeciesCohorts(Cohort c)
        {
            this.Species = c.Species;
            this.cohorts = new List<Cohort>();
            AddCohort(c);
            isSpeciesCohortDamaged = new Landis.Library.AgeOnlyCohorts.SpeciesCohortBoolArray();
        }
        public void AddCohort(Cohort c)
        {
             this.cohorts.Add(c);
           
        }
        
        //---------------------------------------------------------------------
        public void RemoveCohort(Cohort cohort,
                                  ActiveSite site,
                                  ExtensionType disturbanceType)
        {
            cohorts.Remove(cohort);
            Cohort.Died(this, cohort, site, disturbanceType);
        }
        public int MarkCohorts(AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            isSpeciesCohortDamaged.SetAllFalse(Count);
            disturbance.MarkCohortsForDeath(this, isSpeciesCohortDamaged);

            //  Go backwards through list of cohort data, so the removal of an
            //  item doesn't mess up the loop.
            int totalReduction = 0;
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                if (isSpeciesCohortDamaged[i])
                {
                    totalReduction += cohorts[i].Biomass;

                    Landis.Library.BiomassCohorts.Cohort.KilledByAgeOnlyDisturbance(this, cohorts[i], disturbance.CurrentSite, disturbance.Type);

                    RemoveCohort(cohorts[i], disturbance.CurrentSite, disturbance.Type);

                }
            }
            return totalReduction;
        }
        /*
        public int MarkCohorts(Landis.Library.BiomassCohorts.IDisturbance disturbance)
        {
            //  Go backwards through list of cohort data, so the removal of an
            //  item doesn't mess up the loop.
            int totalReduction = 0;
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                Cohort cohort = cohorts[i];
                int reduction = disturbance.ReduceOrKillMarkedCohort(cohort);
                if (reduction > 0)
                {
                    totalReduction += reduction;
                    if (reduction < cohort.Biomass)
                    {
                        float fRed = reduction / cohort.Biomass;
                        cohort.Wood *= fRed;
                        cohort.Fol *= fRed;
                    }
                    else
                    {
                        RemoveCohort(cohort, disturbance.CurrentSite, disturbance.Type);
                        cohort = null;
                    }
                }
                 
            }
            return totalReduction;
        }
        */
        
        IEnumerator<Landis.Library.BiomassCohortsPnET.Cohort> IEnumerable<Landis.Library.BiomassCohortsPnET.Cohort>.GetEnumerator()
        {
            foreach (Cohort data in cohorts)
                yield return data;
        }
        
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Cohort>)this).GetEnumerator();
        }
       
        IEnumerator<Landis.Library.BiomassCohorts.ICohort> IEnumerable<Landis.Library.BiomassCohorts.ICohort>.GetEnumerator()
        {
            foreach (Cohort cohort in cohorts)
                yield return cohort;
        }
        IEnumerator<Landis.Library.AgeOnlyCohorts.ICohort> IEnumerable<Landis.Library.AgeOnlyCohorts.ICohort>.GetEnumerator()
        {
            foreach (Cohort cohort in cohorts)
                yield return cohort;
        }
         
    }
}
