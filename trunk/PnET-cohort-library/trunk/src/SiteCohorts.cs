//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using System.Collections;
using System.Collections.Generic;


namespace Landis.Library.BiomassCohortsPnET
{
    public class SiteCohorts : BiomassCohorts.SiteCohorts,  BiomassCohorts.ISiteCohorts
         
    {
        List<SpeciesCohorts> cohorts;

        public List<SpeciesCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
        }
       
        //---------------------------------------------------------------------
        public new ISpeciesCohorts this[ISpecies species]
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
        public void IncrementCohortsAge()
        {
            foreach (SpeciesCohorts s in cohorts)
            {
                s.IncrementCohortsAge();
            }
        }
        
         
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
        public new IEnumerator<ISpeciesCohorts> GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohorts in cohorts)
                yield return speciesCohorts;
        }
        
        IEnumerator<Landis.Library.BiomassCohorts.ISpeciesCohorts> IEnumerable<Landis.Library.BiomassCohorts.ISpeciesCohorts>.GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohorts in cohorts)
                yield return speciesCohorts;
        }

       


    }
}

