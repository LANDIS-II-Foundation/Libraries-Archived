//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Arjan de Bruijn

using Landis.Core;
using System.Collections;
using System.Collections.Generic;
using Landis.SpatialModeling;

namespace Landis.Library.BiomassCohortsPnET
{
    public class SiteCohorts : BiomassCohorts.SiteCohorts,   BiomassCohorts.ISiteCohorts, AgeOnlyCohorts.ISiteCohorts
    {
        List<SpeciesCohorts> speciescohorts;

        public virtual void RemoveMarkedCohorts(Landis.Library.AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = speciescohorts.Count - 1; i >= 0; i--)
            {
                speciescohorts[i].RemoveCohorts(disturbance);
                if (speciescohorts[i].Count == 0)speciescohorts.RemoveAt(i);
                    
            }
        }


        public List<SpeciesCohorts> Speciescohorts
        {
            get
            {
                return speciescohorts;
            }
        }
        /// <summary>
        /// Occurs when a site is disturbed by an age-only disturbance.
        /// </summary>
        public static new event Landis.Library.BiomassCohorts.DisturbanceEventHandler AgeOnlyDisturbanceEvent;

        void Landis.Library.AgeOnlyCohorts.ISiteCohorts.RemoveMarkedCohorts(Landis.Library.AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            if (AgeOnlyDisturbanceEvent != null)
                AgeOnlyDisturbanceEvent(this, new Landis.Library.BiomassCohorts.DisturbanceEventArgs(disturbance.CurrentSite,disturbance.Type));
                                                                       

            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            int totalReduction = 0;
            for (int i = speciescohorts.Count - 1; i >= 0; i--)
            {
                totalReduction += speciescohorts[i].MarkCohorts(disturbance);
                if (speciescohorts[i].Count == 0)speciescohorts.RemoveAt(i);
                    
            }
            //totalBiomass -= totalReduction;
        }

        public bool AddNewCohort(Cohort cohort, int SuccessionTimeStep)
        {
            SpeciesCohorts speciescohort = this[cohort.Species];

            if (speciescohort != null)
            {
                foreach (Landis.Library.BiomassCohortsPnET.Cohort mycohort in speciescohort)
                {
                    
                    if (cohort.Age <= SuccessionTimeStep)
                    {
                        mycohort.Wood += cohort.Wood;
                        mycohort.Fol += cohort.Fol;
                        mycohort.Root += cohort.Root;
                        mycohort.FolShed += cohort.FolShed;
                        return false;
                    }
                    
                }
                speciescohort.AddNewCohort(cohort);
                return true;
            }
            speciescohorts.Add(new SpeciesCohorts(cohort));
             

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
            
            this[cohort.Species].RemoveCohort(cohort, site, null);

            /*
            if (this[cohort.Species].Count == 0)
            {
                int index = SpeciesIndex(cohort.Species);
                speciescohorts.RemoveAt(index);
            }
             */
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
            for (int i = 0; i < speciescohorts.Count; i++)
            {
                if (speciescohorts[i].Species == species)
                    return speciescohorts[i];
            }
            return null;
        }
       
        //---------------------------------------------------------------------
        public SiteCohorts()
        {
            this.speciescohorts = new List<SpeciesCohorts>();
        }
        //---------------------------------------------------------------------
        
        int SpeciesIndex(ISpecies species)
        {
            for (int i = 0; i < speciescohorts[i].Count; i++)
            {
                if (speciescohorts[i][i].Species == species)
                {
                    return i;
                }
            }
            return -1;
        }
        
         
        void AssertUniqueSpecies()
        {
            List<string> spc = new List<string>();
            for (int i = 0; i < speciescohorts[i].Count; i++) spc.Add(speciescohorts[i][i].Species.Name);
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
            foreach (SpeciesCohorts speciesCohort in speciescohorts)
                yield return speciesCohort;
        }
        IEnumerator<Landis.Library.AgeOnlyCohorts.ISpeciesCohorts> IEnumerable<Landis.Library.AgeOnlyCohorts.ISpeciesCohorts>.GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohort in speciescohorts)
                yield return speciesCohort;
        }
       


    }
}

