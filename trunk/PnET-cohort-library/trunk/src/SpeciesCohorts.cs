//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Landis.Library.BiomassCohorts;


namespace Landis.Library.BiomassCohortsPnET
{
    /// <summary>
    /// The cohorts for a particular species at a site.
    /// </summary>
    public class SpeciesCohorts
        : ISpeciesCohorts,
          AgeOnlyCohorts.ISpeciesCohorts,
          BiomassCohorts.ISpeciesCohorts
    {
        private ISpecies species;
        private bool isMaturePresent;
        
        private static AgeOnlyCohorts.SpeciesCohortBoolArray isSpeciesCohortDamaged;
        static SpeciesCohorts()
        {
            isSpeciesCohortDamaged = new AgeOnlyCohorts.SpeciesCohortBoolArray();
        }

        //  Cohort data is in oldest to youngest order.
        private List<Cohort> cohorts;

        //---------------------------------------------------------------------

        public int Count
        {
            get {
                return cohorts.Count;
            }
        }

        //---------------------------------------------------------------------

        public ISpecies Species
        {
            get {
                return species;
            }
        }

        //---------------------------------------------------------------------

        public bool IsMaturePresent
        {
            get {
                return isMaturePresent;
            }
        }

        //---------------------------------------------------------------------

        public ICohort this[int index]
        {
            get {
                return new Cohort(cohorts[index]);
            }
             
        }

       //---------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance with one young cohort (age = 1).
        /// </summary>
        public SpeciesCohorts(ISpecies species)
        {
            this.species = species;
            this.cohorts = new List<Cohort>();
            this.isMaturePresent = false;
        }
       
        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a copy of a species' cohorts.
        /// </summary>
        public SpeciesCohorts Clone()
        {
            SpeciesCohorts clone = new SpeciesCohorts(this.species);
            clone.cohorts = new List<Cohort>(this.cohorts);
            clone.isMaturePresent = this.isMaturePresent;
            return clone;
        }

        //---------------------------------------------------------------------
        public void AddNewCohort2(Cohort c)
        {
            //System.Console.WriteLine("AddNewCohort2\t" + c.Species.Name);
            this.cohorts.Add(c);
        }
        
        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the age of a cohort at a specific index.
        /// </summary>
        /// <exception cref="System.IndexOutOfRangeException">
        /// </exception>
        public int GetAge(int index)
        {
            return cohorts[index].Age;
        }

         
        //---------------------------------------------------------------------
        public void IncrementCohortsAge()
        {
            foreach (Cohort d in cohorts)
            {
                d.IncrementAge();
            }
            
        }
        private void CohortsToConsole(string msg)
        {
            System.Console.WriteLine(msg);
            foreach (Cohort c in cohorts)
            {
                System.Console.WriteLine(c.Species.Name);
            }
        }
        //---------------------------------------------------------------------
        public void RemoveCohort( Cohort    cohort,
                                  ActiveSite site,
                                  ExtensionType disturbanceType)
        {
            
            cohorts.Remove(cohort);
            Cohort.Died(this, cohort, site, disturbanceType);
        }
         
        //----------------------------------------------------------------------
        // MarkCohorts
        //---------------------------------------------------------------------
        public Cohort Get(int index)
        {
            try
            {
                return cohorts[index];
            }
            catch
            {
                return null;
            }
        }
        public float MarkCohorts(IDisturbance disturbance)
        {
            //  Go backwards through list of cohort data, so the removal of an
            //  item doesn't mess up the loop.
            isMaturePresent = false;
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                Cohort cohort = new Cohort(cohorts[i]);
                disturbance.ReduceOrKillMarkedCohort(cohort);
                double defoliation = disturbance.CumulativeDefoliation();
                cohort.Fol *= 1 - (float)defoliation;

            }
            return 0;
        }
        /// <summary>
        /// Updates the IsMaturePresent property.
        /// </summary>
        /// <remarks>
        /// Should be called after all the species' cohorts have grown.
        /// </remarks>
        public void UpdateMaturePresent()
        {
            isMaturePresent = false;
            for (int i = 0; i < cohorts.Count; i++)
            {
                if (cohorts[i].Age >= species.Maturity)
                {
                    
                    isMaturePresent = true;
                    break;
                }
            }
        }
        public int MarkCohorts(Landis.Library.BiomassCohorts.IDisturbance disturbance)
        {
            //  Go backwards through list of cohort data, so the removal of an
            //  item doesn't mess up the loop.
            isMaturePresent = false;
            int totalReduction = 0;
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                Cohort cohort = cohorts[i];
                int reduction = disturbance.ReduceOrKillMarkedCohort(cohort);
                if (reduction > 0)
                {
                    totalReduction += reduction;
                    if (reduction < cohort.Biomass)
                    {
                        float fRed = reduction / cohort.Biomass;
                        cohort.Wood *= fRed;
                        cohort.Fol *= fRed;
                    }
                    else
                    {
                        RemoveCohort(cohort, disturbance.CurrentSite, disturbance.Type);
                        cohort = null;
                    }
                }
                if (cohort != null && cohort.Age >= species.Maturity)
                {
                   
                    isMaturePresent = true;
                }
            }
            return totalReduction;
        }
        /// <summary>
        /// Removes the cohorts that are completed removed by disturbance.
        /// </summary>
        /// <returns>
        /// The total biomass of all the cohorts damaged by the disturbance.
        /// </returns>
        public float MarkCohorts(AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            isSpeciesCohortDamaged.SetAllFalse(Count);
            disturbance.MarkCohortsForDeath(this, isSpeciesCohortDamaged);

            //  Go backwards through list of cohort data, so the removal of an
            //  item doesn't mess up the loop.
            isMaturePresent = false;
            float totalReduction = 0;
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                if (isSpeciesCohortDamaged[i])
                {
                    //Cohort cohort = new Cohort(cohorts[i]);
                    totalReduction += cohorts[i].Wood;

                    Cohort.KilledByAgeOnlyDisturbance(this, cohorts[i], disturbance.CurrentSite, disturbance.Type);
                    RemoveCohort(cohorts[i], disturbance.CurrentSite, disturbance.Type);
                    

                    //cohort = null;
                }
                else if (cohorts[i].Age >= species.Maturity)
                {
                   
                    isMaturePresent = true;
                }
            }
            return totalReduction;
        }
        public float MarkCohorts(Landis.Library.BiomassCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            throw new System.Exception("Cannot implement MarkCohorts");
        }
        //----------------------------------------------------------------------
        // MarkCohorts
        //---------------------------------------------------------------------

        //---------------------------------------------------------------------
        //  GetEnumerator
        //---------------------------------------------------------------------
        IEnumerator<ICohort> IEnumerable<ICohort>.GetEnumerator()
        {
            foreach (Cohort data in cohorts)
                yield return data;
            //yield return new Cohort(data);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            //Console.Out.WriteLine("Itor 2");
            return ((IEnumerable<ICohort>)this).GetEnumerator();
        }
        IEnumerator<Landis.Library.AgeOnlyCohorts.ICohort> IEnumerable<Landis.Library.AgeOnlyCohorts.ICohort>.GetEnumerator()
        {
            //Console.Out.WriteLine("Itor 3");
            foreach (Cohort data in cohorts)
                yield return new Landis.Library.AgeOnlyCohorts.Cohort(species, data.Age);
        }
        IEnumerator<Landis.Library.BiomassCohorts.ICohort> IEnumerable<Landis.Library.BiomassCohorts.ICohort>.GetEnumerator()
        {
            foreach (Cohort data in cohorts)
                yield return new Landis.Library.BiomassCohorts.Cohort(species, data.Age, (int)data.Wood);
        }
        //---------------------------------------------------------------------
        //  GetEnumerator
        //---------------------------------------------------------------------
    }
}
