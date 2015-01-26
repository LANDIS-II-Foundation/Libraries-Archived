//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;


namespace Landis.Library.Biomass
{
    /// <summary>
    /// Defoliation of a cohort
    /// </summary>
    public class CohortDefoliation
    {
        /// <summary>
        /// Various delegates associated with defoliation.
        /// </summary>
        public static class Delegates
        {
            /// <summary>
            /// A method to compute the proportion of foliage that is lost due to a disturbance
            /// site.
            /// </summary>
            public delegate double Compute(ActiveSite site,
                                           ISpecies species,
                                           int cohortBiomass,
                                           int siteBiomass);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Defaults for defoliation.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// Default method for computing how much a cohort is defoliated at
            /// a site.
            /// </summary>
            /// <returns>
            /// 0%
            /// </returns>
            public static double Compute( ActiveSite site,
                                         ISpecies species,
                                         int cohortBiomass,
                                         int siteBiomass)
                                         
            {
                return 0.0;
            }
        }

        //---------------------------------------------------------------------

        private static Delegates.Compute computeMethod = Defaults.Compute;

        //---------------------------------------------------------------------

        /// <summary>
        /// The method to compute how much a cohort is defoliated at a site.
        /// </summary>
        public static Delegates.Compute Compute
        {
            get {
                return computeMethod;
            }

            set {
                Require.ArgumentNotNull(value);
                computeMethod = value;
            }
        }
    }
}
