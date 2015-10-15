using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;

using Landis.Biomass;
using Landis.Landscape;
using Landis.Species;

using NUnit.Framework;

namespace Landis.Test.Biomass
{
    [TestFixture]
    public class Cohort_Test
    {
        private ISpecies betualle;
        private ActiveSite activeSite;
        private MockCalculator mockCalculator;
        private const int successionTimestep = 10;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            betualle = Data.Species["betualle"];

            bool[,] grid = new bool[,]{ {true} };
            DataGrid<bool> dataGrid = new DataGrid<bool>(grid);
            ILandscape landscape = new Landscape.Landscape(dataGrid);
            activeSite = landscape[1,1];

            mockCalculator = new MockCalculator();
            Landis.Biomass.Cohorts.Initialize(successionTimestep,
                                              mockCalculator);

            Landis.Biomass.Cohort.DeathEvent += DeathNotExpected;
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
        public void NonWoodyPercentage()
        {
            const ushort age = 100;
            const int biomass = 500;
            CohortData data = new CohortData(age, biomass);
            Cohort cohort = new Cohort(betualle, data);

            Percentage nonWoodyPercentage = Percentage.Parse("35%");
            mockCalculator.NonWoodyPercentage = nonWoodyPercentage;
            int expectedNonWoody = (int) (biomass * nonWoodyPercentage);

            int nonWoody = cohort.ComputeNonWoodyBiomass(activeSite);
            Assert.AreEqual(expectedNonWoody, nonWoody);
        }
    }
}
