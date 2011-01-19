//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.Cohorts;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Library.BiomassCohorts
{
    /// <summary>
    /// All the biomass cohorts at a site.
    /// </summary>
    public interface ISiteCohorts
        : Landis.Cohorts.ISiteCohorts<ISpeciesCohorts>
    {
        void AddNewCohort(ISpecies species, int initialBiomass);

        void Grow(ActiveSite site, bool isSuccessionTimestep);


/*
        //---------------------------------------------------------------------

        /// <summary>
        /// Computes who much a disturbance damages the cohorts by reducing
        /// their biomass.
        /// </summary>
        /// <returns>
        /// The total of all the cohorts' biomass reductions.
        /// </returns>
        int RemoveCohorts(IDisturbance disturbance);


        void AddNewCohort(ISpecies species, int initialBiomass);

        void Grow(ActiveSite site, bool isSuccessionTimestep);*/
    }
}
