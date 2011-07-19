namespace Landis.Landscape
{
	/// <summary>
	/// The location of a site relative to another site (known as the origin
	/// site).
	/// </summary>
	public struct RelativeLocation
	{
		private int row;
		private int column;

		//---------------------------------------------------------------------

		/// <summary>
		/// The row where the site is located.  Relative to the row of the
		/// origin site.
		/// </summary>
		public int Row {
			get {
				return row;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The column where the site is located.  Relative to the column of
		/// the origin site.
		/// </summary>
		public int Column {
			get {
				return column;
			}
		}

		//---------------------------------------------------------------------

            //!<  Create a new location.
		public RelativeLocation(int row,
		                	    int column)
		{
			this.row    = row;
			this.column = column;
		}

		//---------------------------------------------------------------------

            //!<  Compare two locations for equality.
		public static bool operator ==(RelativeLocation x,
                                       RelativeLocation y)
		{
			return (x.row == y.row) && (x.column == y.column);
		}

		//---------------------------------------------------------------------

            //!<  Compare two locations for inequality.
		public static bool operator !=(RelativeLocation x,
                                       RelativeLocation y)
		{
			return !(x == y);
		}

		//---------------------------------------------------------------------

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType())
				return false;
			RelativeLocation loc = (RelativeLocation)obj;
			return this == loc;
		}

		//---------------------------------------------------------------------

		public override int GetHashCode()
		{
			return (int)(row ^ column);
		}

		//---------------------------------------------------------------------

		public override string ToString()
		{
			return "(" + row + ", " + column + ")";
		}
	}
}
