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
    public class SiteCohorts : BiomassCohorts.SiteCohorts, Landis.Library.Cohorts.ISiteCohorts<ISpeciesCohorts>, AgeOnlyCohorts.ISiteCohorts, BiomassCohorts.ISiteCohorts
         
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
        
        //---------------------------------------------------------------------
        void AgeCohort.ISiteCohorts.RemoveMarkedCohorts(AgeCohort.ICohortDisturbance disturbance)
        {
            if (AgeOnlyDisturbanceEvent != null)
                AgeOnlyDisturbanceEvent(this, new Landis.Library.BiomassCohorts.DisturbanceEventArgs(disturbance.CurrentSite,
                                                                       disturbance.Type));
            ReduceOrKillBiomassCohorts(new Landis.Library.BiomassCohorts.WrappedDisturbance(disturbance));
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
        public static new event Landis.Library.BiomassCohorts.DisturbanceEventHandler AgeOnlyDisturbanceEvent;

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

