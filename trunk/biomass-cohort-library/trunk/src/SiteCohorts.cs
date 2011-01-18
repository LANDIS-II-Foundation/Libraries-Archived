//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.Cohorts;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

using log4net;

namespace Landis.Library.BiomassCohorts
{
    public class SiteCohorts
        : AgeOnlyCohorts.SiteCohorts,
        ISiteCohorts
    // Landis.Cohorts.TypeIndependent.ISiteCohorts
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;
        public int InitialBiomass;


        /*public new bool HasAge()
        {
            return true;
        }


        public new bool HasBiomass()
        {
            return true;
        }


        public new bool HasLeafBiomass()
        {
            return false;
        }*/


        //---------------------------------------------------------------------

        private List<SpeciesCohorts> cohorts;
        private int totalBiomass;
        //private int prevYearMortality;

        //---------------------------------------------------------------------

        // public int TotalBiomass //(ISiteCohorts cohorts)
        /*int ISiteCohorts.TotalBiomass //(ISiteCohorts cohorts)
        {
            //int totalBiomass = 0;
            //foreach (SpeciesCohorts speciesCohorts in  cohorts)
            //    foreach(ICohort cohort in speciesCohorts)

            //    totalBiomass += cohort.Biomass;
            //return totalBiomass;

            get
            {
                return totalBiomass;
            }
        }*/

        //---------------------------------------------------------------------

        /*public int TotalBiomass
        {
            get
            {
                return totalBiomass;
            }
        }*/


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

        /*
        AgeOnlyCohorts.ISpeciesCohorts Landis.Cohorts.ISiteCohorts<AgeOnlyCohorts.ISpeciesCohorts>.this[ISpecies species]
        //AgeOnlyCohorts.ISpeciesCohorts ISiteCohorts<AgeOnlyCohorts.ISpeciesCohorts>.this[ISpecies species]
        {
            get
            {
                return GetCohorts(species);
            }
        }*/
        

        //---------------------------------------------------------------------

        public SiteCohorts()
        {
            this.cohorts = new List<SpeciesCohorts>();
            this.totalBiomass = 0;
            //this.prevYearMortality = 0;
            //staticPrevYearMortality = this.prevYearMortality;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a copy of a site's cohorts.
        /// </summary>
        public SiteCohorts Clone()
        {
            SiteCohorts clone = new SiteCohorts();
            foreach (SpeciesCohorts speciesCohorts in this.cohorts)
                clone.cohorts.Add(speciesCohorts.Clone());
            clone.totalBiomass = this.totalBiomass;
            //clone.prevYearMortality = this.prevYearMortality;
            return clone;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Grows all the cohorts for a year, updating their biomasses
        /// accordingly.
        /// </summary>
        /// <param name="site">
        /// The site where the cohorts are located.
        /// </param>
        /// <param name="isSuccessionTimestep">
        /// Indicates whether the current timestep is a succession timestep.
        /// If so, then all young cohorts (i.e., those whose ages are less than
        /// the succession timestep) are combined into a single cohort whose
        /// age is the succession timestep and whose biomass is the sum of all
        /// the biomasses of the young cohorts.
        /// </param>
        public void Grow(ActiveSite site, bool isSuccessionTimestep)
        // void ISiteCohorts.Grow(ActiveSite site, bool isSuccessionTimestep)
        {
            if (isSuccessionTimestep && Cohorts.SuccessionTimeStep > 1)
                foreach (SpeciesCohorts speciesCohorts in cohorts)
                    speciesCohorts.CombineYoungCohorts();

            GrowFor1Year(site);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Grows all the cohorts by advancing their ages by 1 year and
        /// updating their biomasses accordingly.
        /// </summary>
        /// <param name="site">
        /// The site where the cohorts are located.
        /// </param>
        private void GrowFor1Year(ActiveSite site)
        {
            //if (isDebugEnabled)
            //    log.DebugFormat("site {0} ({1} Mg/ha): grow cohorts for 1 year",
            //                    site.Location, TotalBiomass(cohorts));

            //  Create a list of iterators, one iterator per set of species
            //  cohorts.  Iterators go through a species' cohorts from oldest
            //  to youngest.  The list is sorted by age, oldest to youngest;
            //  so the first iterator in the list is the iterator's whose
            //  current cohort has the oldest age.
            List<OldToYoungIterator> itors = new List<OldToYoungIterator>();
            foreach (SpeciesCohorts speciesCohorts in cohorts)
            {
                OldToYoungIterator itor = speciesCohorts.OldToYoung;
                InsertIterator(itor, itors);
            }

            //int siteMortality = 0;

            //  Loop through iterators until they're exhausted
            while (itors.Count > 0)
            {
                //  Grow the current cohort of the first iterator in the list.
                //  The cohort's biomass is updated for 1 year's worth of
                //  growth and mortality.
                OldToYoungIterator itor = itors[0];
                //siteMortality += itor.GrowCurrentCohort(site,
                                                        //ref totalBiomass,
                                                        //prevYearMortality);

                itor.GrowCurrentCohort(site);
                if (itor.MoveNext())
                {
                    //  Iterator has been moved to the next cohort, so see if
                    //  the age of this cohort is the oldest.
                    if (itors.Count > 1 && itor.Age < itors[1].Age)
                    {
                        //  Pop the first iterator of the list, and then re-
                        //  insert it into proper place.
                        itors.RemoveAt(0);
                        InsertIterator(itor, itors);
                    }
                }
                else
                {
                    //  Iterator has no more cohorts, so remove it from list.
                    itors.RemoveAt(0);

                    if (itor.SpeciesCohorts.Count > 0)
                        itor.SpeciesCohorts.UpdateMaturePresent();
                    else
                    {
                        //  The set of species cohorts is now empty, so remove
                        //  it from the list of species cohorts.
                        for (int i = 0; i < cohorts.Count; i++)
                        {
                            if (cohorts[i] == itor.SpeciesCohorts)
                            {
                                cohorts.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }

            //prevYearMortality = siteMortality;
            //staticPrevYearMortality = this.prevYearMortality;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Inserts a species-cohort iterator in a list of iterators that is
        /// sorted from oldest to youngest.
        /// </summary>
        private void InsertIterator(OldToYoungIterator itor,
                                    List<OldToYoungIterator> itors)
        {
            bool inserted = false;
            for (int i = 0; i < itors.Count; i++)
            {
                if (itor.Age > itors[i].Age)
                {
                    itors.Insert(i, itor);
                    inserted = true;
                    break;
                }
            }
            if (!inserted)
                itors.Add(itor);
        }

        //---------------------------------------------------------------------

        public int RemoveCohorts(IDisturbance disturbance)
        {
            int totalReduction = 0;
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = cohorts.Count - 1; i >= 0; i--)
            {
                totalReduction += cohorts[i].MarkCohorts(disturbance);
                if (cohorts[i].Count == 0)
                    cohorts.RemoveAt(i);
            }

            //totalBiomass -= totalReduction;
            return totalReduction;
        }

        //---------------------------------------------------------------------

        // void AgeOnlyCohorts.ISiteCohorts.RemoveMarkedCohorts(AgeOnlyCohorts.ICohortDisturbance disturbance)
        public override void RemoveMarkedCohorts(AgeOnlyCohorts.ICohortDisturbance disturbance)
        {
            // Console.Out.WriteLine("A1");

            if (AgeOnlyDisturbanceEvent != null)
                AgeOnlyDisturbanceEvent(this, new DisturbanceEventArgs(disturbance.CurrentSite,
                                                                       disturbance.Type));
            RemoveCohorts(new WrappedDisturbance(disturbance));
        }

        //---------------------------------------------------------------------

        // void AgeOnlyCohorts.ISiteCohorts.RemoveMarkedCohorts(AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        public override void RemoveMarkedCohorts(AgeOnlyCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            // Console.Out.WriteLine("A2");

            if (AgeOnlyDisturbanceEvent != null)
                AgeOnlyDisturbanceEvent(this, new DisturbanceEventArgs(disturbance.CurrentSite,
                                                                       disturbance.Type));

            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            int totalReduction = 0;
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
        public static event DisturbanceEventHandler AgeOnlyDisturbanceEvent;

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a new cohort for a particular species.
        /// </summary>

        public void AddNewCohort(ISpecies species, int initialBiomass)
        // void ISiteCohorts.AddNewCohort(ISpecies species, int   initialBiomass)
        {
            //if (isDebugEnabled)
            //    log.DebugFormat("  add cohort: {0}, initial biomass = {1}; site biomass = {2}",
            //                    species.Name, initialBiomass, totalBiomass);
            // int initialBiomass = InitialBiomass;

            bool speciesPresent = false;
            for (int i = 0; i < cohorts.Count; i++)
            {
                SpeciesCohorts speciesCohorts = cohorts[i];
                if (speciesCohorts.Species == species)
                {
                    speciesCohorts.AddNewCohort(initialBiomass);
                    speciesPresent = true;
                    break;
                }
            }

            if (!speciesPresent)
                cohorts.Add(new SpeciesCohorts(species, initialBiomass));

            //totalBiomass += initialBiomass;
        }

        //---------------------------------------------------------------------

        /*
        public bool IsMaturePresent(ISpecies species)
        {
            for (int i = 0; i < cohorts.Count; i++) {
                SpeciesCohorts speciesCohorts = cohorts[i];
                if (speciesCohorts.Species == species) {
                    return speciesCohorts.IsMaturePresent;
                }
            }
            return false;
        }
        */

        //---------------------------------------------------------------------

        // public IEnumerator<ISpeciesCohorts> GetEnumerator()
        /**/
        public new IEnumerator<ISpeciesCohorts> GetEnumerator()
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

        
        /*IEnumerator<AgeOnlyCohorts.ISpeciesCohorts> IEnumerable<Landis.Library.AgeOnlyCohorts.ISpeciesCohorts>.GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohorts in cohorts)
                yield return speciesCohorts;
        }*/
        

        //---------------------------------------------------------------------

        /*
        IEnumerator<Landis.Cohorts.TypeIndependent.ISpeciesCohorts> IEnumerable<Landis.Cohorts.TypeIndependent.ISpeciesCohorts>.GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohorts in cohorts)
                yield return speciesCohorts;
        }
        */

        //---------------------------------------------------------------------
        
        /*
        IList<ISpecies> Landis.Cohorts.TypeIndependent.ISiteCohorts.SpeciesPresent
        {
            get {
                List<ISpecies> speciesPresent = new List<ISpecies>(cohorts.Count);
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                    speciesPresent.Add(speciesCohorts.Species);
                return speciesPresent;
            }
        }
        */

        //---------------------------------------------------------------------
        /*
        Landis.Cohorts.TypeIndependent.ISpeciesCohorts Landis.Cohorts.TypeIndependent.ISiteCohorts.this[ISpecies species]
        {
            get {
                return GetCohorts(species);
            }
        }
        */
    }
}

