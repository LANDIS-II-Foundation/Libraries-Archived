namespace Landis.Landscape
{
	/// <summary>
	/// The location of a site on a landscape.
	/// </summary>
	public struct Location
	{
		private uint row;
		private uint column;

		//---------------------------------------------------------------------

            //!<  The row where the site is located.
		public uint Row {
			get {
				return row;
			}
		}

		//---------------------------------------------------------------------

            //!<  The column where the site is located.
		public uint Column {
			get {
				return column;
			}
		}

		//---------------------------------------------------------------------

            //!<  Create a new location.
		public Location(uint row,
		                uint column)
		{
			this.row    = row;
			this.column = column;
		}

		//---------------------------------------------------------------------

            //!<  Compare two locations for equality.
		public static bool operator ==(Location x, Location y)
		{
			return (x.row == y.row) && (x.column == y.column);
		}

		//---------------------------------------------------------------------

            //!<  Compare two locations for inequality.
		public static bool operator !=(Location x, Location y)
		{
			return !(x == y);
		}

		//---------------------------------------------------------------------

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType())
				return false;
			Location loc = (Location)obj;
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
