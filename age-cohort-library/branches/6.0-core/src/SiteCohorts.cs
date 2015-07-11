
using Landis.Core;
using System.Collections;
using System.Collections.Generic;
//using Wisc.Flel.GeospatialModeling.Landscapes;
using Landis.SpatialModeling;
using Landis.SpatialModeling.CoreServices;

namespace Landis.Library.AgeOnlyCohorts
{
    public class SiteCohorts
        : ISiteCohorts
        //<ISpeciesCohorts<ICohort>>//, TypeIndependent.ISiteCohorts
    {
        private List<SpeciesCohorts> cohorts;

        public bool HasAge()
        {
            return true;
        }

        public bool HasBiomass()
        {
            return false;
        }

        public bool HasLeafBiomass()
        {
            return false;
        }

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

        public SiteCohorts()
        {
            this.cohorts = new List<SpeciesCohorts>();
        }

        //---------------------------------------------------------------------

        public SiteCohorts(IEnumerable<ISpeciesCohorts> cohorts)
        //public SiteCohorts(ISpeciesCohorts cohorts)
        {
            this.cohorts = new List<SpeciesCohorts>();
            foreach (ISpeciesCohorts speciesCohorts in cohorts)
            {
                this.cohorts.Add(new SpeciesCohorts(speciesCohorts));
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
            for (int i = cohorts.Count - 1; i >= 0; i--) {
                cohorts[i].Grow(years, site, successionTimestep, mCore);
                if (cohorts[i].Count == 0)
                    cohorts.RemoveAt(i);
            }
        }

        //---------------------------------------------------------------------

        public void MarkCohortsForDeath(ICohortDisturbance disturbance)
        {
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = cohorts.Count - 1; i >= 0; i--) {
                cohorts[i].MarkCohortsForDeath(disturbance);
                if (cohorts[i].Count == 0)
                    cohorts.RemoveAt(i);
            }
        }

        //---------------------------------------------------------------------

        public void MarkCohortsForDeath(ISpeciesCohortsDisturbance disturbance)
        {
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            for (int i = cohorts.Count - 1; i >= 0; i--) {
                cohorts[i].MarkCohorts(disturbance);
                if (cohorts[i].Count == 0)
                    cohorts.RemoveAt(i);
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a new cohort for a particular species.
        /// </summary>
        public void AddNewCohort(ISpecies species)
        {
            for (int i = 0; i < cohorts.Count; i++) {
                SpeciesCohorts speciesCohorts = cohorts[i];
                if (speciesCohorts.Species == species) {
                    speciesCohorts.AddNewCohort();
                    return;
                }
            }

            //  Species not present at the site.
            cohorts.Add(new SpeciesCohorts(species));
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
            foreach (ISpeciesCohorts speciesCohorts in cohorts)
                yield return speciesCohorts;
        }
 
        //---------------------------------------------------------------------

        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        

        //---------------------------------------------------------------------

        /*
        IEnumerator<TypeIndependent.ISpeciesCohorts> IEnumerable<TypeIndependent.ISpeciesCohorts>.GetEnumerator()
        {
            foreach (SpeciesCohorts speciesCohorts in cohorts)
                yield return speciesCohorts;
        }
        */

        //---------------------------------------------------------------------

        /*
        IList<ISpecies> TypeIndependent.ISiteCohorts.SpeciesPresent
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
        TypeIndependent.ISpeciesCohorts TypeIndependent.ISiteCohorts.this[ISpecies species]
        {
            get {
                return GetCohorts(species);
            }
        }
        */
    }
}
