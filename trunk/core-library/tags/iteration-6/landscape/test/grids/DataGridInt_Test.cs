using NUnit.Framework;

using Landis.Landscape;

namespace Landis.Test
{
	[TestFixture]
	public class DataGridInt_Test
	{
		private GridDimensions dimensions;
		private DataGrid<int>  grid;
		private DataGrid<int>  grid_rowColCtor;
		private int[,] 		   data;
		private GridDimensions dataDims;
		private DataGrid<int>  grid_dataCtor;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			dimensions = new GridDimensions(321, 89);
			grid = new DataGrid<int>(dimensions);
			grid_rowColCtor = new DataGrid<int>(dimensions.Rows,
			                                    dimensions.Columns);
			data = new int[4,7] { {1, 2, 3, 4, 5, 6, 7},
								  {111, 222, 333, 444, 555, 666, 777},
								  {-1, -2, -3, -4, -5, -6, -7},
								  {1, 4, 9, 16, 25, 36, 49} };
			dataDims = new GridDimensions((uint) data.GetLength(0),
			                              (uint) data.GetLength(1));
			grid_dataCtor = new DataGrid<int>(data);
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
			Assert.AreEqual(typeof(int), grid.DataType);
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
			Assert.AreEqual(typeof(int), grid_rowColCtor.DataType);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DefaultData()
		{
			for (uint row = 1; row <= grid_rowColCtor.Rows; ++row)
				for (uint col = 1; col <= grid_rowColCtor.Columns; ++col)
					Assert.AreEqual(0, grid_rowColCtor[row, col]);
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
			Assert.AreEqual(typeof(int), grid_dataCtor.DataType);
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
			int cellCount = 0;
			for (uint row = 1; row <= grid.Rows; ++row)
				for (uint col = 1; col <= grid.Columns; ++col) {
					cellCount++;
					Location loc = new Location(row, col);
					grid[loc] = cellCount;
					Assert.AreEqual(cellCount, grid[loc]);
				}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void LocationIndexerRow0()
		{
			Location badLoc = new Location(0, 1);
			grid[badLoc] = 0;
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void LocationIndexerColumn0()
		{
			Location badLoc = new Location(1, 0);
			grid[badLoc] = 0;
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void LocationIndexerRowTooBig()
		{
			Location badLoc = new Location(dimensions.Rows + 1, 1);
			grid[badLoc] = 0;
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void LocationIndexerColumnTooBig()
		{
			Location badLoc = new Location(1, dimensions.Columns + 1);
			grid[badLoc] = 0;
		}

		//---------------------------------------------------------------------

		[Test]
		public void RowColumnIndexer()
		{
			int cellCount = 0;
			for (uint row = 1; row <= grid.Rows; ++row)
				for (uint col = 1; col <= grid.Columns; ++col) {
					cellCount++;
					grid[row, col] = cellCount;
					Assert.AreEqual(cellCount, grid[row, col]);
				}
		}

		//---------------------------------------------------------------------

		[Test]
		public void Enumerator()
		{
			int cellCount = 0;
			for (uint row = 1; row <= grid.Rows; ++row)
				for (uint col = 1; col <= grid.Columns; ++col) {
					cellCount++;
					Location loc = new Location(row, col);
					grid[loc] = cellCount;
				}

			cellCount = 0;
			foreach (int i in grid) {
				cellCount++;
				Assert.AreEqual(cellCount, i);
			}
		}
	}
}
