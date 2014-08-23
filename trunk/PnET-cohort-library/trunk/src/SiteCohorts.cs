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
        
         

        List<Cohort> cohorts;

        
        public List<Cohort> Cohorts
        {
            get
            {
                return cohorts;
            }
            
        }
       
        public virtual void RemoveMarkedCohorts(Landis.Library.AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = Speciescohorts.Count - 1; i >= 0; i--)
            {
                Speciescohorts[i].RemoveCohorts(disturbance);
                if (Speciescohorts[i].Count == 0) Speciescohorts.RemoveAt(i);
                    
            }
        }


        public List<SpeciesCohorts> Speciescohorts
        {
            get
            {
                List<SpeciesCohorts> speciescohorts = new List<SpeciesCohorts>();
                foreach (Cohort cohort in cohorts)
                {
                    bool added = false;
                    foreach (SpeciesCohorts speciescohort in speciescohorts)
                    {
                        if (cohort.Species == speciescohort.Species)
                        {
                            speciescohort.AddCohort(cohort);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        speciescohorts.Add(new SpeciesCohorts(cohort));
                    }
                }
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
            for (int i = Speciescohorts.Count - 1; i >= 0; i--)
            {
                totalReduction += Speciescohorts[i].MarkCohorts(disturbance);
                if (Speciescohorts[i].Count == 0) Speciescohorts.RemoveAt(i);
                    
            }
            //totalBiomass -= totalReduction;
        }

        public bool AddNewCohort(Cohort cohort, int SuccessionTimeStep)
        {
            foreach (Cohort mycohort in Cohorts)
            {
                if (mycohort.Age <= SuccessionTimeStep && cohort.Species == mycohort.Species)
                {
                    cohort.Wood += cohort.Wood;
                    mycohort.Fol += cohort.Fol;
                    mycohort.Root += cohort.Root;
                    mycohort.FolShed += cohort.FolShed;
                    return false;
                }
            }    
             
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
            cohorts.Remove(cohort);
            Cohort.Died(this, cohort, site, null);
            
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
            SpeciesCohorts speciecohort = null;
            foreach (Cohort cohort in cohorts)
            {
                if (cohort.Species == species)
                {
                    if (speciecohort == null)
                    {
                        speciecohort = new SpeciesCohorts(cohort);
                    }
                    else speciecohort.AddCohort(cohort);
                }
            }
            return speciecohort;
           
        }
         
        //---------------------------------------------------------------------
        public SiteCohorts()
        {
          
            cohorts = new List<Cohort>();
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
            foreach (SpeciesCohorts speciesCohort in Speciescohorts)
                yield return speciesCohort;
        }
        IEnumerator<Landis.Library.AgeOnlyCohorts.ISpeciesCohorts> IEnumerable<Landis.Library.AgeOnlyCohorts.ISpeciesCohorts>.GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohort in Speciescohorts)
                yield return speciesCohort;
        }
       


    }
}

