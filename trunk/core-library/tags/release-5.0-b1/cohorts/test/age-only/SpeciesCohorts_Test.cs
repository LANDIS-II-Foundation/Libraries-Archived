using Landis;
using Landis.AgeOnly;
using Landis.Species;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Cohorts.AgeOnly
{
	[TestFixture]
	public class SpeciesCohorts_Test
	{
		private ISpecies species;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			species = Data.Species[0];
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

		[Test]
		public void NoCohorts_Species()
		{
			SpeciesCohorts cohorts = MakeCohorts();
			Assert.AreEqual(species, cohorts.Species);
		}

		//---------------------------------------------------------------------

		[Test]
		public void NoCohorts_Count()
		{
			SpeciesCohorts cohorts = MakeCohorts();
			Assert.AreEqual(0, Count(cohorts));
		}

		//---------------------------------------------------------------------

		[Test]
		public void NoCohorts_Grow()
		{
			SpeciesCohorts cohorts = MakeCohorts();
			cohorts.Grow(5, null, null);
			Assert.AreEqual(0, Count(cohorts));
		}

		//---------------------------------------------------------------------

		[Test]
		public void OneCohort_Species()
		{
			SpeciesCohorts cohorts = MakeCohorts(10);
			Assert.AreEqual(species, cohorts.Species);
		}

		//---------------------------------------------------------------------

		[Test]
		public void OneCohort_Count()
		{
			SpeciesCohorts cohorts = MakeCohorts(10);
			Assert.AreEqual(1, Count(cohorts));
		}

		//---------------------------------------------------------------------

		private void CheckAges(SpeciesCohorts cohorts,
		                       params int[]   agesAsInts)
		{
			ushort[] ages = ToUShorts(agesAsInts);
			System.Array.Sort(ages);

			ushort[] cohortAges = (new List<ushort>(cohorts.Ages)).ToArray();
			System.Array.Sort(cohortAges);

			Assert.AreEqual(ages.Length, cohortAges.Length);
			foreach (int i in Indexes.Of(ages))
				Assert.AreEqual(ages[i], cohortAges[i]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void OneCohort_Grow()
		{
			SpeciesCohorts cohorts = MakeCohorts(10);
			cohorts.Grow(5, null, null);
			Assert.AreEqual(1, Count(cohorts));
			CheckAges(cohorts, 15);
		}

		//---------------------------------------------------------------------

		[Test]
		public void OneCohort_GrowMaxAge()
		{
			SpeciesCohorts cohorts = MakeCohorts(species.Longevity);
			cohorts.Grow(1, null, null);
			Assert.AreEqual(0, Count(cohorts));
		}

		//---------------------------------------------------------------------

		[Test]
		public void ManyCohorts_Species()
		{
			SpeciesCohorts cohorts = MakeCohorts(10, 20, 50, 100, 150);
			Assert.AreEqual(species, cohorts.Species);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ManyCohorts_Count()
		{
			SpeciesCohorts cohorts = MakeCohorts(10, 20, 50, 100, 150);
			Assert.AreEqual(5, Count(cohorts));
		}

		//---------------------------------------------------------------------

		[Test]
		public void ManyCohorts_Grow()
		{
			SpeciesCohorts cohorts = MakeCohorts(10, 20, 50, 100, 150);
			cohorts.Grow(10, null, null);
			CheckAges(cohorts, 20, 30, 60, 110, 160);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ManyCohorts_GrowSomeDie()
		{
			SpeciesCohorts cohorts = MakeCohorts(10, 20, species.Longevity-5,
			                                     species.Longevity);
			cohorts.Grow(10, null, null);
			CheckAges(cohorts, 20, 30);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ManyCohorts_GrowAllDie()
		{
			SpeciesCohorts cohorts = MakeCohorts(10, 20, 50, 100, 150);
			cohorts.Grow((ushort) species.Longevity, null, null);
			Assert.AreEqual(0, Count(cohorts));
		}
	}
}
