using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;

using Landis.Biomass;
using Landis.Landscape;
using Landis.Species;

using NUnit.Framework;

using System.Collections.Generic;

namespace Landis.Test.Biomass
{
    [TestFixture]
    public class SiteCohorts_Test
    {
        private ISpecies abiebals;
        private ISpecies betualle;
        private ActiveSite activeSite;
        private MockCalculator mockCalculator;
        private const int successionTimestep = 10;
        private CohortDiedEventHandler cohortDiedEventHandler;
        private Dictionary<ISpecies, ushort[]> expectedCohorts;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            abiebals = Data.Species["abiebals"];
            betualle = Data.Species["betualle"];

            bool[,] grid = new bool[,]{ {true} };
            DataGrid<bool> dataGrid = new DataGrid<bool>(grid);
            ILandscape landscape = new Landscape.Landscape(dataGrid);
            activeSite = landscape[1,1];

            mockCalculator = new MockCalculator();
            Landis.Biomass.Cohorts.Initialize(successionTimestep,
                                              mockCalculator);

            cohortDiedEventHandler = new CohortDiedEventHandler(this.DeathNotExpected);
            Landis.Biomass.Cohort.DiedEvent += cohortDiedEventHandler;

            expectedCohorts = new Dictionary<ISpecies, ushort[]>();
        }

        //---------------------------------------------------------------------

        [TestFixtureTearDown]
        public void TearDown()
        {
            Landis.Biomass.Cohort.DiedEvent -= cohortDiedEventHandler;
        }

        //---------------------------------------------------------------------

        public void DeathNotExpected(ICohort    cohort,
                                     ActiveSite site)
        {
            Assert.Fail("A cohort died unexpectedly");
        }

        //---------------------------------------------------------------------

        [Test]
        public void NoCohorts_Grow()
        {
            SiteCohorts cohorts = new SiteCohorts();
            mockCalculator.CountCalled = 0;
            Util.Grow(cohorts, 5, activeSite, false);
            Assert.AreEqual(0, mockCalculator.CountCalled);
        }

        //---------------------------------------------------------------------

        [Test]
        public void SingleCohort()
        {
            SiteCohorts cohorts = new SiteCohorts();
            const ushort initialBiomass = 300;
            cohorts.AddNewCohort(abiebals, initialBiomass);

            expectedCohorts.Clear();
            expectedCohorts[abiebals] = new ushort[] { 1, initialBiomass };
            Util.CheckCohorts(expectedCohorts, cohorts);
        }

        //---------------------------------------------------------------------

        [Test]
        public void CombineYoungCohorts()
        {
            SiteCohorts cohorts = new SiteCohorts();
            ushort[] initialBiomass = new ushort[] { 300, 700 };
            cohorts.AddNewCohort(abiebals, initialBiomass[0]);

            //  Grow 1st cohort for 4 years, adding 10 to its biomass per year
            mockCalculator.CountCalled = 0;
            mockCalculator.Change = 10;
            Util.Grow(cohorts, 4, activeSite, false);

            Assert.AreEqual(4, mockCalculator.CountCalled);

            expectedCohorts.Clear();
            expectedCohorts[abiebals] = new ushort[] {
                5, (ushort) (300 + 4 * mockCalculator.Change)
            };
            Util.CheckCohorts(expectedCohorts, cohorts);

            //  Add 2nd cohort and then grow both cohorts 6 more years up to
            //  a succession timestep
            cohorts.AddNewCohort(abiebals, initialBiomass[1]);
             mockCalculator.CountCalled = 0;
            Util.Grow(cohorts, 6, activeSite, true);

            //  ComputeChange for both cohorts for 5 years, then combine them,
            //  and then one time for the combined cohort
            Assert.AreEqual(5 * 2 + 1, mockCalculator.CountCalled);

            expectedCohorts.Clear();
            expectedCohorts[abiebals] = new ushort[] {
                successionTimestep,
                (ushort)
                    (300 + (4 + 5) * mockCalculator.Change // first cohort before combining
                     + 700 + 5 * mockCalculator.Change     // 2nd cohort before combining
                     + mockCalculator.Change)              // growth after combining
            };
            Util.CheckCohorts(expectedCohorts, cohorts);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Clone()
        {
            SiteCohorts cohorts = new SiteCohorts();
            const ushort initialBiomass = 55;
            cohorts.AddNewCohort(abiebals, initialBiomass);

            mockCalculator.CountCalled = 0;
            mockCalculator.Change = 1;

            for (int time = successionTimestep; time <= 70; time += successionTimestep) {
                Util.Grow(cohorts, successionTimestep, activeSite, true);
                if (time % 20 == 0)
                    cohorts.AddNewCohort(abiebals, initialBiomass);
                if (time % 30 == 0)
                    cohorts.AddNewCohort(betualle, initialBiomass);
            }

            //  Expected cohort changes:
            //
            //  Time  Cohorts
            //  ----  -------
            //    0   abiebals 1(55)
            //   10   abiebals 10(65)
            //   20   abiebals 20(75) 1(55)
            //   30   abiebals 30(85) 10(65)
            //        betualle 1(55)
            //   40   abiebals 40(95) 20(75) 1(55)
            //        betualle 10(65)
            //   50   abiebals 50(105) 30(85) 10(65)
            //        betualle 20(75)
            //   60   abiebals 60(115) 40(95) 20(75) 1(55)
            //        betualle 30(85) 1(55)
            //   70   abiebals 70(125) 50(105) 30(85) 10(65)
            //        betualle 40(95) 10(65)
            expectedCohorts.Clear();
		    expectedCohorts[abiebals] = new ushort[] {
		        //  age  biomass
		            70,    125,
		            50,    105,
		            30,     85,
		            10,     65
		    };
		    expectedCohorts[betualle] = new ushort[] {
		        //  age  biomass
		            40,     95,
		            10,     65
		    };
		    Util.CheckCohorts(expectedCohorts, cohorts);

		    SiteCohorts clone = cohorts.Clone();
		    Util.CheckCohorts(expectedCohorts, clone);

		    //  Modify the original set of cohorts by growing them for 2 more
		    //  succession timesteps.  Check that clone doesn't change.
            for (int time = 80; time <= 90; time += successionTimestep) {
                Util.Grow(cohorts, successionTimestep, activeSite, true);
		    }
		    Util.CheckCohorts(expectedCohorts, clone);

            expectedCohorts.Clear();
		    expectedCohorts[abiebals] = new ushort[] {
		        //  age  biomass
		            90,    145,
		            70,    125,
		            50,    105,
		            30,     85
		    };
		    expectedCohorts[betualle] = new ushort[] {
		        //  age  biomass
		            60,    115,
		            30,     85
		    };
		    Util.CheckCohorts(expectedCohorts, cohorts);
        }
    }
}
