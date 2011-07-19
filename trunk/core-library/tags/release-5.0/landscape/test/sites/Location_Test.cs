using NUnit.Framework;
using Landis.Landscape;
using System;
using System.Reflection;

//  CS0649 warning is generated by the field "loc" not being assigned.
//	CS1718 warning is generated by the tests of == operator using the same
//	variable as both operands
#pragma warning disable 649, 1718

namespace Landis.Test
{
	[TestFixture]
	public class Location_Test
	{
		Location loc;
		Location loc_1234_987;
		PropertyInfo rowProperty;
		PropertyInfo columnProperty;
		
		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			loc_1234_987 = new Location(1234, 987);
			rowProperty = GetLocationProperty("Row");
			columnProperty = GetLocationProperty("Column");
		}

		//---------------------------------------------------------------------

		private PropertyInfo GetLocationProperty(string propName)
		{
			Type type = typeof(Location);
			return type.GetProperty(propName, BindingFlags.Instance |
			                                  BindingFlags.Public |    // get accessor is public
			                                  BindingFlags.NonPublic); // set accessor is internal
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
			Location loc_0_0 = new Location();
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
			//	Because the instance parameter to the PropertyInfo.SetValue
			//	method is an object, a location argument will be boxed.  So
			//	this code:
			//
			//		Location myLoc = new Location(98, 321);
			//		rowProperty.SetValue(myLoc, (uint) 44, null);
			//
			//	changes the boxed location and leaves the stack variable
			//	"myLoc" unchanged.  To workaround this, we use an already
			//	boxed value for testing.
			object myLocBoxed = new Location(98, 321);
			Assert.AreEqual(98, rowProperty.GetValue(myLocBoxed, null));
			rowProperty.SetValue(myLocBoxed, (uint) 44, null);
			Assert.AreEqual(44, rowProperty.GetValue(myLocBoxed, null));
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test16_SetColumn()
		{
			//	Because the instance parameter to the PropertyInfo.SetValue
			//	method is an object, a location argument will be boxed.  So
			//	this code:
			//
			//		Location myLoc = new Location(98, 321);
			//		columnProperty.SetValue(myLoc, (uint) 7, null);
			//
			//	changes the boxed location and leaves the stack variable
			//	"myLoc" unchanged.  To workaround this, we use an already
			//	boxed value for testing.
			object myLocBoxed = new Location(98, 321);
			Assert.AreEqual(321, columnProperty.GetValue(myLocBoxed, null));
			columnProperty.SetValue(myLocBoxed, (uint) 7, null);
			Assert.AreEqual(7, columnProperty.GetValue(myLocBoxed, null));
		}
	}
}

#pragma warning restore 649, 1718
