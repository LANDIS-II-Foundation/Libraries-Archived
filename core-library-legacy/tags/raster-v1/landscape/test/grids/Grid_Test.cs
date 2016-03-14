using NUnit.Framework;

using Landis.Landscape;

namespace Landis.Test
{
	//  A local class derived from Grid class so we can access its
	//	constructors.
	internal class Grid
		: Landis.Landscape.Grid
	{
		public Grid(GridDimensions dimensions)
			: base(dimensions)
		{
		}

		//---------------------------------------------------------------------

		public Grid(uint rows,
		            uint columns)
			: base(rows, columns)
		{
		}
	}
		
	//-------------------------------------------------------------------------

	[TestFixture]
	public class Grid_Test
	{
		private GridDimensions dims_4321_789;
		private Grid grid_4321_789;
		private Grid grid_22_55555;
		private IGrid iGrid_22_55555;
		
		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			dims_4321_789 = new GridDimensions(4321, 789);
			grid_4321_789 = new Grid(dims_4321_789);
			grid_22_55555 = new Grid(22, 55555);
			iGrid_22_55555 = grid_22_55555;
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
		public void Test05_RowsColsCtorDims()
		{
			Assert.AreEqual(new GridDimensions(22, 55555),
			                grid_22_55555.Dimensions);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test06_RowsColsCtorRows()
		{
			Assert.AreEqual(22, grid_22_55555.Rows);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test07_RowsColsCtorColumns()
		{
			Assert.AreEqual(55555, grid_22_55555.Columns);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test08_RowsColsCtorCount()
		{
			Assert.AreEqual(22 * 55555, grid_22_55555.Count);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test09_IGridDims()
		{
			Assert.AreEqual(grid_22_55555.Dimensions,
			                iGrid_22_55555.Dimensions);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test10_IGridRows()
		{
			Assert.AreEqual(grid_22_55555.Rows, iGrid_22_55555.Rows);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test11_IGridColumns()
		{
			Assert.AreEqual(grid_22_55555.Columns, iGrid_22_55555.Columns);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Test12_IGridCount()
		{
			Assert.AreEqual(grid_22_55555.Count, iGrid_22_55555.Count);
		}
	}
}
