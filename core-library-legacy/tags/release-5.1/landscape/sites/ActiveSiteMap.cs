using Edu.Wisc.Forest.Flel.Grids;

using System.Collections.Generic;
using System.Text;

using log4net;

namespace Landis.Landscape
{
	public class ActiveSiteMap
	{
		private uint rows;
		private uint columns;
		private uint[,] indexes;
		private uint count;
		private LocationAndIndex firstActive;
		private LocationAndIndex firstInactive;

		//---------------------------------------------------------------------

		/// <summary>
		/// The data index assigned to inactive sites.
		/// </summary>
		public const uint InactiveSiteDataIndex = 0;

		//---------------------------------------------------------------------

		/// <summary>
		/// The # of active sites.
		/// </summary>
		public uint Count
		{
			get {
				return count;
			}
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// The # of rows in the map.
		/// </summary>
		public uint Rows
		{
			get {
				return rows;
			}
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// The # of rows in the map.
		/// </summary>
		public uint Columns
		{
			get {
				return columns;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The location and data index of the first active site in row-major
		/// order.
		/// </summary>
		public LocationAndIndex FirstActive
		{
			get {
				return firstActive;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The location and data index of the first inactive site in row-major
		/// order.
		/// </summary>
		public LocationAndIndex FirstInactive
		{
			get {
				return firstInactive;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance using an input data grid.
		/// </summary>
		public ActiveSiteMap(IInputGrid<bool> activeSites)
		{
			Initialize(activeSites);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance using an indexable data grid.
		/// </summary>
		public ActiveSiteMap(IIndexableGrid<bool> activeSites)
		{
			Initialize(new InputGrid<bool>(activeSites));
		}

		//---------------------------------------------------------------------

		private void Initialize(IInputGrid<bool> activeSites)
		{
			this.firstActive = null;
			this.firstInactive = null;

			this.rows = activeSites.Rows;
			this.columns = activeSites.Columns;
			this.indexes = new uint[this.rows, this.columns];
			this.count = 0;

			for (uint row = 0; row < this.rows; ++row) {
				for (uint column = 0; column < this.columns; ++column) {
					if (activeSites.ReadValue()) {
						this.count++;
						this.indexes[row, column] = this.count;
						if (this.firstActive == null)
							this.firstActive = new LocationAndIndex(new Location(row+1, column+1), this.count);
					}
					else {
						if (this.firstInactive == null)
							this.firstInactive = new LocationAndIndex(new Location(row+1, column+1), InactiveSiteDataIndex);
					}
				}  // for each column
			}  // for each row

			if (logger.IsDebugEnabled) {
				LogDebug("Active Site Map");
				LogDebug("");
				LogDebug("Input Grid: {0}", activeSites.Dimensions);
				LogDebug("");

				if (firstActive == null)
					LogDebug("First Active: null");
				else
					LogDebug("First Active: {0} {1}", firstActive.Location, firstActive.Index);
				if (firstInactive == null)
					LogDebug("First Inactive: null");
				else
					LogDebug("First Inactive: {0} {1}", firstInactive.Location, firstInactive.Index);
				LogDebug("");

				StringBuilder line = new StringBuilder(8 * (int) this.columns);
				line.Append("Column:");
				for (int column = 1; column <= this.columns; column++)
					line.Append('\t').Append(column);
				LogDebug(line.ToString());

				LogDebug("Row");
				for (int row = 0; row < this.rows; row++) {
					line.Remove(0, line.Length);
					line.Append(row + 1);
					for (int column = 0; column < this.columns; column++)
						line.Append('\t').Append(indexes[row, column]);
					LogDebug(line.ToString());
				}
			}
		}

		//---------------------------------------------------------------------

		private static ILog logger;

		//---------------------------------------------------------------------

		private void LogDebug(string          message,
		                      params object[] mesgArgs)
		{
			logger.Debug(string.Format(message, mesgArgs));
		}

		//---------------------------------------------------------------------

		static ActiveSiteMap()
		{
			logger = LogManager.GetLogger(typeof(ActiveSiteMap));
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The data index for a particular site.
		/// </summary>
		/// <exception cref="System.IndexOutOfRangeException">
		/// The location's row is 0 or > the # of rows in the map, or the
		/// location's column is 0 or > the # of columns in the map.
		/// </exception>
		public uint this[Location location]
		{
			get {
				return indexes[location.Row-1, location.Column-1];
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets the next active site in row-major order.
		/// </summary>
		/// <param name="site">
		/// Current site's location and data index.
		/// </param>
		/// <returns>
		/// true if the next active site's location and data index have been
		/// assigned to the site parameter.  false if there are no more active
		/// sites.
		/// </returns>
		public bool GetNextActive(ref LocationAndIndex site)
		{
			Location nextLoc = RowMajor.Next(site.Location, columns);
			while (nextLoc.Row <= rows) {
				uint index = indexes[nextLoc.Row-1, nextLoc.Column-1];
				if (index != InactiveSiteDataIndex) {
					site.Location = nextLoc;
					site.Index = index;
					return true;
				}
				nextLoc = RowMajor.Next(nextLoc, columns);
			}
			return false;
		}

		//---------------------------------------------------------------------

		public ActiveSiteMapEnumerator GetEnumerator()
		{
			return new ActiveSiteMapEnumerator(this);
		}
	}
}
