//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
using System.Collections.Generic;

namespace Landis.Library.BiomassCohorts
{
    /// <summary>
    /// All the biomass cohorts at a site.
    /// </summary>
    public interface ISiteCohorts
        : Landis.Library.Cohorts.ISiteCohorts<BiomassCohorts.ISpeciesCohorts>

    {
        /// <summary>
        /// Computes who much a disturbance damages the cohorts by reducing
        /// their biomass.
        /// </summary>
        /// <returns>
        /// The total of all the cohorts' biomass reductions.
        /// </returns>
        int ReduceOrKillBiomassCohorts(IDisturbance disturbance);
         //---------------------------------------------------------------------
        /// <summary>
        /// Assigns the updated forage to a cohort.
        /// </summary>
        /// <returns>
        /// </returns>
        int UpdateForage(IDisturbance disturbance);
        //---------------------------------------------------------------------
        /// <summary>
        /// Assigns the updated forage in reach to a cohort.
        /// </summary>
        /// <returns>
        /// </returns>
        int UpdateForageInReach(IDisturbance disturbance);
        //---------------------------------------------------------------------
        /// <summary>
        /// Assigns the last browse prop to a cohort.
        /// </summary>
        /// <returns>
        /// </returns>
        double UpdateLastBrowseProp(IDisturbance disturbance);
        //---------------------------------------------------------------------
    }
}
