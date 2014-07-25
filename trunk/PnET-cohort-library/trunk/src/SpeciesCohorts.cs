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
    public class SpeciesCohorts:  ISpeciesCohorts, BiomassCohorts.ISpeciesCohorts
    {
        private ISpecies species;
       
        //  Cohort data is in oldest to youngest order.
        private List<Cohort> cohorts;

        //---------------------------------------------------------------------

        public int Count
        {
            get {
                return cohorts.Count;
            }
        }

        //---------------------------------------------------------------------

        public ISpecies Species
        {
            get {
                return species;
            }
        }

        //---------------------------------------------------------------------

        public bool IsMaturePresent
        {
            get {
                foreach (Cohort c in cohorts)
                {
                    if (c.Age > species.Maturity) return true;
                }
                return false;
            }
        }

        //---------------------------------------------------------------------

        public ICohort this[int index]
        {
            get {
                return cohorts[index];
            }
             
        }

       //---------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance with one young cohort (age = 1).
        /// </summary>
        public SpeciesCohorts(ISpecies species)
        {
            this.species = species;
            this.cohorts = new List<Cohort>();
        }
       
        //---------------------------------------------------------------------
        public void AddNewCohort2(Cohort c)
        {
            this.cohorts.Add(c);
        }
        
         
        //---------------------------------------------------------------------
        public void IncrementCohortsAge()
        {
            foreach (Cohort d in cohorts)
            {
                d.IncrementAge();
            }
            
        }
      
        //---------------------------------------------------------------------
        public void RemoveCohort(Cohort cohort,
                                  ActiveSite site,
                                  ExtensionType disturbanceType)
        {
            
            cohorts.Remove(cohort);
            Cohort.Died(this, cohort, site, disturbanceType);
        }
        public Cohort Get(int index)
        {
            try
            {
                return cohorts[index];
            }
            catch
            {
                return null;
            }
        }
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
        
        IEnumerator<ICohort> IEnumerable<ICohort>.GetEnumerator()
        {
            foreach (Cohort data in cohorts)
                yield return data;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ICohort>)this).GetEnumerator();
        }
        
        IEnumerator<Landis.Library.BiomassCohorts.ICohort> IEnumerable<Landis.Library.BiomassCohorts.ICohort>.GetEnumerator()
        {
            foreach (Cohort data in cohorts)
                yield return new Landis.Library.BiomassCohorts.Cohort(species, data.Age, (int)data.Wood);
        }
         
    }
}
