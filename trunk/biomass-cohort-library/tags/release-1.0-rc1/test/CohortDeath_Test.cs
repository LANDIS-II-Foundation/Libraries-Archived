using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;

using Landis.Biomass;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;

using NUnit.Framework;

using System.Collections.Generic;

namespace Landis.Test.Biomass
{
    [TestFixture]
    public class CohortDeath_Test
    {
        //  Base class for mock disturbances that remove cohorts between the
        //  ages of 5 and 30.
        public class RemoveAgeBetween5And30
        {
            public static readonly PlugInType DisturbanceType = new PlugInType("disturbance:test");
            private ActiveSite currentSite;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public RemoveAgeBetween5And30(ActiveSite currentSite)
            {
                this.currentSite = currentSite;
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public PlugInType Type
            {
                get {
                    return DisturbanceType;
                }
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public ActiveSite CurrentSite
            {
                get {
                    return currentSite;
                }
            }
        }

        //---------------------------------------------------------------------

        //  Mock biomass disturbance.
        public class RemoveAgeBetween5And30_Biomass
            : RemoveAgeBetween5And30,
              IDisturbance
        {
            public RemoveAgeBetween5And30_Biomass(ActiveSite currentSite)
                : base(currentSite)
            {
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            ushort IDisturbance.Damage(ICohort cohort)
            {
                if (5 <= cohort.Age && cohort.Age <= 30)
                    return cohort.Biomass;
                else
                    return 0;
            }
        }

        //---------------------------------------------------------------------

        //  Mock age-only disturbance.
        public class RemoveAgeBetween5And30_AgeOnly
            : RemoveAgeBetween5And30,
              AgeCohort.IDisturbance
        {
            public RemoveAgeBetween5And30_AgeOnly(ActiveSite currentSite)
                : base(currentSite)
            {
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            bool AgeCohort.IDisturbance.Damage(AgeCohort.ICohort cohort)
            {
                if (5 <= cohort.Age && cohort.Age <= 30)
                    return true;
                else
                    return false;
            }
        }

        //---------------------------------------------------------------------

        private ISpecies poputrem;
        private ActiveSite activeSite;
        private ActiveSite activeSite2;
        private object expectedSender;
        private PlugInType expectedDistType;
        private ActiveSite expectedSite;
        private MockCalculator mockCalculator;
        private const int successionTimestep = 20;
        private Dictionary<ISpecies, ushort[]> expectedCohorts;
        private List<ICohort> deadCohorts;
        private List<ICohort> cohortsKilledByAgeOnlyDist;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            poputrem = Data.Species["poputrem"];

            bool[,] grid = new bool[,]{
                {true,  false},
                {false, true}
            };
            DataGrid<bool> dataGrid = new DataGrid<bool>(grid);
            ILandscape landscape = new Landscape.Landscape(dataGrid);
            activeSite = landscape[1,1];
            activeSite2 = landscape[2,2];

            expectedSender = null;
            expectedDistType = null;
            expectedSite = null;

            mockCalculator = new MockCalculator();
            Landis.Biomass.Cohorts.Initialize(successionTimestep,
                                              mockCalculator);

            Landis.Biomass.Cohort.DeathEvent += CohortDeath;

            expectedCohorts = new Dictionary<ISpecies, ushort[]>();
            deadCohorts = new List<ICohort>();
            cohortsKilledByAgeOnlyDist = new List<ICohort>();
        }

        //---------------------------------------------------------------------

        [TestFixtureTearDown]
        public void TearDown()
        {
            Landis.Biomass.Cohort.DeathEvent -= CohortDeath;
        }

        //---------------------------------------------------------------------

        public void CohortDeath(object         sender,
                                DeathEventArgs eventArgs)
        {
            Assert.AreEqual(expectedSender, sender);
            Assert.AreEqual(expectedDistType, eventArgs.DisturbanceType);
            Assert.AreEqual(expectedSite, eventArgs.Site);
            deadCohorts.Add(eventArgs.Cohort);
        }

        //---------------------------------------------------------------------

        [Test]
        public void SingleCohort_LongevityReached()
        {
            SiteCohorts cohorts = new SiteCohorts();
            const ushort initialBiomass = 300;
            cohorts.AddNewCohort(poputrem, initialBiomass);

            mockCalculator.CountCalled = 0;
            mockCalculator.Change = 1;

            expectedSender = cohorts[poputrem];
            expectedDistType = null;  // death during growth phase
            expectedSite = activeSite;
            deadCohorts.Clear();

            //  Repeatedly grow for succession timesteps until longevity
            //  reached.
            int time = 0;
            do {
                 time += successionTimestep;
                Util.Grow(cohorts, successionTimestep, activeSite, true);
            } while (time <= poputrem.Longevity);

            expectedCohorts.Clear();
            Util.CheckCohorts(expectedCohorts, cohorts);

            //  Calculator called L times where L is longevity.  Inituitively,
            //  one would think since initial cohort's age is 1, it'd only take
            //  L-1 times to get to the max age (= L).  So the calculator
            //  should be called L-1 times.  But the combining of young cohorts
            //  at the first succession timestep (t_succ = 20) results in the
            //  calculator being called twice with cohort age = t_succ-1 (19).
            //  At the end of year 19, the cohort's age is 20 and the
            //  calculator has been called 19 times.  But at the start of year
            //  20, the combine-young-cohorts operation is done because it's a
            //  succession timestep.  The combine operation takes all the young
            //  cohorts (age <= t_succ = 20) and replaces them with one cohort
            //  with age = t_succ-1 (= 19).  This ensures that after the growth
            //  phase, the cohort's age will be t_succ (20).  So the growth
            //  phase of year 20 calls the calculator for the 20th time with
            //  cohort age 19.
            Assert.AreEqual(poputrem.Longevity, mockCalculator.CountCalled);

            Assert.AreEqual(1, deadCohorts.Count);
            ICohort deadCohort = deadCohorts[0];
            Assert.AreEqual(poputrem, deadCohort.Species);
            Assert.AreEqual(poputrem.Longevity, deadCohort.Age);
            Assert.AreEqual(initialBiomass + (poputrem.Longevity * mockCalculator.Change),
                            deadCohort.Biomass);
        }

        //---------------------------------------------------------------------

        [Test]
        public void SingleCohort_ZeroBiomass()
        {
            SiteCohorts cohorts = new SiteCohorts();
            const ushort initialBiomass = 100;
            cohorts.AddNewCohort(poputrem, initialBiomass);

            mockCalculator.CountCalled = 0;
            mockCalculator.Change = -2;

            expectedSender = cohorts[poputrem];
            expectedDistType = null;  // death during growth phase
            expectedSite = activeSite;
            deadCohorts.Clear();

            for (int time = 1; time <= 60; time++) {
                if (time % successionTimestep == 0)
                    Util.Grow(cohorts, successionTimestep, activeSite, true);
            }

            expectedCohorts.Clear();
            Util.CheckCohorts(expectedCohorts, cohorts);

            Assert.AreEqual(1, deadCohorts.Count);
            ICohort deadCohort = deadCohorts[0];
            Assert.AreEqual(poputrem, deadCohort.Species);
            Assert.AreEqual(initialBiomass / -mockCalculator.Change, deadCohort.Age);
            Assert.AreEqual(0, deadCohort.Biomass);
        }

        //---------------------------------------------------------------------

        private void CohortsRemoved(object disturbance)
        {
            SiteCohorts cohorts = new SiteCohorts();
            const ushort initialBiomass = 40;

            mockCalculator.CountCalled = 0;
            mockCalculator.Change = 1;

            int timeOfLastSucc = 0;
            for (int time = 1; time <= 50; time++) {
                //  Simulate the site being disturbed every 8 years which
                //  results in a new cohort being added.
                bool siteDisturbed = (time % 8 == 0);

                bool isSuccTimestep = (time % successionTimestep == 0);

                if (siteDisturbed || isSuccTimestep) {
                    Util.Grow(cohorts, (ushort) (time - timeOfLastSucc),
                              activeSite, isSuccTimestep);
                    timeOfLastSucc = time;
                }

                if (siteDisturbed)
                    cohorts.AddNewCohort(poputrem, initialBiomass);
            }

            //  Expected sequence of cohort changes:
            //
            //        Time  Grow_________
            //        Last         Cohorts               New
            //  Time  Succ  years  afterwards            Cohort
            //  ----  ----  -----  ---------------       ------
            //    8     0     8                           1(40)
            //   16     8     8    9(48)                  1(40)
            //   20    16     4    20(95*)                       * = 48+3 + 40+3 + 1
            //   24    20     4    24(99)                 1(40)
            //   32    24     8    32(107),9(48)          1(40)
            //   40    32     8    40(115),20(103*)       1(40)  * = 48+7 + 40+7 + 1
            //   48    40     8    48(123),28(111),9(48)  1(40)

            expectedCohorts.Clear();
            expectedCohorts[poputrem] = new ushort[] {
                //  age  biomass
                    48,    123,
                    28,    111,
                     9,     48,
                     1,     40
            };
            Util.CheckCohorts(expectedCohorts, cohorts);

            //  Remove cohorts whose ages are between 10 and 30
            expectedSender = cohorts[poputrem];
            deadCohorts.Clear();
            if (disturbance is IDisturbance)
                cohorts.DamageBy((IDisturbance) disturbance);
            else if (disturbance is AgeCohort.IDisturbance)
                ((AgeCohort.ISiteCohorts) cohorts).DamageBy((AgeCohort.IDisturbance) disturbance);

            expectedCohorts.Clear();
            expectedCohorts[poputrem] = new ushort[] {
                //  age  biomass
                    48,    123,
                     1,     40
            };
            Util.CheckCohorts(expectedCohorts, cohorts);
            
            CheckDeadCohorts(deadCohorts);
        }


        //---------------------------------------------------------------------

        private void CheckDeadCohorts(List<ICohort> cohorts)
        {
            Assert.AreEqual(2, cohorts.Count);
            ushort[] cohortData = new ushort[] {
                //  age  biomass (in young to old order because Remove goes
                //                from back to front)
                     9,     48,
                    28,    111
            };
            for (int i = 0; i < cohorts.Count; i++) {
                ICohort deadCohort = cohorts[i];
                Assert.AreEqual(poputrem, deadCohort.Species);
                Assert.AreEqual(cohortData[i*2], deadCohort.Age);
                Assert.AreEqual(cohortData[i*2+1], deadCohort.Biomass);
            }
        }

        //---------------------------------------------------------------------

        [Test]
        public void BiomassDisturbance()
        {
            expectedDistType = RemoveAgeBetween5And30.DisturbanceType;
            expectedSite = activeSite;
            CohortsRemoved(new RemoveAgeBetween5And30_Biomass(activeSite));
        }

        //---------------------------------------------------------------------

        [Test]
        public void AgeOnlyDisturbance()
        {
            Landis.Biomass.SiteCohorts.AgeOnlyDisturbanceEvent += AgeOnlyDisturbanceEvent;
            Landis.Biomass.Cohort.AgeOnlyDeathEvent += AgeOnlyCohortDeath;
            cohortsKilledByAgeOnlyDist.Clear();

            expectedDistType = RemoveAgeBetween5And30.DisturbanceType;
            expectedSite = activeSite2;
            CohortsRemoved(new RemoveAgeBetween5And30_AgeOnly(activeSite2));
            CheckDeadCohorts(cohortsKilledByAgeOnlyDist);

            Landis.Biomass.Cohort.AgeOnlyDeathEvent -= AgeOnlyCohortDeath;
            Landis.Biomass.SiteCohorts.AgeOnlyDisturbanceEvent -= AgeOnlyDisturbanceEvent;
        }

        //---------------------------------------------------------------------

        public void AgeOnlyDisturbanceEvent(object               sender,
                                            DisturbanceEventArgs eventArgs)
        {
            Assert.IsTrue(sender is SiteCohorts);
            Assert.AreEqual(expectedDistType, eventArgs.DisturbanceType);
            Assert.AreEqual(expectedSite, eventArgs.Site);

            //  No cohorts killed yet
            Assert.AreEqual(0, cohortsKilledByAgeOnlyDist.Count);
        }

        //---------------------------------------------------------------------

        public void AgeOnlyCohortDeath(object         sender,
                                       DeathEventArgs eventArgs)
        {
            Assert.IsTrue(sender is WrappedDisturbance);
            Assert.AreEqual(expectedDistType, eventArgs.DisturbanceType);
            Assert.AreEqual(expectedSite, eventArgs.Site);
            cohortsKilledByAgeOnlyDist.Add(eventArgs.Cohort);
        }
    }
}
