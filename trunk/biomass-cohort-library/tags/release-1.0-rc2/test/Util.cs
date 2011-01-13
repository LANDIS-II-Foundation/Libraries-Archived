using Landis.Biomass;
using Landis.Landscape;
using Landis.Species;

using NUnit.Framework;

using System.Collections.Generic;

namespace Landis.Test.Biomass
{
    public static class Util
    {
        /// <summary>
        /// Checks that two sets of site cohorts are the same.
        /// </summary>
        /// <param name="expected">
        /// Keys are expected species.  A species' value is an array of the
        /// ages and biomasses of its expected cohorts from oldest to youngest.
        /// Ages and biomasses are alternate:
        /// 
        /// <pre>
        ///     item 0 = expected age of oldest cohort
        ///     item 1 =    "     biomass  "      "
        ///     item 2 =    "     age of 2nd oldest cohort
        ///     item 3 =    "     biomass "    "      "
        ///     ...
        /// </pre>
        /// </param>
        /// <param name="actual">
        /// The actual site cohorts to verify.
        /// </param>
        public static void CheckCohorts(Dictionary<ISpecies, ushort[]> expected,
                                        ISiteCohorts                   actual)
        {
            foreach (ISpecies species in expected.Keys) {
                ISpeciesCohorts speciesCohorts = actual[species];
                Assert.IsNotNull(speciesCohorts);

                //  Assume cohorts are ordered from oldest to youngest
                ushort[] expectedCohortData = expected[species];
                Assert.AreEqual(expectedCohortData.Length, speciesCohorts.Count * 2);
                int i = 0;
                foreach (ICohort cohort in speciesCohorts) {
                    Assert.AreEqual(expectedCohortData[i], cohort.Age);
                    Assert.AreEqual(expectedCohortData[i+1], cohort.Biomass);
                    i += 2;
                }
            }

            //  Check if any extra species beyond those that were expected
            foreach (ISpeciesCohorts speciesCohorts in actual)
                Assert.IsTrue(expected.ContainsKey(speciesCohorts.Species));
        }

        //---------------------------------------------------------------------
    
        /// <summary>
        /// Grows all cohorts at a site for a number of years.
        /// </summary>
        public static void Grow(SiteCohorts cohorts,
                                int         years,
                                ActiveSite  site,
                                bool        isSuccessionTimestep)
        {
            for (int y = 1; y <= years; ++y)
                cohorts.Grow(site, (y == years && isSuccessionTimestep));
        }
    }
}
