//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Edu.Wisc.Forest.Flel.Util;
//using Troschuetz.Random;

namespace Landis.Extension.Insects
{
    public enum DistributionType {Gamma, Beta, Weibull};

    
    /// <summary>
    /// Definition of a wind severity.
    /// </summary>
    public class Distribution
        : IDistribution
    {
        private DistributionType name;
        private double value1;
        private double value2;


        //---------------------------------------------------------------------
        public DistributionType Name
        {
            get {
                return name;
            }

            set {
                name = value;
            }
        }
        //---------------------------------------------------------------------
        public double Value1
        {
            get {
                return value1;
            }
            set {
                if (value < 0 || value > 100)
                        throw new InputValueException(value.ToString(), "Value must be between 0 and 100");
                value1 = value;
            }
        }
        //---------------------------------------------------------------------
        public double Value2
        {
            get {
                return value2;
            }
            set {
                if (value < 0.0 || value > 10.0)
                        throw new InputValueException(value.ToString(), "Value must be between 0.0 and 10.0");
                value2 = value;
            }
        }

        //---------------------------------------------------------------------

        public Distribution()
        {
        }

/*        public Distribution(
                        DistributionType name,
                        double value1,
                        double value2
                        )
        {
            this.name = name;
            this.value1 = value1;
            this.value2 = value2;
        }*/
        //---------------------------------------------------------------------

        public static double GenerateRandomNum(DistributionType dist, double parameter1, double parameter2)
        {
            double randomNum = 0.0;
            /*if(dist == DistributionType.Normal)
            {
                NormalDistribution randVar = new NormalDistribution(RandomNumberGenerator.Singleton);
                randVar.Mu = parameter1;      // mean
                randVar.Sigma = parameter2;   // std dev
                randomNum = randVar.NextDouble();
            }
            if(dist == DistributionType.Lognormal)
            {
                LognormalDistribution randVar = new LognormalDistribution(RandomNumberGenerator.Singleton);
                randVar.Mu = parameter1;      // mean
                randVar.Sigma = parameter2;   // std dev
                randomNum = randVar.NextDouble();
            }*/
            if(dist == DistributionType.Weibull)
            {
                PlugIn.ModelCore.WeibullDistribution.Alpha = parameter1;// mean
                PlugIn.ModelCore.WeibullDistribution.Lambda = parameter2;// std dev
                randomNum = PlugIn.ModelCore.WeibullDistribution.NextDouble();
                
                //WeibullDistribution randVar = new WeibullDistribution(RandomNumberGenerator.Singleton);
                //randVar.Alpha = parameter1;      
                //randVar.Lambda = parameter2;   
                //randomNum = randVar.NextDouble();
            }

            if(dist == DistributionType.Gamma)
            {
                PlugIn.ModelCore.GammaDistribution.Alpha = parameter1;// mean
                PlugIn.ModelCore.GammaDistribution.Theta = parameter2;// std dev
                randomNum = PlugIn.ModelCore.GammaDistribution.NextDouble();

                //GammaDistribution randVar = new GammaDistribution(RandomNumberGenerator.Singleton);
                //randVar.Alpha = parameter1;      // mean
                //randVar.Theta = parameter2;   // std dev
                //randomNum = randVar.NextDouble();
            }
            if(dist == DistributionType.Beta)
            {
                PlugIn.ModelCore.BetaDistribution.Alpha = parameter1;// mean
                PlugIn.ModelCore.BetaDistribution.Beta = parameter2;// std dev
                if (parameter1 == 0) // Alpha/Beta = 0 returns 0
                    randomNum = 0;
                else if (parameter2 == 0) // Beta/Alpha = 0 returns 1
                    randomNum = 1;
                else
                    randomNum = PlugIn.ModelCore.BetaDistribution.NextDouble();

                //BetaDistribution randVar = new BetaDistribution(RandomNumberGenerator.Singleton);
                //randVar.Alpha = parameter1;      // mean
                //randVar.Beta = parameter2;   // std dev
                //randomNum = randVar.NextDouble();
            }
            return randomNum;
        }
    }
}
