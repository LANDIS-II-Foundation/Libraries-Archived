
using Landis.Core;
using System.Collections;
using System.Collections.Generic;
using Landis.SpatialModeling;
using System;

namespace Landis.Library.AgeOnlyCohorts
{
    public class SiteCohorts
        : ISiteCohorts
    {
        private List<SpeciesCohorts> spp_cohorts;

        //---------------------------------------------------------------------

        public ISpeciesCohorts this[ISpecies species]
        {
            get {
                return GetCohorts(species);
            }
        }

        public IList<ISpecies> ListOfSpeciesPresent
        {
            get
            {
                IList<ISpecies> spp_list = new List<ISpecies>();
                for (int i = 0; i < spp_cohorts.Count; i++)
                {
                    SpeciesCohorts speciesCohorts = spp_cohorts[i];
                    if (speciesCohorts.Count > 0)
                        spp_list.Add(speciesCohorts.Species);
                }
                return spp_list;
            }
        }

        //---------------------------------------------------------------------

        private SpeciesCohorts GetCohorts(ISpecies species)
        {
            for (int i = 0; i < spp_cohorts.Count; i++) {
                SpeciesCohorts speciesCohorts = spp_cohorts[i];
                if (speciesCohorts.Species == species)
                    return speciesCohorts;
            }
            return null;
        }

        //---------------------------------------------------------------------

        public SiteCohorts()
        {
            this.spp_cohorts = new List<SpeciesCohorts>();
        }

        //---------------------------------------------------------------------

        public SiteCohorts(IEnumerable<ISpeciesCohorts> cohorts)
        {
            this.spp_cohorts = new List<SpeciesCohorts>();
            foreach (ISpeciesCohorts speciesCohorts in cohorts)
            {
                this.spp_cohorts.Add(new SpeciesCohorts(speciesCohorts));
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Grows all the cohorts by advancing their ages.
        /// </summary>
        /// <param name="years">
        /// The number of years to advance each cohort's age.
        /// </param>
        /// <param name="site">
        /// The site where the cohorts are located.
        /// </param>
        /// <param name="successionTimestep">
        /// Indicates whether the current timestep is a succession timestep.
        /// If so, then all young cohorts (i.e., those whose ages are less than
        /// or equal to the succession timestep are combined into a single
        /// cohort whose age is the succession timestep.
        /// </param>
        public void Grow(ushort        years,
                         ActiveSite    site,
                         int?          successionTimestep,
                         ICore         mCore)
        {
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = spp_cohorts.Count - 1; i >= 0; i--) {
                spp_cohorts[i].Grow(years, site, successionTimestep, mCore);
                if (spp_cohorts[i].Count == 0)
                    spp_cohorts.RemoveAt(i);
            }
        }

        //---------------------------------------------------------------------

        public virtual void RemoveMarkedCohorts(ICohortDisturbance disturbance)
        {
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.

            for (int i = spp_cohorts.Count - 1; i >= 0; i--) {
                spp_cohorts[i].RemoveMarkedCohorts(disturbance);
                if (spp_cohorts[i].Count == 0)
                    spp_cohorts.RemoveAt(i);
            }
        }

        //---------------------------------------------------------------------

        public virtual void RemoveMarkedCohorts(ISpeciesCohortsDisturbance disturbance)
        {
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = spp_cohorts.Count - 1; i >= 0; i--) {
                spp_cohorts[i].RemoveCohorts(disturbance);
                if (spp_cohorts[i].Count == 0)
                    spp_cohorts.RemoveAt(i);
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a new cohort for a particular species.
        /// </summary>
        public void AddNewCohort(ISpecies species)
        {
            for (int i = 0; i < spp_cohorts.Count; i++) {
                SpeciesCohorts speciesCohorts = spp_cohorts[i];
                if (speciesCohorts.Species == species) {
                    speciesCohorts.AddNewCohort();
                    return;
                }
            }

            //  Species not present at the site.
            spp_cohorts.Add(new SpeciesCohorts(species));
        }

        //---------------------------------------------------------------------

        public bool IsMaturePresent(ISpecies species)
        {
            for (int i = 0; i < spp_cohorts.Count; i++) {
                SpeciesCohorts speciesCohorts = spp_cohorts[i];
                if (speciesCohorts.Species == species) {
                    return speciesCohorts.IsMaturePresent;
                }
            }
            return false;
        }

        //---------------------------------------------------------------------

        public IEnumerator<ISpeciesCohorts> GetEnumerator()
        {
            foreach (ISpeciesCohorts speciesCohorts in spp_cohorts)
                yield return speciesCohorts;
        }

        //---------------------------------------------------------------------

        public string Write()
        {
            string msg = "";
            for (int i = 0; i < spp_cohorts.Count; i++)
            {
                SpeciesCohorts speciesCohorts = spp_cohorts[i];
                if (speciesCohorts.Count > 0)
                    foreach (ICohort cohort in speciesCohorts)
                        msg += String.Format("  {0}/{1};", cohort.Species.Name, cohort.Age);
            }
            return msg;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}
