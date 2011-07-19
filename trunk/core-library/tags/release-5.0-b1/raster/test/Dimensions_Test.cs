using Edu.Wisc.Forest.Flel.Util;
using Landis.Raster;
using NUnit.Framework;
using System;

namespace Landis.Test.Raster
{
	[TestFixture]
	public class Dimensions_Test
	{
		//  The test below compares the same object to itself; warning CS1718
		//	notifies user of this situation.
#pragma warning disable 1718
		[Test]
		public void Equals_SameObj()
		{
			Dimensions dims = new Dimensions(10, 333);
			Assert.IsTrue(dims == dims);
		}
#pragma warning restore 1718

		//---------------------------------------------------------------------

		[Test]
		public void Equals_Null()
		{
			Dimensions dims = new Dimensions(100, 77);
			Assert.IsFalse(dims == null);
			Assert.IsFalse(null == dims);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Equals_DiffObjs()
		{
			Dimensions dimsA = new Dimensions(22, 4444);
			Dimensions dimsB = new Dimensions(dimsA.Rows, dimsA.Columns);
			Assert.IsTrue(dimsA == dimsB);
		}
	}
}
