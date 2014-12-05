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
    public class SpeciesCohorts :  BiomassCohorts.ISpeciesCohorts, AgeOnlyCohorts.ISpeciesCohorts, IEnumerable<Cohort>
    {
        private static Landis.Library.AgeOnlyCohorts.SpeciesCohortBoolArray isSpeciesCohortDamaged;
       
        private List<Cohort> cohorts;

        public Cohort this[int i]
        {
            get
            {
                return cohorts[i];
            }
        }

        public ISpecies Species { get; private set; }

        public int Count
        {
            get
            {
                return cohorts.Count;
            }
        }
        
        //---------------------------------------------------------------------
        
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
        
       
        public SpeciesCohorts(ISpecies Species)
        {
            this.Species = Species;
            this.cohorts = new List<Cohort>();
            isSpeciesCohortDamaged = new Landis.Library.AgeOnlyCohorts.SpeciesCohortBoolArray();
        }
        public void AddCohort(Cohort c)
        {
             this.cohorts.Add(c);
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

                     
                    cohorts[i].IsAlive = false;
                    Cohort.Died(this, cohorts[i], disturbance.CurrentSite, disturbance.Type);

                }
            }
            return totalReduction;
        }
        
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
