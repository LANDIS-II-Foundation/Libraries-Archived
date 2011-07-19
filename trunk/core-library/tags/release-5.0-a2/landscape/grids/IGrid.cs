namespace Landis.Landscape
{
	public interface IGrid
	{
            //!<  The grid's dimensions (rows and columns).
		GridDimensions Dimensions {
			get;
		}

		//---------------------------------------------------------------------

            //!<  The number of rows in the grid.
		uint Rows {
			get;
		}

		//---------------------------------------------------------------------

            //!<  The number of columns in the grid.
		uint Columns {
			get;
		}

		//---------------------------------------------------------------------

          //!<  The number of elements (cells) in the grid.
		ulong Count {
			get;
		}
	}
}
