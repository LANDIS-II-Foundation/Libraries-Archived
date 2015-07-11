//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Library.BiomassCohortsPnET
{
    /// <summary>
    /// A disturbance that damages cohorts thereby reducing their biomass.
    /// </summary>
    
    public interface IDisturbance : Landis.Library.BiomassCohorts.IDisturbance
    {
        double CumulativeDefoliation();
     
    }
    
}
