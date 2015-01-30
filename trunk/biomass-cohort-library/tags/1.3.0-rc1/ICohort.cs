//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;

namespace Landis.Library.BiomassCohorts
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public interface ICohort
        : Landis.Library.AgeOnlyCohorts.ICohort
    {
        /// <summary>
        /// The cohort's biomass (g m-2).
        /// </summary>
        int Biomass
        {
            get;
        }


        //---------------------------------------------------------------------

        /// <summary>
        /// Computes how much of the cohort's biomass is non-woody.
        /// </summary>
        /// <param name="site">
        /// The site where the cohort is located.
        /// </param>
        int ComputeNonWoodyBiomass(ActiveSite site);
        
    }
}
