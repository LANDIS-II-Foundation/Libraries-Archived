//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;

namespace Landis.Extension.Insects
{
    
    /// <summary>
    /// Definition of a wind severity.
    /// </summary>
    public class Susceptible
        : ISusceptible
    {
        private byte number;
        private IDistribution distribution_80;
        private IDistribution distribution_60;
        private IDistribution distribution_40;
        private IDistribution distribution_20;
        private IDistribution distribution_0;


        //---------------------------------------------------------------------

        /// <summary>
        /// The severity's number (between 1 and 254).
        /// </summary>
        public byte Number
        {
            get {
                return number;
            }
            set {
                if (value > 5)
                        throw new InputValueException(value.ToString(), "Value must be between 1 and 5.");
                number = value;
            }
        }

        //---------------------------------------------------------------------
        public IDistribution Distribution_80
        {
            get {
                return distribution_80;
            }
            set {
                distribution_80 = value;
            }
        }
        //---------------------------------------------------------------------
        public IDistribution Distribution_60
        {
            get {
                return distribution_60;
            }
            set {
                distribution_60 = value;
            }
        }
        //---------------------------------------------------------------------
        public IDistribution Distribution_40
        {
            get {
                return distribution_40;
            }
            set {
                distribution_40 = value;
            }
        }
        //---------------------------------------------------------------------
        public IDistribution Distribution_20
        {
            get {
                return distribution_20;
            }
            set {
                distribution_20 = value;
            }
        }
        //---------------------------------------------------------------------
        public IDistribution Distribution_0
        {
            get {
                return distribution_0;
            }
            set {
                distribution_0 = value;
            }
        }
        //---------------------------------------------------------------------

        public Susceptible()
        {
        }

    }
}
