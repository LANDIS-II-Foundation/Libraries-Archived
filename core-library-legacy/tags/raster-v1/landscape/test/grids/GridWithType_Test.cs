using NUnit.Framework;

using Landis.Landscape;

namespace Landis.Test
{
	//  A local class derived from GridWithType class so we can access its
	//	constructors.
	internal class GridWithType
		: Landis.Landscape.GridWithType
	{
		public GridWithType(GridDimensions dimensions,
		                	System.Type    dataType)
			: base(dimensions, dataType)
		{
		}

		//---------------------------------------------------------------------

		public GridWithType(uint        rows,
		                	uint        columns,
		                	System.Type dataType)
			: base(rows, columns, dataType)
		{
		}
	}

	//-------------------------------------------------------------------------

	//	A local class for test purposes.
	internal class Foo
	{
		public Foo()
		{
		}
	}

	//-------------------------------------------------------------------------

	[TestFixture]
	public class GridWithType_Test
	{
		private GridDimensions dims_4321_789;
		private GridWithType grid_4321_789;
		private System.Type grid_4321_789_dataType;

		private GridWithType grid_22_55555;
		private System.Type grid_22_55555_dataType;
		
		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			dims_4321_789 = new GridDimensions(4321, 789);
			grid_4321_789_dataType = typeof(float);
			grid_4321_789 = new GridWithType(dims_4321_789,
			                             	 grid_4321_789_dataType);

			grid_22_55555_dataType = typeof(Foo);
			grid_22_55555 = new GridWithType(22, 55555, grid_22_55555_dataType);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test01_DimCtorDims()
		{
			Assert.AreEqual(dims_4321_789, grid_4321_789.Dimensions);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test02_DimCtorRows()
		{
			Assert.AreEqual(dims_4321_789.Rows, grid_4321_789.Rows);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test03_DimCtorColumns()
		{
			Assert.AreEqual(dims_4321_789.Columns, grid_4321_789.Columns);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test04_DimCtorCount()
		{
			Assert.AreEqual(dims_4321_789.Rows * dims_4321_789.Columns,
			                grid_4321_789.Count);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test05_DimCtorDataType()
		{
			Assert.AreEqual(grid_4321_789_dataType, grid_4321_789.DataType);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test06_RowsColsCtorDims()
		{
			Assert.AreEqual(new GridDimensions(22, 55555),
			                grid_22_55555.Dimensions);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test07_RowsColsCtorRows()
		{
			Assert.AreEqual(22, grid_22_55555.Rows);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test08_RowsColsCtorColumns()
		{
			Assert.AreEqual(55555, grid_22_55555.Columns);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test09_RowsColsCtorCount()
		{
			Assert.AreEqual(22 * 55555, grid_22_55555.Count);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test10_RowColsCtorDataType()
		{
			Assert.AreEqual(grid_22_55555_dataType, grid_22_55555.DataType);
		}
	}
}
