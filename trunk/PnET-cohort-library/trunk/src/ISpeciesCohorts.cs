//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using System.Collections.Generic;
using Landis.SpatialModeling;

namespace Landis.Library.BiomassCohortsPnET
{
    
        /// The biomass cohorts for a particular species at a site.
        /// </summary>
        public interface ISpeciesCohorts
             : Landis.Library.Cohorts.ISpeciesCohorts<Landis.Library.BiomassCohortsPnET.ICohort>
        {

            void RemoveCohort(Cohort cohort,
                              ActiveSite site,
                              ExtensionType disturbanceType);



             
        }
    
}
