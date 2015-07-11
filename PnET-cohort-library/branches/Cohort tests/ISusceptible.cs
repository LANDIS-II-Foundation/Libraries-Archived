//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;

namespace Landis.Extension.Insects
{
    //public enum DistributionType {Gamma, Beta, Weibull};
    
    public interface ISusceptible
    {

        byte Number{get;set;}
        IDistribution Distribution_80 {get;set;}
        IDistribution Distribution_60 {get;set;}
        IDistribution Distribution_40 {get;set;}
        IDistribution Distribution_20 {get;set;}
        IDistribution Distribution_0 {get;set;}
    }

    
}
