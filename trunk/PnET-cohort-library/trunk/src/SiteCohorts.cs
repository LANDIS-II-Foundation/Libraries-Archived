//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using AgeCohort = Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
 

namespace Landis.Library.BiomassCohortsPnET
{
    public class SiteCohorts
        : ISiteCohorts,
         AgeOnlyCohorts.ISiteCohorts,
         BiomassCohorts.ISiteCohorts
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
        

        public void Grow(ushort years, ActiveSite site, int? successionTimestep, ICore mCore)
        {
        }
        public string  Write()
        {
            throw new System.Exception("Cannot implement write");
        }
       
        public void Grow(ActiveSite site, bool isSuccessionTimestep)
        {
            throw new System.Exception("Incompatibility issue");
        }

        public int ReduceOrKillBiomassCohorts(Landis.Library.BiomassCohorts.IDisturbance disturbance)
        {
            float totalReduction = 0;
            //  Go through list of species co horts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                totalReduction += cohorts[i].MarkCohorts(disturbance);
                if (cohorts[i].Count == 0)
                    cohorts.RemoveAt(i);
            }

            return (int)totalReduction;
        }
         /*
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
         */
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
        public void AddNewCohort(ISpecies species, ushort age, int initialBiomass)
        {
            throw new System.Exception("Incompatibility issue");
        }
        /// <summary>
        /// Add new age-only cohort.  Only used to maintain interface.  .DO NOT USE.
        /// </summary>
        public void AddNewCohort(ISpecies species)
        {
            throw new System.Exception("Incompatibility issue");
        }
        public void AddNewCohort(Cohort cohort, int SuccessionTimeStep)
        {
            int index = SpeciesIndex(cohort.Species);
            if (index >= 0)
            {
                for (int i = 0; i < cohorts[index].Count ; i++)
                {
                    ICohort c = cohorts[index][i];
                    if (c.Age <= SuccessionTimeStep)
                    {
                        c.Wood += cohort.Wood;
                        c.Fol += cohort.Fol;
                        c.Root += cohort.Root;
                        c.FolShed += cohort.FolShed;
                         
                        return;
                    }
                }
                cohorts[index].AddNewCohort2(cohort);//
            }
            else
            {
                cohorts.Add(new SpeciesCohorts(cohort.Species));
                cohorts[cohorts.Count - 1].AddNewCohort2(cohort);
            }
           
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
        public bool IsMaturePresent(ISpecies species)
        {
            int index = SpeciesIndex(species);
            if (index < 0) return false;
            return cohorts[index].IsMaturePresent;
        }


        //---------------------------------------------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<ISpeciesCohorts> GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohorts in cohorts)
                yield return speciesCohorts;
        }
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

