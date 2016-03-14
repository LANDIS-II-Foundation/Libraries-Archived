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

		/// <summary>
		/// The row where the site is located.
		/// </summary>
		public uint Row {
			get {
				return row;
			}

			internal set {
				row = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The column where the site is located.
		/// </summary>
		public uint Column {
			get {
				return column;
			}

			internal set {
				column = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="row">The row where the site is located.</param>
		/// <param name="column">
		/// The column where the site is located.</param>
		public Location(uint row,
		                uint column)
		{
			this.row    = row;
			this.column = column;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Compares two locations for equality.
		/// </summary>
		public static bool operator ==(Location x, Location y)
		{
			return (x.row == y.row) && (x.column == y.column);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Compare two locations for inequality.
		/// </summary>
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
