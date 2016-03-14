using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;

using System.Diagnostics;

namespace Landis.Landscape
{
	/// <summary>
	/// An individual site on a landscape.
	/// </summary>
	public abstract class Site
//		: ISite
	{
		private ILandscape landscape;
		private LocationAndIndex locationAndIndex;

		//---------------------------------------------------------------------

		public ILandscape Landscape
		{
			get {
				return landscape;
			}

			protected set {
				landscape = value;
			}
		}

		//---------------------------------------------------------------------

		public Location Location
		{
			get {
				return locationAndIndex.Location;
			}
		}

		//---------------------------------------------------------------------

		public abstract bool IsActive
		{
			get;
		}

		//---------------------------------------------------------------------

        public uint DataIndex
        {
        	get {
        		return locationAndIndex.Index;
        	}
        }

		//---------------------------------------------------------------------

		internal LocationAndIndex LocationAndIndex
		{
			get {
				return locationAndIndex;
			}
		}

		//---------------------------------------------------------------------

		public abstract bool IsMutable
		{
			get;
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
			this.locationAndIndex = new LocationAndIndex(location, dataIndex);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance for a site on a landscape.
		/// </summary>
		/// <param name="landscape">
		///  The landscape where the site is located.
		/// </param>
		/// <param name="locationAndIndex">
		///  The location of the site, and the index of its data for site
		///  variables.
		/// </param>
		internal protected Site(ILandscape       landscape,
				                LocationAndIndex locationAndIndex)
		{
           	Debug.Assert( landscape.IsValid(locationAndIndex.Location) );
			this.landscape = landscape;
			this.locationAndIndex  = locationAndIndex;
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
			Location? neighborLocation = ComputeNeighborLocation(this.Location, location);
			if (neighborLocation.HasValue)
				return landscape.GetSite(neighborLocation.Value);
			return null;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Computes the absolute location of a site's neighbor.
		/// </summary>
		/// <param name="siteLoc">
		/// The site's location.
		/// </param>
		/// <param name="neighborRelLoc">
		/// The location of the neighbor relative to the site.
		/// </param>
		/// <param name="neighborLoc">
		/// The neighbor's location on the landscape.
		/// </param>
		/// <returns>
		/// true if the neighbor is on the landscape; false otherwise (in
		/// which case the
		/// </returns>
		public Location? ComputeNeighborLocation(Location         siteLoc,
		                                         RelativeLocation neighborRelLoc)
		{
			long neighborRow = siteLoc.Row + neighborRelLoc.Row;
			long neighborColumn = siteLoc.Column + neighborRelLoc.Column;
			if (neighborRow < 0 || neighborRow > uint.MaxValue ||
			    neighborColumn < 0 || neighborColumn > uint.MaxValue)
				return null;
			else
				return new Location((uint) neighborRow,
				                    (uint) neighborColumn);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets a neighboring site.
		/// </summary>
		/// <param name="location">
		/// The location of the neighboring site relative to the site.
		/// </param>
		/// <param name="neighbor">
		/// The object which will be assigned the requested site.  If this
		/// parameter is null and the location is valid, then a new instance
		/// will be created and assigned to the parameter.
		/// </param>
		/// <returns>
		/// true if the location is on the landscape, and the information
		/// about the requested site was assigned to the neighbor parameter.
		/// false if the location is not valid (in which case, the neighbor
		/// parameter is unchanged).
		/// </returns>
		public bool GetNeighbor(RelativeLocation location,
		                        ref MutableSite  neighbor)
		{
			Location? neighborLocation = ComputeNeighborLocation(this.Location, location);
			if (neighborLocation.HasValue)
				return landscape.GetSite(neighborLocation.Value, ref neighbor);
			return false;
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
