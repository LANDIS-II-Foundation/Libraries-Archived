using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Biomass;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;
using Landis.Test.AgeCohort;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Biomass
{
    [TestFixture]
    public class SpeciesCohortsDisturbance_Test
    {
        private ISpecies abiebals;
        private ISpecies betualle;
        private SiteCohorts initialSiteCohorts;
        private ActiveSite activeSite;
        private MockCalculator mockCalculator;
        private const int successionTimestep = 10;
        private MockSpeciesCohortsDisturbance disturbance;
        private Dictionary<ISpecies, List<ushort>> deadCohorts;
        private int deadCohortsBiomass;

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
            CreateInitialCohorts();

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
            deadCohortsBiomass += cohort.Biomass;
        }

        //---------------------------------------------------------------------

        private void CreateInitialCohorts()
        {
            ushort[] abiebalsAges = new ushort[]{30, 40, 50, 150, 170};
            ushort[] betualleAges = new ushort[]{100, 120, 280, 300};

            //  Work with ages from oldest to youngest
            System.Array.Sort(abiebalsAges, Landis.AgeCohort.Util.WhichIsOlder);
            System.Array.Sort(betualleAges, Landis.AgeCohort.Util.WhichIsOlder);

            //  Loop through succession timesteps from the time when the
            //  oldest cohort would have been added to site (= -{its age}) to
            //  the present (time = 0).  Each cohort is added to the site
            //  when time = -{its age}.
            initialSiteCohorts = new SiteCohorts();
            const ushort initialBiomass = 55;
            List<ushort> abiebalsAgesLeft = new List<ushort>(abiebalsAges);
            List<ushort> betualleAgesLeft = new List<ushort>(betualleAges);
            ushort maxAge = System.Math.Max(abiebalsAgesLeft[0], betualleAgesLeft[0]);
            for (int time = -maxAge; time <= 0; time += successionTimestep) {
                Util.Grow(initialSiteCohorts, successionTimestep, activeSite, true);
                if (abiebalsAgesLeft.Count > 0) {
                    if (time == -(abiebalsAgesLeft[0])) {
                        initialSiteCohorts.AddNewCohort(abiebals, initialBiomass);
                        abiebalsAgesLeft.RemoveAt(0);
                    }
                }
                if (betualleAgesLeft.Count > 0) {
                    if (time == -(betualleAgesLeft[0])) {
                        initialSiteCohorts.AddNewCohort(betualle, initialBiomass);
                        betualleAgesLeft.RemoveAt(0);
                    }
                }
            }
        }

        //---------------------------------------------------------------------

        [SetUp]
        public void TestInit()
        {
            deadCohorts.Clear();
            deadCohortsBiomass = 0;
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
            SiteCohorts siteCohorts = initialSiteCohorts.Clone();
            ((Landis.AgeCohort.ISiteCohorts) siteCohorts).DamageBy(disturbance);

            CheckCohorts(siteCohorts[abiebals]);
            CheckCohorts(siteCohorts[betualle]);

            CheckDeadCohorts(abiebals, 30, 40, 50, 150, 170);
            CheckDeadCohorts(betualle, 100, 120, 280, 300);

            Assert.AreEqual(initialSiteCohorts.TotalBiomass - deadCohortsBiomass,
                            siteCohorts.TotalBiomass);
        }

        //---------------------------------------------------------------------

        [Test]
        public void BetualleAll()
        {
            disturbance.Damage = disturbance.AllOfSelectedSpecies;
            disturbance.SelectedSpecies = betualle;
            SiteCohorts siteCohorts = initialSiteCohorts.Clone();
            ((Landis.AgeCohort.ISiteCohorts) siteCohorts).DamageBy(disturbance);

            CheckCohorts(siteCohorts[abiebals], 30, 40, 50, 150, 170);
            CheckCohorts(siteCohorts[betualle]);

            CheckDeadCohorts(abiebals);
            CheckDeadCohorts(betualle, 100, 120, 280, 300);

            Assert.AreEqual(initialSiteCohorts.TotalBiomass - deadCohortsBiomass,
                            siteCohorts.TotalBiomass);
        }

        //---------------------------------------------------------------------

        [Test]
        public void AbiebalsOldest()
        {
            disturbance.Damage = disturbance.OldestOfSelectedSpecies;
            disturbance.SelectedSpecies = abiebals;
            SiteCohorts siteCohorts = initialSiteCohorts.Clone();
            ((Landis.AgeCohort.ISiteCohorts) siteCohorts).DamageBy(disturbance);

            CheckCohorts(siteCohorts[abiebals], 30, 40, 50, 150);
            CheckCohorts(siteCohorts[betualle], 100, 120, 280, 300);

            CheckDeadCohorts(abiebals, 170);
            CheckDeadCohorts(betualle);

            Assert.AreEqual(initialSiteCohorts.TotalBiomass - deadCohortsBiomass,
                            siteCohorts.TotalBiomass);
        }

        //---------------------------------------------------------------------

        [Test]
        public void AllExceptYoungest()
        {
            disturbance.Damage = disturbance.AllExceptYoungest;
            SiteCohorts siteCohorts = initialSiteCohorts.Clone();
            ((Landis.AgeCohort.ISiteCohorts) siteCohorts).DamageBy(disturbance);

            CheckCohorts(siteCohorts[abiebals], 30);
            CheckCohorts(siteCohorts[betualle], 100);

            CheckDeadCohorts(abiebals, 40, 50, 150, 170);
            CheckDeadCohorts(betualle, 120, 280, 300);

            Assert.AreEqual(initialSiteCohorts.TotalBiomass - deadCohortsBiomass,
                            siteCohorts.TotalBiomass);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Every2ndCohort()
        {
            disturbance.Damage = disturbance.Every2ndCohort;
            SiteCohorts siteCohorts = initialSiteCohorts.Clone();
            ((Landis.AgeCohort.ISiteCohorts) siteCohorts).DamageBy(disturbance);

            CheckCohorts(siteCohorts[abiebals], 30, 50, 170);
            CheckCohorts(siteCohorts[betualle], 100, 280);

            CheckDeadCohorts(abiebals, 40, 150);
            CheckDeadCohorts(betualle, 120, 300);

            Assert.AreEqual(initialSiteCohorts.TotalBiomass - deadCohortsBiomass,
                            siteCohorts.TotalBiomass);
        }
    }
}
