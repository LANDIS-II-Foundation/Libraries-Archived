//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;

namespace Landis.Library.LeafBiomassCohorts
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public interface ICohort
        : Landis.Library.BiomassCohorts.ICohort //, Landis.Library.AgeOnlyCohorts.ICohort
    {
        /// <summary>
        /// The cohort's biomass (g / m^2).
        /// </summary>
        float WoodBiomass
        {
            get;
        }

        /// <summary>
        /// The cohort's leaf biomass (g / m^2).
        /// </summary>
        float LeafBiomass
        {
            get;
        }


        //int Biomass {get;}
        //---------------------------------------------------------------------


    }
}
