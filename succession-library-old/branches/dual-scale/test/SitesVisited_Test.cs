using Edu.Wisc.Forest.Flel.Util;
using Landis.Cohorts.TypeIndependent;
using TypeIndependent = Landis.Cohorts.TypeIndependent;
using Landis.Succession;
using NUnit.Framework;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Test.Succession
{
    [TestFixture]
    public class SitesVisited_Test
    {
        public class MySuccession
            : NullSuccession
        {
            public List<Location> SitesVisited;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public MySuccession(int timestep)
                : base("My Succession")
            {
                Timestep = timestep;
                SitesVisited = new List<Location>();
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public new ISiteVar<bool> Disturbed
            {
                get {
                    return base.Disturbed;
                }
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public void Initialize(PlugIns.ICore modelCore)
            {
                Initialize(modelCore,
                           null,         // double[,] establishProbabilities,
                           SeedingAlgorithms.NoDispersal,
                           null);        // Reproduction.Delegates.AddNewCohort
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            protected override void AgeCohorts(ActiveSite site,
                                               ushort     years,
                                               int?       successionTimestep)
            {
                SitesVisited.Add(site.Location);
            }
        }

        //---------------------------------------------------------------------

        public class MyCohorts
            : AgeCohort.ILandscapeCohorts,
              TypeIndependent.ILandscapeCohorts
        {
            AgeCohort.ISiteCohorts Cohorts.ILandscapeCohorts<AgeCohort.ISiteCohorts>.this[Site site]
            {
                get {
                    return null;
                }
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            TypeIndependent.ISiteCohorts TypeIndependent.ILandscapeCohorts.this[Site site]
            {
                get {
                    return null;
                }
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            CohortAttribute[] TypeIndependent.ILandscapeCohorts.CohortAttributes
            {
                get {
                    return null;
                }
            }
        }

        //---------------------------------------------------------------------

        [Test]
        public void Disturbed()
        {
            SimpleCore core = new SimpleCore();
            core.Species = new Species.Dataset(null);
            core.Landscape = DisturbedSites.MixedLandscape;
            core.SuccessionCohorts = new MyCohorts();

            const int successionTimestep = 10;
            MySuccession succession = new MySuccession(successionTimestep);
            succession.Initialize(core);

            //  Assert that no sites are flagged as disturbed before Run()
            foreach (Site site in DisturbedSites.MixedLandscape.AllSites)
                Assert.IsFalse(succession.Disturbed[site]);

            foreach (Location location in DisturbedSites.Locations) {
                ActiveSite site = DisturbedSites.MixedLandscape[location];
                succession.Disturbed[site] = true;
            }

            core.CurrentTime = successionTimestep + 1;
            succession.Run();
            AssertAreEqual(DisturbedSites.Locations,
                           succession.SitesVisited.ToArray());

            //  Confirm that the Disturbed site variable has been reset at end
            //  of the Run() method.
            foreach (Site site in DisturbedSites.MixedLandscape.AllSites)
                Assert.IsFalse(succession.Disturbed[site]);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(Location[] expected,
                                    Location[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }
    }
}
