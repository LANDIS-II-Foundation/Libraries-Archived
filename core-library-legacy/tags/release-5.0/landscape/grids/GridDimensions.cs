namespace Landis.Landscape
{
	/// <summary>
	/// The dimensions of a grid.
	/// </summary>
	public struct GridDimensions
	{
		private uint rows;
		private uint columns;

		//---------------------------------------------------------------------

            //!<  The number of rows.
		public uint Rows {
			get {
				return rows;
			}
		}

		//---------------------------------------------------------------------

            //!<  The number of columns.
		public uint Columns {
			get {
				return columns;
			}
		}

		//---------------------------------------------------------------------

            //!<  Create a new set of dimensions.
		public GridDimensions(uint rows,
		                	  uint columns)
		{
			this.rows    = rows;
			this.columns = columns;
		}

		//---------------------------------------------------------------------

            //!<  Compare two locations for equality.
		public static bool operator ==(GridDimensions x,
                                       GridDimensions y)
		{
			return (x.rows == y.rows) && (x.columns == y.columns);
		}

		//---------------------------------------------------------------------

            //!<  Compare two locations for inequality.
		public static bool operator !=(GridDimensions x,
                                       GridDimensions y)
		{
			return !(x == y);
		}

		//---------------------------------------------------------------------

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType())
				return false;
			GridDimensions loc = (GridDimensions)obj;
			return this == loc;
		}

		//---------------------------------------------------------------------

		public override int GetHashCode()
		{
			return (int)(rows ^ columns);
		}

		//---------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("{0:#,##0} row{1} by {2:#,##0} column{3}",
			                     rows, (rows == 1 ? "" : "s"),
			                     columns, (columns == 1 ? "" : "s"));
		}
	}
}
