using Edu.Wisc.Forest.Flel.Grids;

namespace Landis.Landscape
{
	/// <summary>
	/// The location and data index for a site.
	/// </summary>
	public class LocationAndIndex
	{
		private Location location;
		private uint index;

		//-----------------------------------------------------------------

		/// <summary>
		/// The site's location.
		/// </summary>
		public Location Location {
			get {
				return location;
			}

			internal set {
				location = value;
			}
		}

		//-----------------------------------------------------------------

		public uint Row {
			get {
				return location.Row;
			}

			internal set {
		        location = new Location(value, location.Column);
			}
		}

		//-----------------------------------------------------------------

		public uint Column {
			get {
				return location.Column;
			}

			internal set {
		        location = new Location(location.Row, value);
			}
		}

		//-----------------------------------------------------------------

		/// <summary>
		/// The site's data index.
		/// </summary>
		/// <remarks>
		/// A site's data index is used internally by site variables.
		/// </remarks>
		public uint Index {
			get {
				return index;
			}

			internal set {
				index = value;
			}
		}

		//-----------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance with location (0,0) and index 0.
		/// </summary>
		public LocationAndIndex()
		{
			this.location = new Location(0, 0);
			this.index = 0;
		}

		//-----------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance of a location and data-index pair.
		/// </summary>
		public LocationAndIndex(Location location,
		             			uint     index)
		{
			this.location = location;
			this.index    = index;
		}
	}
}
