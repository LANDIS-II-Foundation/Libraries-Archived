//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Arjan de Bruijn

using Landis.Core;
using System.Collections;
using System.Collections.Generic;
using Landis.SpatialModeling;

namespace Landis.Library.BiomassCohortsPnET
{
    public class SiteCohorts : 
          BiomassCohorts.SiteCohorts,
          BiomassCohorts.ISiteCohorts
         
    {
        List<SpeciesCohorts> cohorts;
        
         
        public bool AddNewCohort(Cohort cohort, int SuccessionTimeStep)
        {
            SpeciesCohorts speciescohort = this[cohort.Species];

            if (speciescohort != null)
            {
                foreach (Landis.Library.BiomassCohortsPnET.Cohort mycohort in speciescohort)
                {
                    /*
                    if (cohort.Age <= SuccessionTimeStep)
                    {
                        mycohort.Wood += cohort.Wood;
                        mycohort.Fol += cohort.Fol;
                        mycohort.Root += cohort.Root;
                        mycohort.FolShed += cohort.FolShed;
                        return false;
                    }
                     */
                }
                speciescohort.AddNewCohort(cohort);
                return true;
            }
            cohorts.Add(new SpeciesCohorts(cohort));
             

            return true;
        }
        public bool HasCohort(Cohort cohort)
        {
            if (this[cohort.Species] == null)
            {
                return false;
            }
            foreach (Cohort c in this[cohort.Species])
            {
                if (cohort == c) return true;
            }
            return false;
        }
        public void RemoveCohort(Cohort cohort, ActiveSite site)
        {
            if (this[cohort.Species] == null)
            {
                return;
                SpeciesCohorts s = this[cohort.Species];
            }
            this[cohort.Species].RemoveCohort(cohort, site, null);
            if (this[cohort.Species].Count == 0)
            {
                cohorts.RemoveAt(SpeciesIndex(cohort.Species));
            }
        }
       
     
        //---------------------------------------------------------------------
        public new SpeciesCohorts this[ISpecies species]
        {
            get
            {
                return GetCohorts(species);
            }
        }
         
        //---------------------------------------------------------------------
        private SpeciesCohorts GetCohorts(ISpecies species)
        {
            for (int i = 0; i < cohorts.Count; i++)
            {
                SpeciesCohorts speciesCohorts = cohorts[i];
                if (speciesCohorts.Species == species)
                    return speciesCohorts;
            }
            return null;
        }
       
        //---------------------------------------------------------------------
        public SiteCohorts()
        {
            this.cohorts = new List<SpeciesCohorts>();
        }
        //---------------------------------------------------------------------
        
        int SpeciesIndex(ISpecies species)
        {
            for (int i = 0; i < cohorts.Count; i++)
            {
                if (cohorts[i].Species == species)
                {
                    return i;
                }
            }
            return -1;
        }
        
         
        void AssertUniqueSpecies()
        {
            List<string> spc = new List<string>();
            for (int i = 0; i < cohorts.Count; i++)spc.Add(cohorts[i].Species.Name);
            spc.Sort();
            for (int s = 0; s < spc.Count - 1; s++)
            {
                if (spc[s] == spc[s + 1])
                {
                    throw new System.Exception("Error in species cohorts, duplicate species");
                }
            }
        }
        

        //---------------------------------------------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /*
        public new IEnumerator<ISpeciesCohorts> GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohorts in cohorts)
                yield return speciesCohorts;
        }*/
        
        IEnumerator<Landis.Library.BiomassCohorts.ISpeciesCohorts> IEnumerable<Landis.Library.BiomassCohorts.ISpeciesCohorts>.GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohorts in cohorts)
                yield return speciesCohorts;
        }
         
       


    }
}

