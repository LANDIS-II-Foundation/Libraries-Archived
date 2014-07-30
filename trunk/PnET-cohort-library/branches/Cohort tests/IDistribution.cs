//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Edu.Wisc.Forest.Flel.Util;
//using Troschuetz.Random;

namespace Landis.Extension.Insects
{
     

    public interface IDistribution
    {

        DistributionType Name {get;set;}
        double Value1 {get;set;}
        double Value2 {get;set;}
    }

    
    
}
