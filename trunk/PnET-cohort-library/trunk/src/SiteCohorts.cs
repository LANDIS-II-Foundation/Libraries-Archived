//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using AgeCohort = Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using log4net;

namespace Landis.Library.BiomassCohortsPnET
{
    public class SiteCohorts
        : ISiteCohorts,
         AgeOnlyCohorts.ISiteCohorts,
         BiomassCohorts.ISiteCohorts
    {
         private List<SpeciesCohorts> cohorts;

        //---------------------------------------------------------------------
         public void Shuffle()
         {
             Random rng = new Random();
             int n = cohorts.Count;
             while (n > 1)
             {
                 int k = rng.Next(n--);
                 SpeciesCohorts temp = cohorts[n];
                 cohorts[n] = cohorts[k];
                 cohorts[k] = temp;
             }
         }

        public void Grow(ushort years, ActiveSite site, int? successionTimestep, ICore mCore)
        {
        }
        public string  Write()
        {
            throw new System.Exception("Cannot implement write");
        }
        public void AddNewCohort(ISpecies species, ushort age, int initialBiomass)
        {
            throw new System.Exception("Incompatibility issue");
        }
        public void Grow(ActiveSite site, bool isSuccessionTimestep)
        {
            throw new System.Exception("Incompatibility issue");
        }
        public int ReduceOrKillBiomassCohorts(IDisturbance disturbance)
        {
            float totalReduction = 0;
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                totalReduction += cohorts[i].MarkCohorts(disturbance);
                if (cohorts[i].Count == 0)
                    cohorts.RemoveAt(i);
            }

            return (int)totalReduction;
        }
         
        public int ReduceOrKillBiomassCohorts(Landis.Library.BiomassCohorts.IDisturbance disturbance)
        {
            float totalReduction = 0;
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                totalReduction += cohorts[i].MarkCohorts(disturbance);
                if (cohorts[i].Count == 0)
                    cohorts.RemoveAt(i);
            }

            return (int)totalReduction;

            
        }
         
        //---------------------------------------------------------------------
        public ISpeciesCohorts this[ISpecies species]
        {
            get
            {
                return GetCohorts(species);
            }
        }
        //---------------------------------------------------------------------
        AgeCohort.ISpeciesCohorts Landis.Library.Cohorts.ISiteCohorts<Landis.Library.AgeOnlyCohorts.ISpeciesCohorts>.this[ISpecies species]
        {
            get
            {
                return GetCohorts(species);
            }
        }
        Landis.Library.BiomassCohorts.ISpeciesCohorts Landis.Library.Cohorts.ISiteCohorts<Landis.Library.BiomassCohorts.ISpeciesCohorts>.this[ISpecies species]
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
        
        public void UpdateMaturePresent()
        {
            foreach (SpeciesCohorts s in cohorts)
            {
                s.UpdateMaturePresent();
            }
        }


        public void CombineCohorts(ActiveSite site, int SuccessionTimeStep, int Year)
        {
            List<Cohort> combined_cohorts = new List<Cohort>();
            foreach (SpeciesCohorts speciesCohorts in cohorts)
            {
                //Cohort combined_cohort;
                speciesCohorts.CombineYoungCohorts(SuccessionTimeStep, Year);
            }
        }
        
        //---------------------------------------------------------------------
        void AgeCohort.ISiteCohorts.RemoveMarkedCohorts(AgeCohort.ICohortDisturbance disturbance)
        {
            if (AgeOnlyDisturbanceEvent != null)
                AgeOnlyDisturbanceEvent(this, new Landis.Library.BiomassCohorts.DisturbanceEventArgs(disturbance.CurrentSite,
                                                                       disturbance.Type));
           ReduceOrKillBiomassCohorts(new WrappedDisturbance(disturbance));
        }

        //---------------------------------------------------------------------
        void AgeCohort.ISiteCohorts.RemoveMarkedCohorts(AgeCohort.ISpeciesCohortsDisturbance disturbance)
        {
            if (AgeOnlyDisturbanceEvent != null)
                AgeOnlyDisturbanceEvent(this, new Landis.Library.BiomassCohorts.DisturbanceEventArgs(disturbance.CurrentSite,
                                                                       disturbance.Type));

            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            float totalReduction = 0;
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                totalReduction += cohorts[i].MarkCohorts(disturbance);
                if (cohorts[i].Count == 0)
                    cohorts.RemoveAt(i);
            }
            //totalBiomass -= totalReduction;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Occurs when a site is disturbed by an age-only disturbance.
        /// </summary>
        public static event Landis.Library.BiomassCohorts.DisturbanceEventHandler AgeOnlyDisturbanceEvent;

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
        public void AddNewCohort(Cohort cohort)
        {
            int index = SpeciesIndex(cohort.Species);
            if (index >= 0)
            {
                cohorts[index].AddNewCohort2(cohort);//
            }
            else
            {
                cohorts.Add(new SpeciesCohorts(cohort.Species));
                cohorts[cohorts.Count - 1].AddNewCohort2(cohort);
            }
            AssertUniqueSpecies();
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

        /// <summary>
        /// Add new age-only cohort.  Only used to maintain interface.  .DO NOT USE.
        /// </summary>
        public void AddNewCohort(ISpecies species)
        {
        }

        //---------------------------------------------------------------------


        public bool IsMaturePresent(ISpecies species)
        {
            for (int i = 0; i < cohorts.Count; i++)
            {
                SpeciesCohorts speciesCohorts = cohorts[i];
                if (speciesCohorts.Species == species)
                {
                    return speciesCohorts.IsMaturePresent;
                }
            }
            return false;
        }


        //---------------------------------------------------------------------

        public IEnumerator<ISpeciesCohorts> GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohorts in cohorts)
                yield return speciesCohorts;
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //---------------------------------------------------------------------

        IEnumerator<AgeCohort.ISpeciesCohorts> IEnumerable<AgeCohort.ISpeciesCohorts>.GetEnumerator()
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

