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
    public class OneYearTimestep_Test
    {
        private ISpecies abiebals;
        private ActiveSite activeSite;
        private MockCalculator mockCalculator;
        private const int successionTimestep = 1;
        private Dictionary<ISpecies, ushort[]> expectedCohorts;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            abiebals = Data.Species["abiebals"];

            bool[,] grid = new bool[,]{ {true} };
            DataGrid<bool> dataGrid = new DataGrid<bool>(grid);
            ILandscape landscape = new Landscape.Landscape(dataGrid);
            activeSite = landscape[1,1];

            mockCalculator = new MockCalculator();
            Landis.Biomass.Cohorts.Initialize(successionTimestep,
                                              mockCalculator);

            Landis.Biomass.Cohort.DeathEvent += DeathNotExpected;

            expectedCohorts = new Dictionary<ISpecies, ushort[]>();
        }

        //---------------------------------------------------------------------

        [TestFixtureTearDown]
        public void TearDown()
        {
            Landis.Biomass.Cohort.DeathEvent -= DeathNotExpected;
        }

        //---------------------------------------------------------------------

        public void DeathNotExpected(object         sender,
                                     DeathEventArgs eventArgs)
        {
            Assert.Fail("A cohort died unexpectedly");
        }

        //---------------------------------------------------------------------

        [Test]
        public void Grow()
        {
            SiteCohorts cohorts = new SiteCohorts();
            const int initialBiomass = 35;
            cohorts.AddNewCohort(abiebals, initialBiomass);

            mockCalculator.CountCalled = 0;
            mockCalculator.Change = 8;

            cohorts.Grow(activeSite, true);

            expectedCohorts.Clear();
            expectedCohorts[abiebals] = new ushort[] {
                //  age  biomass
                     2,   (int) (initialBiomass + mockCalculator.Change)
            };
            Util.CheckCohorts(expectedCohorts, cohorts);
        }
    }
}
