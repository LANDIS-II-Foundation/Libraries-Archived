using NUnit.Framework;

using Landis.Landscape;

//  CS0649 warning is generated by the field "loc" not being assigned.
#pragma warning disable 649

namespace Landis.Test
{
	[TestFixture]
	public class Location_Test
	{
		Location loc;
		Location loc_1234_987;
		
		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			loc_1234_987 = new Location(1234, 987);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test01_DefaultCtorCheckRow()
		{
			Assert.AreEqual(0, loc.Row);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test02_DefaultCtorCheckColumn()
		{
			Assert.AreEqual(0, loc.Column);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test03_CheckRow()
		{
			Assert.AreEqual(1234, loc_1234_987.Row);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test04_CheckColumn()
		{
			Assert.AreEqual(987, loc_1234_987.Column);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test05_EqualityWithSameLoc()
		{
			Assert.IsTrue(loc == loc);
			Assert.IsTrue(loc_1234_987 == loc_1234_987);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test06_EqualityWithDiffLocs()
		{
			Assert.IsFalse(loc == loc_1234_987);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test07_InequalityWithSameLoc()
		{
			Assert.IsFalse(loc != loc);
			Assert.IsFalse(loc_1234_987 != loc_1234_987);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test08_InequalityWithDiffLocs()
		{
			Assert.IsTrue(loc != loc_1234_987);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test09_EqualsWithNull()
		{
			Assert.IsFalse(loc.Equals(null));
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test10_EqualsWithSameObj()
		{
			Assert.IsTrue(loc.Equals(loc));
			Assert.IsTrue(loc_1234_987.Equals(loc_1234_987));
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test11_EqualsWithDiffObj()
		{
			Assert.IsFalse(loc.Equals(loc_1234_987));
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test12_EqualsWithDiffObjSameValue()
		{
			Location loc_0_0;
			Assert.IsTrue(loc.Equals(loc_0_0));
			Assert.IsTrue(loc_1234_987.Equals(new Location(1234, 987)));
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test13_GetHashCode()
		{
			Assert.AreEqual((int)(0 ^ 0), loc.GetHashCode());
			Assert.AreEqual((int)((uint)1234 ^ (uint)987),
			                loc_1234_987.GetHashCode());
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test14_ToString()
		{
			Assert.AreEqual("(0, 0)", loc.ToString());
			Assert.AreEqual("(1234, 987)", loc_1234_987.ToString());
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test15_SetRow()
		{
			Location myLoc = new Location(98, 321);
			myLoc.Row = 44;
			Assert.AreEqual(44, myLoc.Row);
			Assert.AreEqual(new Location(44, 321), myLoc);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test16_SetColumn()
		{
			Location myLoc = new Location(98, 321);
			myLoc.Column = 7;
			Assert.AreEqual(7, myLoc.Column);
			Assert.AreEqual(new Location(98, 7), myLoc);
		}
	}
}

#pragma warning restore 649
