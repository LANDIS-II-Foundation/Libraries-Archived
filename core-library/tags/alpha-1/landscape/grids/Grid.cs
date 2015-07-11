namespace Landis.Landscape
{
	//!  Abstract class for grids.

	//!  This class is a building block for grid classes: SiteDataGrid, SiteData,
	//!  and Landscape.
	public class Grid
		: IGrid
	{
		private GridDimensions dimensions;
		private ulong count;

		//---------------------------------------------------------------------

            //!<  The grid's dimensions (rows and columns).
		public GridDimensions Dimensions {
			get {
				return dimensions;
			}
		}

		//---------------------------------------------------------------------

            //!<  The number of rows in the grid.
		public uint Rows {
			get {
				return dimensions.Rows;
			}
		}

		//---------------------------------------------------------------------

            //!<  The number of columns in the grid.
		public uint Columns {
			get {
				return dimensions.Columns;
			}
		}

		//---------------------------------------------------------------------

          //!<  The number of elements (cells) in the grid.
		public ulong Count {
			get {
				return count;
			}
		}

		//---------------------------------------------------------------------

            //!<  Create a grid.
		protected Grid(GridDimensions dimensions)
		{
			this.dimensions = dimensions;
			this.count = dimensions.Rows * dimensions.Columns;
		}

		//---------------------------------------------------------------------

            //!<  Create a grid.
		protected Grid(uint rows,
					   uint columns)
		{
           	this.dimensions = new GridDimensions(rows, columns);
			this.count = rows * columns;
		}
	}
}
