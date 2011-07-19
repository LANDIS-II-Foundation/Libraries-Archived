using NUnit.Framework;

using Landis.Landscape;

namespace Landis.Test
{
	[TestFixture]
	public class DataGridBool_Test
	{
		private GridDimensions dimensions;
		private DataGrid<bool> grid;
		private DataGrid<bool> grid_rowColCtor;
		private bool[,] 	   data;
		private GridDimensions dataDims;
		private DataGrid<bool> grid_dataCtor;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			dimensions = new GridDimensions(22, 333);
			grid = new DataGrid<bool>(dimensions);
			grid_rowColCtor = new DataGrid<bool>(dimensions.Rows,
			                                     dimensions.Columns);
			data = new bool[7,5] { {false, false, false, false, false},
								   {false, false, true,  false, false},
								   {false, true,  true,  true,  false},
								   {true,  true,  true,  true,  true },
								   {false, true,  true,  true,  false},
								   {false, false, true,  false, false},
								   {false, false, false, false, false} };
			dataDims = new GridDimensions((uint) data.GetLength(0),
			                              (uint) data.GetLength(1));
			grid_dataCtor = new DataGrid<bool>(data);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DimCtorDims()
		{
			Assert.AreEqual(dimensions, grid.Dimensions);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DimCtorRows()
		{
			Assert.AreEqual(dimensions.Rows, grid.Rows);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DimCtorColumns()
		{
			Assert.AreEqual(dimensions.Columns, grid.Columns);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DimCtorCount()
		{
			Assert.AreEqual(dimensions.Rows * dimensions.Columns,
			                grid.Count);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DimCtorDataType()
		{
			Assert.AreEqual(typeof(bool), grid.DataType);
		}

		//---------------------------------------------------------------------

		[Test]
		public void RowColumnCtorDims()
		{
			Assert.AreEqual(dimensions, grid_rowColCtor.Dimensions);
		}

		//---------------------------------------------------------------------

		[Test]
		public void RowColumnCtorRows()
		{
			Assert.AreEqual(dimensions.Rows, grid_rowColCtor.Rows);
		}

		//---------------------------------------------------------------------

		[Test]
		public void RowColumnCtorColumns()
		{
			Assert.AreEqual(dimensions.Columns, grid_rowColCtor.Columns);
		}

		//---------------------------------------------------------------------

		[Test]
		public void RowColumnCtorCount()
		{
			Assert.AreEqual(dimensions.Rows * dimensions.Columns,
			                grid_rowColCtor.Count);
		}

		//---------------------------------------------------------------------

		[Test]
		public void RowColumnCtorDataType()
		{
			Assert.AreEqual(typeof(bool), grid_rowColCtor.DataType);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DefaultData()
		{
			for (uint row = 1; row <= grid_rowColCtor.Rows; ++row)
				for (uint col = 1; col <= grid_rowColCtor.Columns; ++col)
					Assert.AreEqual(false, grid_rowColCtor[row, col]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DataCtorDims()
		{
			Assert.AreEqual(dataDims, grid_dataCtor.Dimensions);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DataCtorRows()
		{
			Assert.AreEqual(dataDims.Rows, grid_dataCtor.Rows);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DataCtorColumns()
		{
			Assert.AreEqual(dataDims.Columns, grid_dataCtor.Columns);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DataCtorCount()
		{
			Assert.AreEqual(dataDims.Rows * dataDims.Columns,
			                grid_dataCtor.Count);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DataCtorDataType()
		{
			Assert.AreEqual(typeof(bool), grid_dataCtor.DataType);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DataCtorData()
		{
			for (uint row = 1; row <= dataDims.Rows; ++row)
				for (uint col = 1; col <= dataDims.Columns; ++col)
					Assert.AreEqual(data[row-1, col-1],
				                	grid_dataCtor[row, col]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void LocationIndexer()
		{
			bool cellValue = false;
			for (uint row = 1; row <= grid.Rows; ++row)
				for (uint col = 1; col <= grid.Columns; ++col) {
					cellValue = !cellValue;
					Location loc = new Location(row, col);
					grid[loc] = cellValue;
					Assert.AreEqual(cellValue, grid[loc]);
				}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void LocationIndexerRow0()
		{
			Location badLoc = new Location(0, 1);
			grid[badLoc] = true;
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void LocationIndexerColumn0()
		{
			Location badLoc = new Location(1, 0);
			grid[badLoc] = true;
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void LocationIndexerRowTooBig()
		{
			Location badLoc = new Location(dimensions.Rows + 1, 1);
			grid[badLoc] = true;
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void LocationIndexerColumnTooBig()
		{
			Location badLoc = new Location(1, dimensions.Columns + 1);
			grid[badLoc] = true;
		}

		//---------------------------------------------------------------------

		[Test]
		public void RowColumnIndexer()
		{
			bool cellValue = false;
			for (uint row = 1; row <= grid.Rows; ++row)
				for (uint col = 1; col <= grid.Columns; ++col) {
					cellValue = !cellValue;
					grid[row, col] = cellValue;
					Assert.AreEqual(cellValue, grid[row, col]);
				}
		}

		//---------------------------------------------------------------------

		[Test]
		public void Enumerator()
		{
			bool cellValue = false;
			for (uint row = 1; row <= grid.Rows; ++row)
				for (uint col = 1; col <= grid.Columns; ++col) {
					cellValue = !cellValue;
					Location loc = new Location(row, col);
					grid[loc] = cellValue;
				}

			cellValue = false;
			foreach (bool b in grid) {
				cellValue = !cellValue;
				Assert.AreEqual(cellValue, b);
			}
		}
	}
}
