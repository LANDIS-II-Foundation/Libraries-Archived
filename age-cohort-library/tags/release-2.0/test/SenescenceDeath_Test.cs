using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Landis.AgeCohort;
using Landis.Landscape;
using Landis.Species;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.AgeCohort
{
    [TestFixture]
    public class SenescenceDeath_Test
    {
        private ISpecies species;
        private object expectedSender;
        private List<ICohort> deadCohorts;
        private ActiveSite activeSite;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            species = Data.Species[0];
            expectedSender = null;
            deadCohorts = new List<ICohort>();

            bool[,] grid = new bool[,]{ {true} };
            DataGrid<bool> dataGrid = new DataGrid<bool>(grid);
            ILandscape landscape = new Landscape.Landscape(dataGrid);
            activeSite = landscape[1,1];

            Cohort.DeathEvent += MySenescenceDeathMethod;
        }

        //---------------------------------------------------------------------

        [TestFixtureTearDown]
        public void TearDown()
        {
            Cohort.DeathEvent -= MySenescenceDeathMethod;
            expectedSender = null;
        }

        //---------------------------------------------------------------------

        public void MySenescenceDeathMethod(object         sender,
                                            DeathEventArgs eventArgs)
        {
            Assert.AreEqual(expectedSender, sender);
            Assert.AreEqual(null, eventArgs.DisturbanceType);
            Assert.AreEqual(activeSite, eventArgs.Site);
            Assert.IsNotNull(eventArgs.Cohort);
            deadCohorts.Add(eventArgs.Cohort);
        }

        //---------------------------------------------------------------------

        [SetUp]
        public void TestInit()
        {
            expectedSender = null;
            deadCohorts.Clear();
        }

        //---------------------------------------------------------------------

        private ushort[] ToUShorts(int[] ints)
        {
            ushort[] ushorts;
            if (ints == null)
                ushorts = new ushort[0];
            else {
                ushorts = new ushort[ints.Length];
                foreach (int index in Indexes.Of(ints))
                    ushorts[index] = (ushort) ints[index];
            }
            return ushorts;
        }

        //---------------------------------------------------------------------

        private SpeciesCohorts MakeCohorts(params int[] agesAsInts)
        {
            ushort[] ages = ToUShorts(agesAsInts);
            List<ushort> ageList = new List<ushort>();
            foreach (ushort age in ages)
                ageList.Add((ushort)age);
            return new SpeciesCohorts(species, ageList);
        }

        //---------------------------------------------------------------------

        public int CompareCohorts(ICohort x,
                                  ICohort y)
        {
            if (x.Age > y.Age)
                return 1;
            else if (x.Age < y.Age)
                return -1;
            else
                return 0;
        }

        //---------------------------------------------------------------------

        private void CheckDeadCohorts(params int[] agesAsInts)
        {
            ushort[] ages = ToUShorts(agesAsInts);
            System.Array.Sort(ages);

            deadCohorts.Sort(CompareCohorts);

            Assert.AreEqual(ages.Length, deadCohorts.Count);
            foreach (int i in Indexes.Of(ages)) {
                Assert.AreEqual(ages[i], deadCohorts[i].Age);
                Assert.AreEqual(species, deadCohorts[i].Species);
            }
        }

        //---------------------------------------------------------------------

        [Test]
        public void OneCohort()
        {
            SpeciesCohorts cohorts = MakeCohorts(species.Longevity);
            expectedSender = cohorts;
            cohorts.Grow(1, activeSite, null);
            Assert.AreEqual(0, cohorts.Count);
            CheckDeadCohorts(species.Longevity + 1);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ManyCohort()
        {
            SpeciesCohorts cohorts = MakeCohorts(10, 20,
                                                 species.Longevity - 9,
                                                 species.Longevity - 5,
                                                 species.Longevity);
            expectedSender = cohorts;
            cohorts.Grow(10, activeSite, null);
            Assert.AreEqual(2, cohorts.Count);
            CheckDeadCohorts(species.Longevity + 1,
                             species.Longevity + 5,
                             species.Longevity + 10);
        }
    }
}
