using Landis.Util;
using NUnit.Framework;

namespace Landis.Test.Util
{
	[TestFixture]
	public class Object_Test
	{
		private class TestClassA
		{
		}
		private TestClassA objectA1;
		private TestClassA objectA2;
		private class TestClassB
		{
		}
		private TestClassB objectB;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			objectA1 = new TestClassA();
			objectA2 = new TestClassA();
			objectB = new TestClassB();
		}

		//---------------------------------------------------------------------

		[Test]
		public void Compare_2Nulls()
		{
			Assert.AreEqual(Object.CompareResult.ReferToSame,
			                Object.Compare(null, null));
		}

		//---------------------------------------------------------------------

		[Test]
		public void Compare_LeftOperandNull()
		{
			Assert.AreEqual(Object.CompareResult.OneIsNull,
			                Object.Compare(objectA1, null));
			Assert.AreEqual(Object.CompareResult.OneIsNull,
			                Object.Compare(objectB, null));
		}

		//---------------------------------------------------------------------

		[Test]
		public void Compare_RightOperandNull()
		{
			Assert.AreEqual(Object.CompareResult.OneIsNull,
			                Object.Compare(null, objectA1));
			Assert.AreEqual(Object.CompareResult.OneIsNull,
			                Object.Compare(null, objectB));
		}

		//---------------------------------------------------------------------

		[Test]
		public void Compare_SameObject()
		{
			Assert.AreEqual(Object.CompareResult.ReferToSame,
			                Object.Compare(objectA1, objectA1));
			Assert.AreEqual(Object.CompareResult.ReferToSame,
			                Object.Compare(objectA2, objectA2));
			Assert.AreEqual(Object.CompareResult.ReferToSame,
			                Object.Compare(objectB, objectB));
		}

		//---------------------------------------------------------------------

		[Test]
		public void Compare_DifferentInstances()
		{
			Assert.AreEqual(Object.CompareResult.DifferentInstances,
			                Object.Compare(objectA1, objectA2));
			Assert.AreEqual(Object.CompareResult.DifferentInstances,
			                Object.Compare(objectA1, objectB));
			Assert.AreEqual(Object.CompareResult.DifferentInstances,
			                Object.Compare(objectB, objectA2));
		}
	}
}
