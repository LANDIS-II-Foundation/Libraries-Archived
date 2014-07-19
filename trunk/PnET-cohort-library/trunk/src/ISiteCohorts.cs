//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using System.Collections.Generic;
namespace Landis.Library.BiomassCohortsPnET
{
    /// <summary>
    /// All the biomass cohorts at a site.
    /// </summary>
    public interface ISiteCohorts
        : Landis.Library.Cohorts.ISiteCohorts<ISpeciesCohorts>

    {

        void AddNewCohort(Cohort cohort, int SuccessionTimeStep);
        void IncrementCohortsAge();
        
     
    }
}
