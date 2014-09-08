//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Arjan de Bruijn

using Landis.Core;
using System.Collections;
using System.Collections.Generic;
using Landis.SpatialModeling;
using System.Linq;

namespace Landis.Library.BiomassCohortsPnET
{

    // THERE SHOULD NOT BE INHERITANCE OF BiomassCohorts.SiteCohorts HERE. Cannot get rid of it because it gets stuck on the 'this' pointer
    public class SiteCohorts : BiomassCohorts.SiteCohorts,   BiomassCohorts.ISiteCohorts, AgeOnlyCohorts.ISiteCohorts
    {

        List<Cohort> cohorts = new List<Cohort>();
        public  IEcoregion Ecoregion { get; private set; }
        public ActiveSite Site { get; private set; }

        public SiteCohorts(IEcoregion Ecoregion, ActiveSite Site)
        {
            this.Ecoregion = Ecoregion;
            this.Site = Site;
        }
        
        public List<Cohort> Cohorts
        {
            get
            {
                return cohorts;
            }
            
        }

        public new bool IsMaturePresent(ISpecies species)
        {
            foreach (Cohort cohort in cohorts)
            {
                if (cohort.Species == species && cohort.Age > species.Maturity)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void RemoveMarkedCohorts(Landis.Library.AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            foreach (SpeciesCohorts speciescohort in Speciescohorts)
            { 
                speciescohort.RemoveCohorts(disturbance);
            }
            
        }


        public List<SpeciesCohorts> Speciescohorts
        {
            get
            {
                List<SpeciesCohorts> speciescohorts  = new List<SpeciesCohorts>();

                List<Cohort> RankedCohorts = new List<Cohort>(cohorts.OrderByDescending(o => o.Species.Name));

                ISpecies lastspecies = null;
                foreach (Cohort cohort in RankedCohorts)
                {
                    if (lastspecies == null )
                    {
                     
                        speciescohorts.Add(new SpeciesCohorts(cohort));
                        
                    }
                    else if (cohort.Species.Name != lastspecies.Name)
                    {
                        speciescohorts.Add(new SpeciesCohorts(cohort));
                     
                    }
                    else
                    {
                        speciescohorts[speciescohorts.Count - 1].AddCohort(cohort);
                    }
                    lastspecies = cohort.Species;
                    
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
            foreach (SpeciesCohorts speciescohort in Speciescohorts)
            {
                totalReduction += speciescohort.MarkCohorts(disturbance);
            }   
        }

        public bool AddNewCohort(Cohort cohort, int SuccessionTimeStep)
        {
            foreach (Cohort mycohort in cohorts)
            {
                if (mycohort.Age <= SuccessionTimeStep && cohort.Species == mycohort.Species)
                {
                    cohort.Wood += cohort.Wood;
                    mycohort.Fol += cohort.Fol;
                    mycohort.Root += cohort.Root;
                   
                    return false;
                }
            }
            cohorts.Add(cohort);  
            return true;
        }
        public void RemoveCohort(Cohort cohort, ActiveSite site)
        {
            cohorts.Remove(cohort);
            Cohort.Died(this, cohort, site, null);
            
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
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

