using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;

using Landis.Biomass;
using Landis.Landscape;
using Landis.Species;

using NUnit.Framework;

namespace Landis.Test.Biomass
{
    [TestFixture]
    public class OldToYoungIterator_Test
    {
        private ISpecies betualle;
        private ActiveSite activeSite;
        private MockCalculator mockCalculator;

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
        public void Timestep10_GrowCurrent()
        {
            OldToYoungIterator itor = CreateAndUseItor(10);
        }

        //---------------------------------------------------------------------

        private OldToYoungIterator CreateAndUseItor(int successionTimestep)
        {
            return CreateAndUseItor(CreateCohorts(successionTimestep));
        }
 
        //---------------------------------------------------------------------

        private SpeciesCohorts CreateCohorts(int successionTimestep)
        {
            Landis.Biomass.Cohorts.Initialize(successionTimestep,
                                              mockCalculator);

            const ushort initialBiomass = 500;
            SpeciesCohorts cohorts = new SpeciesCohorts(betualle, initialBiomass);

            Assert.AreEqual(1, cohorts.Count);
            ICohort cohort = cohorts[0];
            Assert.IsNotNull(cohort);
            Assert.AreEqual(betualle, cohort.Species);
            Assert.AreEqual(1, cohort.Age);
            Assert.AreEqual(initialBiomass, cohort.Biomass);

            return cohorts;
        }
 
        //---------------------------------------------------------------------

        private OldToYoungIterator CreateAndUseItor(SpeciesCohorts cohorts)
        {
            Assert.AreEqual(1, cohorts.Count);
            ICohort cohort = cohorts[0];
            ushort initialAge = cohort.Age;
            ushort initialBiomass = cohort.Biomass;

            OldToYoungIterator itor = cohorts.OldToYoung;
            Assert.IsNotNull(itor);
            Assert.AreEqual(cohorts, itor.SpeciesCohorts);
            Assert.AreEqual(cohort.Age, itor.Age);

            const int initialSiteBiomass = 1234;
            int siteBiomass = initialSiteBiomass;
            const int prevYearMortality = 55;

            const int biomassChange = 111;
            mockCalculator.Change = biomassChange;
            mockCalculator.Mortality = 20;

            int mortality = itor.GrowCurrentCohort(activeSite, ref siteBiomass,
                                                   prevYearMortality);
            Assert.AreEqual(mockCalculator.Mortality, mortality);

            Assert.AreEqual(1, cohorts.Count);
            cohort = cohorts[0];
            Assert.IsNotNull(cohort);
            Assert.AreEqual(betualle, cohort.Species);
            Assert.AreEqual(initialAge + 1, cohort.Age);
            Assert.AreEqual(initialBiomass + biomassChange, cohort.Biomass);

            Assert.IsFalse(itor.MoveNext());
            return itor;
        }

        //---------------------------------------------------------------------

        [Test]
        public void Timestep10_AtEnd_MoveNext()
        {
            OldToYoungIterator itor = CreateAndUseItor(10);
            Assert.IsFalse(itor.MoveNext());
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void Timestep10_AtEnd_GrowCurrent()
        {
            OldToYoungIterator itor = CreateAndUseItor(10);
            int siteBiomass = 0;
            itor.GrowCurrentCohort(activeSite, ref siteBiomass, 0);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void Timestep10_AtEnd_Age()
        {
            OldToYoungIterator itor = CreateAndUseItor(10);
            int dummy = itor.Age;
        }

        //---------------------------------------------------------------------

        [Test]
        public void Timestep1_GrowCurrent()
        {
            OldToYoungIterator itor = CreateAndUseItor(1);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Timestep1_AtEnd_MoveNext()
        {
            OldToYoungIterator itor = CreateAndUseItor(1);
            Assert.IsFalse(itor.MoveNext());
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void Timestep1_AtEnd_GrowCurrent()
        {
            OldToYoungIterator itor = CreateAndUseItor(1);
            int siteBiomass = 0;
            itor.GrowCurrentCohort(activeSite, ref siteBiomass, 0);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void Timestep1_AtEnd_Age()
        {
            OldToYoungIterator itor = CreateAndUseItor(1);
            int dummy = itor.Age;
        }

        //---------------------------------------------------------------------

        private SpeciesCohorts CreateCohortsAndCombineYoung(int successionTimestep)
        {
            SpeciesCohorts cohorts = CreateCohorts(successionTimestep);
            cohorts.CombineYoungCohorts();
            return cohorts;
        }

        //---------------------------------------------------------------------

        [Test]
        public void Timestep1_CombineYoung_GrowCurrent()
        {
            OldToYoungIterator itor = CreateAndUseItor(CreateCohortsAndCombineYoung(1));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Timestep1_CombineYoung_AtEnd_MoveNext()
        {
            OldToYoungIterator itor = CreateAndUseItor(CreateCohortsAndCombineYoung(1));
            Assert.IsFalse(itor.MoveNext());
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void Timestep1_CombineYoung_AtEnd_GrowCurrent()
        {
            OldToYoungIterator itor = CreateAndUseItor(CreateCohortsAndCombineYoung(1));
            int siteBiomass = 0;
            itor.GrowCurrentCohort(activeSite, ref siteBiomass, 0);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void Timestep1_CombineYoung_AtEnd_Age()
        {
            OldToYoungIterator itor = CreateAndUseItor(CreateCohortsAndCombineYoung(1));
            int dummy = itor.Age;
        }
    }
}
