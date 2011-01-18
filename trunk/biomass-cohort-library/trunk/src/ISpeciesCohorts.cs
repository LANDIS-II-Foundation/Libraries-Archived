//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
// using Landis.Library.BaseCohorts;
using System.Collections.Generic;
//using Wisc.Flel.GeospatialModeling.Landscapes;

namespace Landis.Library.BiomassCohorts
{
        /// The biomass cohorts for a particular species at a site.
        /// </summary>
        public interface ISpeciesCohorts
            : Landis.Cohorts.ISpeciesCohorts<ICohort>
            // : Landis.Cohorts.ISpeciesCohorts<Landis.Library.BiomassCohorts.ICohort>
        {
        }
    
}
