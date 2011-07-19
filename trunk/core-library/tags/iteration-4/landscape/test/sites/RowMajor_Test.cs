using System;
using NUnit.Framework;

using Landis.Landscape;

namespace Landis.Test
{
	[TestFixture]
	public class RowMajor_Test
	{
		//---------------------------------------------------------------------

		private Location Loc(uint row,
			                 uint column)
		{
			return new Location(row, column);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test01_NextWith1Col()
		{
			Assert.AreEqual(Loc(2,1), RowMajor.Next(Loc(1,1), 1));
			Assert.AreEqual(Loc(3,1), RowMajor.Next(Loc(2,1), 1));
			                                
			Assert.AreEqual(Loc(790,1), RowMajor.Next(Loc(789,1), 1));
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test02_NextWith89Cols()
		{
			Assert.AreEqual(Loc(1,2), RowMajor.Next(Loc(1,1), 89));
			Assert.AreEqual(Loc(1,3), RowMajor.Next(Loc(1,2), 89));
			Assert.AreEqual(Loc(1,89), RowMajor.Next(Loc(1,88), 89));
			Assert.AreEqual(Loc(2,1), RowMajor.Next(Loc(1,89), 89));

			Assert.AreEqual(Loc(45,89), RowMajor.Next(Loc(45,88), 89));
			Assert.AreEqual(Loc(46,1), RowMajor.Next(Loc(45,89), 89));
		}
	}
}
