using NUnit.Framework;

using Landis.Landscape;

namespace Landis.Test
{
	[TestFixture]
	public class InputGridBool_Test
	{
		private GridDimensions  dimensions;
		private bool[,] 	    data;
		private DataGrid<bool>  dataGrid;
		private InputGrid<bool> grid;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			data = new bool[7,5] { {false, false, false, false, false},
								   {false, false, true,  false, false},
								   {false, true,  true,  true,  false},
								   {true,  true,  true,  true,  true },
								   {false, true,  true,  true,  false},
								   {false, false, true,  false, false},
								   {false, false, false, false, false} };
			dimensions = new GridDimensions((uint) data.GetLength(0),
			                                (uint) data.GetLength(1));
			dataGrid = new DataGrid<bool>(data);
			grid = new InputGrid<bool>(dataGrid);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GridCtor_Dims()
		{
			Assert.AreEqual(dimensions, grid.Dimensions);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GridCtor_Rows()
		{
			Assert.AreEqual(dimensions.Rows, grid.Rows);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GridCtor_Columns()
		{
			Assert.AreEqual(dimensions.Columns, grid.Columns);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GridCtor_Count()
		{
			Assert.AreEqual(dimensions.Rows * dimensions.Columns,
			                grid.Count);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GridCtor_DataType()
		{
			Assert.AreEqual(typeof(bool), grid.DataType);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GridCtor_ReadValue()
		{
			for (uint row = 1; row <= dimensions.Rows; ++row)
				for (uint col = 1; col <= dimensions.Columns; ++col)
					Assert.AreEqual(data[row-1, col-1], grid.ReadValue());
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IO.EndOfStreamException))]
		public void GridCtor_ReadPastEnd()
		{
			InputGrid<bool> myGrid = new InputGrid<bool>(dataGrid);
			
			for (uint row = 1; row <= dimensions.Rows; ++row)
				for (uint col = 1; col <= dimensions.Columns; ++col)
					Assert.AreEqual(data[row-1, col-1], myGrid.ReadValue());

			myGrid.ReadValue();
		}
	}
}
