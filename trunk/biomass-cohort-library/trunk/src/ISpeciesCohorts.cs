//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using System.Collections.Generic;

namespace Landis.Library.BiomassCohorts
{
        /// The biomass cohorts for a particular species at a site.
        /// </summary>
        public interface ISpeciesCohorts
            //: Landis.Library.AgeOnlyCohorts.ISpeciesCohorts//<ICohort>
             : Landis.Library.Cohorts.ISpeciesCohorts<Landis.Library.BiomassCohorts.ICohort>
        {
        }
    
}
