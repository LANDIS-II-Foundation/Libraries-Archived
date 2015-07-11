//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Edu.Wisc.Forest.Flel.Util;


namespace Landis.Extension.Insects
{
    /// <summary>
    /// Extra Spp Paramaters
    /// </summary>
    public interface ISppParameters
    {
        /// <summary>
        /// </summary>
        int Susceptibility{get;set;}
        double GrowthReduceSlope{get;set;}
        double GrowthReduceIntercept{get;set;}
        double MortalitySlope{get;set;}
        double MortalityIntercept{get;set;}
    }
}


namespace Landis.Extension.Insects
{
    public class SppParameters
        : ISppParameters
    {
        private int susceptibility;
        private double growthReduceSlope;
        private double growthReduceIntercept;
        private double mortalitySlope;
        private double mortalityIntercept;

        //---------------------------------------------------------------------
        /// <summary>
        /// Susceptibility to an insect.  1 = most susceptible; 3 = least.
        /// </summary>
        public int Susceptibility
        {
            get
            {
                return susceptibility;
            }
            set {
                if (value != 1 && value != 2 && value != 3)
                        throw new InputValueException(value.ToString(), "Value must be = 1, 2, or 3");
                susceptibility = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// The slope of the relationship between defoliation and growth reduction.
        /// </summary>
        public double GrowthReduceSlope
        {
            get
            {
                return growthReduceSlope;
            }
            set {
                growthReduceSlope = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// The intercept of the relationship between defoliation and growth reduction.
        /// </summary>
        public double GrowthReduceIntercept
        {
            get
            {
                return growthReduceIntercept;
            }
            set {
                growthReduceIntercept = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// The slope of the relationship between defoliation and mortality.
        /// </summary>
        public double MortalitySlope
        {
            get
            {
                return mortalitySlope;
            }
            set {
                mortalitySlope = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// The intercept of the relationship between defoliation and mortality.
        /// </summary>
        public double MortalityIntercept
        {
            get
            {
                return mortalityIntercept;
            }
            set {
                mortalityIntercept = value;
            }
        }
        //---------------------------------------------------------------------
        public SppParameters()
        {
        }
        //---------------------------------------------------------------------
/*        public SppParameters(int susceptibility,
                            double growthReduceSlope,
                            double growthReduceIntercept,
                            double mortalitySlope,
                            double mortalityIntercept
                            )
        {
            this.susceptibility = susceptibility;
            this.growthReduceSlope = growthReduceSlope;
            this.growthReduceIntercept = growthReduceIntercept;
            this.mortalitySlope = mortalitySlope;
            this.mortalityIntercept = mortalityIntercept;
        }

        //---------------------------------------------------------------------

        public SppParameters()
        {
            this.susceptibility = 0;
            this.growthReduceSlope = 0.0;
            this.growthReduceIntercept = 0.0;
            this.mortalitySlope = 0.0;
            this.mortalityIntercept = 0.0;
        }*/
    }
}
