//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Library.BiomassCohorts
{
    /// <summary>
    /// All the biomass cohorts at a site.
    /// </summary>
    public interface ISiteCohorts
        : Landis.Library.AgeOnlyCohorts.ISiteCohorts//<ISpeciesCohorts>
    {
        int RemoveCohorts(IDisturbance disturbance);
        void AddNewCohort(ISpecies species, ushort age, int initialBiomass);
        string Write();
        void Grow(ActiveSite site, bool isSuccessionTimestep);
    }
}
