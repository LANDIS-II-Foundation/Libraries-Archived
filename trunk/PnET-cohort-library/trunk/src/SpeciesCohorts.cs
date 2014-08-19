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
    public class SpeciesCohorts : BiomassCohorts.ISpeciesCohorts, IEnumerable<Cohort>
       
    {

        private ISpecies species;
        private List<Cohort> cohorts;

        public int Count
        {
            get
            {
                return cohorts.Count;
            }
        }
        public ISpecies Species
        {
            get
            {
                return species;
            }
        }
        public bool IsMaturePresent 
        {
            get
            {
                for (int i = 0; i < cohorts.Count; i++)
                {
                    Cohort Cohorts = cohorts[i];
                    if (Cohorts.Age > species.Maturity)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        
        public Cohort this[int index]
        {
            get {
                 
                return cohorts[index];
            }
             
        }
        
        public SpeciesCohorts(Cohort c) //: base(c.Species,c.Age,c.Biomass)
        {
            this.species = c.Species;
            this.cohorts = new List<Cohort>();
            AddNewCohort(c);
        }
        public void AddNewCohort(Cohort c)
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

        
        IEnumerator<Landis.Library.BiomassCohortsPnET.Cohort> IEnumerable<Landis.Library.BiomassCohortsPnET.Cohort>.GetEnumerator()
        {
            foreach (Cohort data in cohorts)
                yield return data;
        }
        
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Cohort>)this).GetEnumerator();
        }
        /*
        IEnumerator IEnumerable.GetEnumerator()
        {
            return cohorts.GetEnumerator();
        }
        */
        IEnumerator<Landis.Library.BiomassCohorts.ICohort> IEnumerable<Landis.Library.BiomassCohorts.ICohort>.GetEnumerator()
        {
            foreach (Cohort data in cohorts)
                yield return new Landis.Library.BiomassCohorts.Cohort(data.Species, data.Age, (int)data.Wood);
        }
         
    }
}
