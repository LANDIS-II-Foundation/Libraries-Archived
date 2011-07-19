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
			switch (Landis.Util.Object.Compare(x, y)) {
				case Landis.Util.Object.CompareResult.ReferToSame:
					return true;
				case Landis.Util.Object.CompareResult.OneIsNull:
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
