using Edu.Wisc.Forest.Flel.Util;
using System.Diagnostics;

namespace Landis.Landscape
{
	/// <summary>
	/// An individual site on a landscape.
	/// </summary>
	public abstract class Site
	{
		private ILandscape landscape;
		private Location location;
		private uint index;

		//---------------------------------------------------------------------

		/// <summary>
		///  The landscape where the site is located.
		/// </summary>
		public ILandscape Landscape
		{
			get {
				return landscape;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		///  The site's location.
		/// </summary>
		public Location Location
		{
			get {
				return location;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		///  Is the site active?
		/// </summary>
		public abstract bool IsActive
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		///  The index of the site's data for the landscape's site variables.
		/// </summary>
        public uint DataIndex
        {
        	get {
        		return index;
        	}
        }

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance for a site on a landscape.
		/// </summary>
		/// <param name="landscape">
		///  The landscape where the site is located.
		/// </param>
		/// <param name="location">
		///  The location of the site.
		/// </param>
		/// <param name="dataIndex">
		///  The index of the site's data for site variables.
		/// </param>
		internal protected Site(ILandscape landscape,
				                Location   location,
				                uint	   dataIndex)
		{
           	Debug.Assert( landscape.IsValid(location) );
			this.landscape = landscape;
			this.location  = location;
			this.index	   = dataIndex;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Are two sites the same site?
		/// </summary>
		public static bool operator ==(Site x, Site y)
		{
			switch (Object.Compare(x, y)) {
				case Object.CompareResult.ReferToSame:
					return true;
				case Object.CompareResult.OneIsNull:
					return false;
			}
			return (x.landscape == y.landscape) && (x.Location == y.Location);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Are two sites different?
		/// </summary>
		/// <remarks>
		/// The sites are different if they are on the same landscape, but
		/// their locations are different, or if they are on different
		/// landscapes.
		/// </remarks>
		public static bool operator !=(Site x, Site y)
		{
			return !(x == y);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets a neighboring site.
		/// </summary>
		/// <param name="location">
		/// The location of the neighboring site relative to the site.
		/// </param>
		/// <returns>
		/// null if the location refers to an absolute location that is not
		/// valid for the site's landscape.
		/// </returns>
		public Site GetNeighbor(RelativeLocation location)
		{
			long neighborRow = this.Location.Row + location.Row;
			long neighborColumn = this.Location.Column + location.Column;
			if (neighborRow < 0 || neighborRow > uint.MaxValue ||
			    neighborColumn < 0 || neighborColumn > uint.MaxValue)
				return null;
			return landscape.GetSite((uint) neighborRow,
			                         (uint) neighborColumn);
		}

		//---------------------------------------------------------------------

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType())
				return false;
			Site site = (Site)obj;
			return this == site;
		}

		//---------------------------------------------------------------------

		//	Site's hash code is the hash code of its location.
		public override int GetHashCode()
		{
			return Location.GetHashCode();
		}
	}
}
