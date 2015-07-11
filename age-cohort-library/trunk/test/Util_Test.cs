using Edu.Wisc.Forest.Flel.Util;
using Landis.AgeCohort;
using AgeCohortUtil = Landis.AgeCohort.Util;
using Landis.Species;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.AgeCohort
{
    [TestFixture]
    public class Util_Test
    {
        private ISpecies species1;
        private ISpecies species2;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            species1 = Data.Species[0];
            species2 = Data.Species[1];
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

        [Test]
        public void GetMaxAge_NoSpeciesCohorts()
        {
            SpeciesCohorts cohorts = MakeCohorts(species1);
            Assert.AreEqual(0, AgeCohortUtil.GetMaxAge(cohorts));
        }

        //---------------------------------------------------------------------

        [Test]
        public void GetMaxAge_1SpeciesCohort()
        {
            int age = 10;
            SpeciesCohorts cohorts = MakeCohorts(species1, age);
            Assert.AreEqual(age, AgeCohortUtil.GetMaxAge(cohorts));
        }

        //---------------------------------------------------------------------

        [Test]
        public void GetMaxAge_ManySpeciesCohorts()
        {
            SpeciesCohorts cohorts = MakeCohorts(species1, 10, 20, 50, 100, 150);
            Assert.AreEqual(150, AgeCohortUtil.GetMaxAge(cohorts));
        }

        //---------------------------------------------------------------------

        [Test]
        public void GetMaxAge_ManySiteCohorts()
        {
            SpeciesCohorts cohorts1 = MakeCohorts(species1, 10, 20, 50, 100, 150);
            SpeciesCohorts cohorts2 = MakeCohorts(species2, 170, 190, 200);

            List<ISpeciesCohorts> list = new List<ISpeciesCohorts>();
            list.Add(cohorts1);
            list.Add(cohorts2);

            SiteCohorts siteCohorts = new SiteCohorts(list);
            Assert.AreEqual(200, AgeCohortUtil.GetMaxAge(siteCohorts));
        }

        //---------------------------------------------------------------------

        [Test]
        public void WhichIsOlderCohort_XOlder()
        {
            Cohort cohortX = new Cohort(species1, 200);
            Cohort cohortY = new Cohort(species1, 100);
            int result = AgeCohortUtil.WhichIsOlderCohort(cohortX, cohortY);
            Assert.IsTrue(result < 0);
        }

        //---------------------------------------------------------------------

        [Test]
        public void WhichIsOlderCohort_Same()
        {
            Cohort cohortX = new Cohort(species1, 200);
            Cohort cohortY = new Cohort(species1, cohortX.Age);
            int result = AgeCohortUtil.WhichIsOlderCohort(cohortX, cohortY);
            Assert.IsTrue(result == 0);
        }

        //---------------------------------------------------------------------

        [Test]
        public void WhichIsOlderCohort_YOlder()
        {
            Cohort cohortX = new Cohort(species1, 20);
            Cohort cohortY = new Cohort(species1, 100);
            int result = AgeCohortUtil.WhichIsOlderCohort(cohortX, cohortY);
            Assert.IsTrue(result > 0);
        }
    }
}
