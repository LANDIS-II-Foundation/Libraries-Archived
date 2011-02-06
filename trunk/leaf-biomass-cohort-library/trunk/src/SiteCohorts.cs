using Landis.Core;
using Landis.SpatialModeling;
using AgeCohort = Landis.Library.AgeOnlyCohorts;
//using Landis.Cohorts;

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using log4net;

namespace Landis.Library.LeafBiomassCohorts
{
    public class SiteCohorts
        : ISiteCohorts, AgeOnlyCohorts.ISiteCohorts
        //: AgeOnlyCohorts.SiteCohorts, ISiteCohorts
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        //---------------------------------------------------------------------

        private List<SpeciesCohorts> cohorts;


        //---------------------------------------------------------------------

        public ISpeciesCohorts this[ISpecies species]
        {
            get {
                return GetCohorts(species);
            }
        }

        //---------------------------------------------------------------------

        private SpeciesCohorts GetCohorts(ISpecies species)
        {
            for (int i = 0; i < cohorts.Count; i++) {
                SpeciesCohorts speciesCohorts = cohorts[i];
                if (speciesCohorts.Species == species)
                    return speciesCohorts;
            }
            return null;
        }

        //---------------------------------------------------------------------

        AgeCohort.ISpeciesCohorts Landis.Library.Cohorts.ISiteCohorts<Landis.Library.AgeOnlyCohorts.ISpeciesCohorts>.this[ISpecies species]
        {
            get
            {
                return GetCohorts(species);
            }
        }
        //---------------------------------------------------------------------

        // Library.BaseCohorts.ISpeciesCohorts Library.BaseCohorts.ISiteCohorts<ISpeciesCohorts>.this[ISpecies species]
        /*ISpeciesCohorts ISiteCohorts.this[ISpecies species]
        {
            get {
                return GetCohorts(species);
            }
        }*/

        //---------------------------------------------------------------------

        public SiteCohorts()
        {
            this.cohorts = new List<SpeciesCohorts>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a copy of a site's cohorts.
        /// </summary>
        /*public SiteCohorts Clone()
        {
            SiteCohorts clone = new SiteCohorts();
            foreach (SpeciesCohorts speciesCohorts in this.cohorts)
                clone.cohorts.Add(speciesCohorts.Clone());
            //clone.totalBiomass = this.totalBiomass;
            //clone.prevYearMortality = this.prevYearMortality;
            return clone;
        }*/

        //---------------------------------------------------------------------
        public void Grow(ushort years, ActiveSite site, int? successionTimestep, ICore mCore)
        {
            Grow(site, successionTimestep.HasValue, true);
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
        public void Grow(ActiveSite site,
                         bool       isSuccessionTimestep,
                         bool annualTimestep)
        {
            if (isSuccessionTimestep && Cohorts.SuccessionTimeStep > 1)
                foreach (SpeciesCohorts speciesCohorts in cohorts)
                    speciesCohorts.CombineYoungCohorts();

            GrowFor1Year(site, annualTimestep);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Grows all the cohorts by advancing their ages by 1 year and
        /// updating their biomasses accordingly.
        /// </summary>
        /// <param name="site">
        /// The site where the cohorts are located.
        /// </param>
        private void GrowFor1Year(ActiveSite site, bool annualTimestep)
        {
            if (isDebugEnabled)
                log.DebugFormat("site {0}: grow cohorts for 1 year",
                                site.Location);

            //  Create a list of iterators, one iterator per set of species
            //  cohorts.  Iterators go through a species' cohorts from oldest
            //  to youngest.  The list is sorted by age, oldest to youngest;
            //  so the first iterator in the list is the iterator's whose
            //  current cohort has the oldest age.
            List<OldToYoungIterator> itors = new List<OldToYoungIterator>();
            foreach (SpeciesCohorts speciesCohorts in cohorts) {
                OldToYoungIterator itor = speciesCohorts.OldToYoung;
                InsertIterator(itor, itors);
            }


            //  Loop through iterators until they're exhausted
            while (itors.Count > 0) {
                //  Grow the current cohort of the first iterator in the list.
                //  The cohort's biomass is updated for 1 year's worth of
                //  growth and mortality.
                OldToYoungIterator itor = itors[0];
                itor.GrowCurrentCohort(site, annualTimestep);

                if (itor.MoveNext()) {
                    //  Iterator has been moved to the next cohort, so see if
                    //  the age of this cohort is the oldest.
                    if (itors.Count > 1 && itor.Age < itors[1].Age) {
                        //  Pop the first iterator of the list, and then re-
                        //  insert it into proper place.
                        itors.RemoveAt(0);
                        InsertIterator(itor, itors);
                    }
                }
                else {
                    //  Iterator has no more cohorts, so remove it from list.
                    itors.RemoveAt(0);

                    if (itor.SpeciesCohorts.Count > 0)
                        itor.SpeciesCohorts.UpdateMaturePresent();
                    else {
                        //  The set of species cohorts is now empty, so remove
                        //  it from the list of species cohorts.
                        for (int i = 0; i < cohorts.Count; i++) {
                            if (cohorts[i] == itor.SpeciesCohorts) {
                                cohorts.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }

        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Inserts a species-cohort iterator in a list of iterators that is
        /// sorted from oldest to youngest.
        /// </summary>
        private void InsertIterator(OldToYoungIterator       itor,
                                    List<OldToYoungIterator> itors)
        {
            bool inserted = false;
            for (int i = 0; i < itors.Count; i++) {
                if (itor.Age > itors[i].Age) {
                    itors.Insert(i, itor);
                    inserted = true;
                    break;
                }
            }
            if (! inserted)
                itors.Add(itor);
        }

        //---------------------------------------------------------------------

        public int ReduceOrKillBiomassCohorts(IDisturbance disturbance)
        {
            int totalReduction = 0;

            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = cohorts.Count - 1; i >= 0; i--) {
                totalReduction += cohorts[i].DamageBySpecies(disturbance);
                if (cohorts[i].Count == 0)
                    cohorts.RemoveAt(i);
            }

            return totalReduction;
        }

        //---------------------------------------------------------------------

        void AgeCohort.ISiteCohorts.RemoveMarkedCohorts(AgeCohort.ICohortDisturbance disturbance)
        {
            if (AgeOnlyDisturbanceEvent != null)
                AgeOnlyDisturbanceEvent(this, new DisturbanceEventArgs(disturbance.CurrentSite,
                                                                       disturbance.Type));
            ReduceOrKillBiomassCohorts(new WrappedDisturbance(disturbance));
        }

        //---------------------------------------------------------------------

        void AgeCohort.ISiteCohorts.RemoveMarkedCohorts(AgeCohort.ISpeciesCohortsDisturbance disturbance)
        {
            if (AgeOnlyDisturbanceEvent != null)
                AgeOnlyDisturbanceEvent(this, new DisturbanceEventArgs(disturbance.CurrentSite,
                                                                       disturbance.Type));

            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            int totalReduction = 0;
            for (int i = cohorts.Count - 1; i >= 0; i--) {
                totalReduction += cohorts[i].MarkCohorts(disturbance);
                if (cohorts[i].Count == 0)
                    cohorts.RemoveAt(i);
            }
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
        public void AddNewCohort(ISpecies species,
                                ushort initial_age,
                                 float initialWoodBiomass,
                                 float initialLeafBiomass)
        {
            //if (isDebugEnabled)
            //    log.DebugFormat("  add cohort: {0}, initial biomass = {1}; site biomass = {2}",
            //                    species.Name, initialBiomass, totalBiomass);

            bool speciesPresent = false;
            for (int i = 0; i < cohorts.Count; i++) {
                SpeciesCohorts speciesCohorts = cohorts[i];
                if (speciesCohorts.Species == species) {
                    speciesCohorts.AddNewCohort(initial_age, initialWoodBiomass, initialLeafBiomass);
                    speciesPresent = true;
                    break;
                }
            }

            if (! speciesPresent)
                cohorts.Add(new SpeciesCohorts(species, initial_age, initialWoodBiomass, initialLeafBiomass));

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
            for (int i = 0; i < cohorts.Count; i++) {
                SpeciesCohorts speciesCohorts = cohorts[i];
                if (speciesCohorts.Species == species) {
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

        //---------------------------------------------------------------------

    }
}
