using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Landis.AgeCohort;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.AgeCohort
{
    [TestFixture]
    public class SpeciesCohortsDisturbance_Test
    {
        private ISpecies abiebals;
        private ISpecies betualle;
        private SiteCohorts siteCohorts;
        private ActiveSite activeSite;
        private MockSpeciesCohortsDisturbance disturbance;
        private Dictionary<ISpecies, List<ushort>> deadCohorts;

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

            disturbance = new MockSpeciesCohortsDisturbance();
            disturbance.CurrentSite = activeSite;
            deadCohorts = new Dictionary<ISpecies, List<ushort>>();

            Cohort.DeathEvent += MyCohortDiedMethod;
        }

        //---------------------------------------------------------------------

        [TestFixtureTearDown]
        public void TearDown()
        {
            Cohort.DeathEvent -= MyCohortDiedMethod;
        }

        //---------------------------------------------------------------------

        public void MyCohortDiedMethod(object         sender,
                                       DeathEventArgs eventArgs)
        {
            Assert.AreEqual(MockSpeciesCohortsDisturbance.Type, eventArgs.DisturbanceType);
            Assert.AreEqual(activeSite, eventArgs.Site);
            Assert.IsNotNull(eventArgs.Cohort);

            ICohort cohort = eventArgs.Cohort;
            ISpeciesCohorts speciesCohorts = sender as ISpeciesCohorts;
            Assert.IsNotNull(speciesCohorts);
            Assert.AreEqual(cohort.Species, speciesCohorts.Species);

            List<ushort> ages;
            if (! deadCohorts.TryGetValue(cohort.Species, out ages)) {
                ages = new List<ushort>();
                deadCohorts[cohort.Species] = ages;
            }
            ages.Add(cohort.Age);
        }

        //---------------------------------------------------------------------

        [SetUp]
        public void TestInit()
        {
            List<ISpeciesCohorts> speciesCohortList = new List<ISpeciesCohorts>();
            speciesCohortList.Add(MakeCohorts(abiebals, 30, 40, 50, 150, 170));
            speciesCohortList.Add(MakeCohorts(betualle, 100, 120, 280, 300));
            siteCohorts = new SiteCohorts(speciesCohortList);

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

        private SpeciesCohorts MakeCohorts(ISpecies     species,
                                           params int[] agesAsInts)
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

        private void CheckCohorts(ISpeciesCohorts speciesCohorts,
                                  params int[]    agesAsInts)
        {
            ushort[] expectedAges = ToUShorts(agesAsInts);
            if (expectedAges.Length == 0) {
                Assert.IsNull(speciesCohorts);
            }
            else {
                Assert.IsNotNull(speciesCohorts);
                List<ushort> ages = new List<ushort>();
                foreach (ICohort cohort in speciesCohorts)
                    ages.Add(cohort.Age);
                AssertAreEqual(expectedAges, ages);
            }
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(ushort[]     expectedAges,
                                    List<ushort> ages)
        {
            System.Array.Sort(expectedAges);
            ages.Sort();
            Assert.AreEqual(expectedAges.Length, ages.Count);
            for (int i = 0; i < expectedAges.Length; i++)
                Assert.AreEqual(expectedAges[i], ages[i]);
        }

        //---------------------------------------------------------------------

        private void CheckDeadCohorts(ISpecies     species,
                                      params int[] agesAsInts)
        {
            ushort[] expectedAges = ToUShorts(agesAsInts);
            if (expectedAges.Length == 0) {
                Assert.IsFalse(deadCohorts.ContainsKey(species));
            }
            else {
                List<ushort> ages;
                Assert.IsTrue(deadCohorts.TryGetValue(species, out ages));
                AssertAreEqual(expectedAges, ages);
            }
        }

        //---------------------------------------------------------------------

        [Test]
        public void ClearCut()
        {
            disturbance.Damage = disturbance.ClearCut;
            siteCohorts.DamageBy(disturbance);

            CheckCohorts(siteCohorts[abiebals]);
            CheckCohorts(siteCohorts[betualle]);

            CheckDeadCohorts(abiebals, 30, 40, 50, 150, 170);
            CheckDeadCohorts(betualle, 100, 120, 280, 300);
        }

        //---------------------------------------------------------------------

        [Test]
        public void BetualleAll()
        {
            disturbance.Damage = disturbance.AllOfSelectedSpecies;
            disturbance.SelectedSpecies = betualle;
            siteCohorts.DamageBy(disturbance);

            CheckCohorts(siteCohorts[abiebals], 30, 40, 50, 150, 170);
            CheckCohorts(siteCohorts[betualle]);

            CheckDeadCohorts(abiebals);
            CheckDeadCohorts(betualle, 100, 120, 280, 300);
        }

        //---------------------------------------------------------------------

        [Test]
        public void AbiebalsOldest()
        {
            disturbance.Damage = disturbance.OldestOfSelectedSpecies;
            disturbance.SelectedSpecies = abiebals;
            siteCohorts.DamageBy(disturbance);

            CheckCohorts(siteCohorts[abiebals], 30, 40, 50, 150);
            CheckCohorts(siteCohorts[betualle], 100, 120, 280, 300);

            CheckDeadCohorts(abiebals, 170);
            CheckDeadCohorts(betualle);
        }

        //---------------------------------------------------------------------

        [Test]
        public void AllExceptYoungest()
        {
            disturbance.Damage = disturbance.AllExceptYoungest;
            siteCohorts.DamageBy(disturbance);

            CheckCohorts(siteCohorts[abiebals], 30);
            CheckCohorts(siteCohorts[betualle], 100);

            CheckDeadCohorts(abiebals, 40, 50, 150, 170);
            CheckDeadCohorts(betualle, 120, 280, 300);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Every2ndCohort()
        {
            disturbance.Damage = disturbance.Every2ndCohort;
            siteCohorts.DamageBy(disturbance);

            CheckCohorts(siteCohorts[abiebals], 30, 50, 170);
            CheckCohorts(siteCohorts[betualle], 100, 280);

            CheckDeadCohorts(abiebals, 40, 150);
            CheckDeadCohorts(betualle, 120, 300);
        }
    }
}
