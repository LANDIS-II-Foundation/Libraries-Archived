//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Arjan de Bruijn

using Landis.Core;
using System.Collections;
using System.Collections.Generic;
using Landis.SpatialModeling;
using System.Linq;

namespace Landis.Library.BiomassCohortsPnET
{
    public class SiteCohorts :  BiomassCohorts.ISiteCohorts, AgeOnlyCohorts.ISiteCohorts 
    {
        List<Cohort> cohorts = new List<Cohort>();

        public  IEcoregion Ecoregion { get; private set; }
        public ActiveSite Site { get; private set; }
        public float WaterMin;

        public int ReduceOrKillBiomassCohorts(Landis.Library.BiomassCohorts.IDisturbance disturbance)
        {
            int totalReduction = 0;
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                int reduction = disturbance.ReduceOrKillMarkedCohort(cohorts[i]);
                if (reduction > 0)
                {
                    totalReduction += reduction;
                    if (reduction < cohorts[i].Biomass)
                    {
                        cohorts[i].Wood -= reduction;
                    }
                    else
                    {
                        //cohorts[i].IsAlive = false;
                        Cohort.Died(this, cohorts[i], disturbance.CurrentSite, disturbance.Type);
                        cohorts.Remove(cohorts[i]);
                    }
                }

            }
            return totalReduction;
        }
         

        
        public Landis.Library.BiomassCohorts.ISpeciesCohorts this[ISpecies species]
        {
            get
            {
                foreach(SpeciesCohorts s in Speciescohorts)
                {
                    if (s.Species.Name == species.Name)
                    {
                        return s;
                    }
                }
                return null;
//                throw new System.Exception("Cannot retrieve speciescohort " + species.Name);
            }
        }
        Landis.Library.AgeOnlyCohorts.ISpeciesCohorts Landis.Library.Cohorts.ISiteCohorts<Landis.Library.AgeOnlyCohorts.ISpeciesCohorts>.this[ISpecies species]
        {
            get
            {
                foreach (SpeciesCohorts s in Speciescohorts)
                {
                    if (s.Species.Name == species.Name) return s;
                }
                return null;
                
            }
        }


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

        public bool IsMaturePresent(ISpecies species)
        {
            foreach (Cohort cohort in cohorts)
            {
                if (cohort.Species.Name == species.Name)
                {
                    if (cohort.Age > species.Maturity)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        
        private List<SpeciesCohorts> speciescohorts =null;

        public List<SpeciesCohorts> Speciescohorts
        {
            set 
            {
                speciescohorts = value;
            }
            get
            {
                if (speciescohorts != null) return speciescohorts;


                speciescohorts  = new List<SpeciesCohorts>();

                List<Cohort> RankedCohorts = new List<Cohort>(cohorts.OrderByDescending(o => o.Species.Name));

                ISpecies lastspecies = null;
                foreach (Cohort cohort in RankedCohorts)
                {
                    if (lastspecies == null || cohort.Species.Name != lastspecies.Name)
                    {
                        speciescohorts.Add(new SpeciesCohorts(cohort.Species));
                    }
                    speciescohorts[speciescohorts.Count - 1].AddCohort(cohort);


                    lastspecies = cohort.Species;
                    
                }
                return speciescohorts;
            }
        }
        /// <summary>
        /// Occurs when a site is disturbed by an age-only disturbance.
        /// </summary>
        public static event Landis.Library.BiomassCohorts.DisturbanceEventHandler AgeOnlyDisturbanceEvent;

        
        void Landis.Library.AgeOnlyCohorts.ISiteCohorts.RemoveMarkedCohorts(Landis.Library.AgeOnlyCohorts.ICohortDisturbance disturbance)
        {
             
            if (AgeOnlyDisturbanceEvent != null)
            {
                AgeOnlyDisturbanceEvent(this, new Landis.Library.BiomassCohorts.DisturbanceEventArgs(disturbance.CurrentSite,
                                                                       disturbance.Type));
            }
            ReduceOrKillBiomassCohorts(new Landis.Library.BiomassCohorts.WrappedDisturbance(disturbance));
             
        }
        public void ResetSpeciesCohorts()
        {
            // This is a time saver; setting the speciescohorts to null 
            // when I want to add/ remove a cohort.
            // when other modules ask for speciescohorts, they will be signalled 
            // that cohorts have changed so they will gather the cohorts in speices cohorts anew
            Speciescohorts = null;
        }
        public bool AddNewCohort(Cohort cohort, int SuccessionTimeStep)
        {

            ResetSpeciesCohorts();

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

        public void RemoveCohort(Cohort cohort, Landis.SpatialModeling.ActiveSite site)
        {
            ResetSpeciesCohorts();
            Speciescohorts = null;

            
            cohorts.Remove(cohort);
            Cohort.Died(this, cohort, site, null);

        }
        void Landis.Library.AgeOnlyCohorts.ISiteCohorts.RemoveMarkedCohorts(Landis.Library.AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            
            if (AgeOnlyDisturbanceEvent != null)
                AgeOnlyDisturbanceEvent(this, new Landis.Library.BiomassCohorts.DisturbanceEventArgs(disturbance.CurrentSite,disturbance.Type));
                                                                       

            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            int totalReduction = 0;
            foreach (SpeciesCohorts speciescohort in Speciescohorts)
            {
                totalReduction += MarkCohorts(speciescohort, disturbance);
            }   
             
        }
         
        public int MarkCohorts(SpeciesCohorts speciescohort, AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            Landis.Library.AgeOnlyCohorts.SpeciesCohortBoolArray isSpeciesCohortDamaged = new AgeOnlyCohorts.SpeciesCohortBoolArray();

            isSpeciesCohortDamaged.SetAllFalse(speciescohort.Count);
            disturbance.MarkCohortsForDeath(speciescohort, isSpeciesCohortDamaged);

            //  Go backwards through list of cohort data, so the removal of an
            //  item doesn't mess up the loop.
            int totalReduction = 0;
            for (int i = speciescohort.Count  - 1; i >= 0; i--)
            {
                if (isSpeciesCohortDamaged[i])
                {
                    totalReduction += speciescohort[i].Biomass;

                    Landis.Library.BiomassCohorts.Cohort.KilledByAgeOnlyDisturbance(speciescohort, speciescohort[i], disturbance.CurrentSite, disturbance.Type);

                    Cohort.Died(speciescohort, speciescohort[i], disturbance.CurrentSite, disturbance.Type);

                    cohorts.Remove(speciescohort[i]);
                }
            }
            return totalReduction;
        }
         
        public IEnumerator<Landis.Library.BiomassCohorts.ISpeciesCohorts> GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohorts in Speciescohorts)
            {
                yield return speciesCohorts;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        IEnumerator<Landis.Library.BiomassCohorts.ISpeciesCohorts> IEnumerable<Landis.Library.BiomassCohorts.ISpeciesCohorts>.GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohort in Speciescohorts)
            {
                yield return speciesCohort;
            }
        }
        IEnumerator<Landis.Library.AgeOnlyCohorts.ISpeciesCohorts> IEnumerable<Landis.Library.AgeOnlyCohorts.ISpeciesCohorts>.GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohort in Speciescohorts)
            {
                yield return speciesCohort;
            }
        }
       


    }
}

