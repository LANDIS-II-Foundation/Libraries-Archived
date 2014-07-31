//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using System.Collections;
using System.Collections.Generic;


namespace Landis.Library.BiomassCohortsPnET
{
    public class SiteCohorts : 
          BiomassCohorts.SiteCohorts,
          BiomassCohorts.ISiteCohorts
         
    {
        List<SpeciesCohorts> cohorts;

        public void AddNewCohort(Cohort cohort, int SuccessionTimeStep)
        {
            int index = SpeciesIndex(cohort.Species);
            if (index >= 0)
            {
                for (int i = 0; i < cohorts.Count; i++)
                {
                    for (int cc =0; cc< cohorts[i].Count ; cc++)
                    {
                        ICohort c = cohorts[i][cc];
                        if (c.Species == cohort.Species && c.Age <= SuccessionTimeStep)
                        {
                            c.Wood += cohort.Wood;
                            c.Fol += cohort.Fol;
                            c.Root += cohort.Root;
                            c.FolShed += cohort.FolShed;

                            return;
                        }
                    }
                }

                cohorts[index].AddNewCohort(cohort);//
            }
            else
            {
                cohorts.Add(new SpeciesCohorts(cohort));
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

