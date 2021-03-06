using Landis;
using Landis.AgeOnly;
using Landis.Landscape;
using Landis.Species;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Cohorts.AgeOnly
{
	[TestFixture]
	public class CohortDeath_Test
	{
		private ISpecies species;
		private List<ICohort> deadCohorts;
		private ActiveSite activeSite;
		private DeathMethod<ICohort> originalDiedMethod;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			species = Data.Species[0];
			deadCohorts = new List<ICohort>();

			bool[,] grid = new bool[,]{ {true} };
			DataGrid<bool> dataGrid = new DataGrid<bool>(grid);
			ILandscape landscape = new Landscape.Landscape(dataGrid);
			activeSite = landscape[1,1];

			originalDiedMethod = Cohort.Died;
			Cohort.Died = MyCohortDiedMethod;
		}

		//---------------------------------------------------------------------

		[TestFixtureTearDown]
		public void TearDown()
		{
			Cohort.Died = originalDiedMethod;
		}

		//---------------------------------------------------------------------

		public void MyCohortDiedMethod(ICohort    cohort,
		                               ActiveSite site)
		{
			Assert.AreEqual(activeSite, site);
			deadCohorts.Add(cohort);
		}

		//---------------------------------------------------------------------

		[SetUp]
		public void TestInit()
		{
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

		private int Count(SpeciesCohorts cohorts)
		{
			int count = 0;
			foreach (ushort age in cohorts.Ages)
				count++;
			return count;
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
			cohorts.Grow(1, activeSite);
			Assert.AreEqual(0, Count(cohorts));
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
			cohorts.Grow(10, activeSite);
			Assert.AreEqual(2, Count(cohorts));
			CheckDeadCohorts(species.Longevity + 1,
			                 species.Longevity + 5,
			                 species.Longevity + 10);
		}
	}
}
