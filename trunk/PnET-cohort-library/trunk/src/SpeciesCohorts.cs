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
using log4net;

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
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;
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
                return new Cohort(species, cohorts[index]);
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

        /// <summary>
        /// Combines all young cohorts into a single cohort whose age is the
        /// succession timestep - 1 and whose biomass is the sum of all the
        /// biomasses of the young cohorts.
        /// </summary>
        /// <remarks>
        /// The age of the combined cohort is set to the succession timestep -
        /// 1 so that when the combined cohort undergoes annual growth, its
        /// age will end up at the succession timestep.
        /// <p>
        /// For this method, young cohorts are those whose age is less than or
        /// equal to the succession timestep.  We include the cohort whose age
        /// is equal to the timestep because such a cohort is generated when
        /// reproduction occurs during a succession timestep.
        /// </remarks>
        public bool CombineYoungCohorts(int SuccessionTimeStep, int Year)
        {
           
            //  Work from the end of cohort data since the array is in old-to-
            //  young order.
            int youngCount = 0;
            float totalWoodC = 0;
            float totalFolC = 0;
            float totalRootC = 0;
            float totalNSC = 0;
            float totalfolshead = 0;

            bool leaf_on = false;
            for (int i = cohorts.Count - 1; i >= 0; i--) {
                Cohort c = cohorts[i];
                if (c.Age <= SuccessionTimeStep)
                {
                    youngCount++;
                    totalWoodC += c.Wood;
                    totalFolC += c.Fol;
                    totalNSC += c.NSC;
                    totalRootC += c.Root;
                     leaf_on=c.Leaf_On;
                     totalfolshead += c.FolShed;
                 }
                else
                    break;
            }
            
            if (youngCount > 1) {
                cohorts.RemoveRange(cohorts.Count - youngCount, youngCount);

                cohorts.Add(new Cohort(this.species, (ushort)(BiomassCohorts.Cohorts.SuccessionTimeStep - 1), totalFolC, totalfolshead,totalWoodC, totalNSC, totalRootC,  Year, leaf_on));
                
                return true;
            }
            return false;
        }

        //---------------------------------------------------------------------
        public void IncrementCohortsAge()
        {
            foreach (Cohort d in cohorts)
            {
                d.IncrementAge();
            }
            
        }
        //---------------------------------------------------------------------
        public void RemoveCohort(int        index,
                                  ICohort    cohort,
                                  ActiveSite site,
                                  ExtensionType disturbanceType)
        {
            cohorts.RemoveAt(index);
            Cohort.Died(this, cohort, site, disturbanceType);
        }
        //----------------------------------------------------------------------
        // MarkCohorts
        //---------------------------------------------------------------------
        public ICohort Get(int index)
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
                Cohort cohort = new Cohort(species, cohorts[i]);
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
                        RemoveCohort(i, cohort, disturbance.CurrentSite,
                                     disturbance.Type);
                        cohort = null;
                    }
                }
                if (cohort != null && cohort.Age >= species.Maturity)
                    isMaturePresent = true;
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
                    Cohort cohort = new Cohort(species, cohorts[i]);
                    totalReduction += cohort.Wood;

                    RemoveCohort(i, cohort, disturbance.CurrentSite,
                                 disturbance.Type);
                    Cohort.KilledByAgeOnlyDisturbance(this, cohort, disturbance.CurrentSite, disturbance.Type);

                    cohort = null;
                }
                else if (cohorts[i].Age >= species.Maturity)
                    isMaturePresent = true;
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
                yield return new Cohort(species, data);
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
