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

            //!<  The landscape where the site is located.
		public ILandscape Landscape {
			get {
				return landscape;
			}
		}

		//---------------------------------------------------------------------

            //!<  The site's location.
		public Location Location {
			get {
				return location;
			}
		}

		//---------------------------------------------------------------------

            //!<  Is the site active?
		public abstract bool IsActive {
			get;
		}

		//---------------------------------------------------------------------

            //!<  \brief  The index of the site's data for the landscape's site
            //!           variables.
        public uint DataIndex {
            	get {
            		return index;
            	}
        }

		//---------------------------------------------------------------------

            //!<  \brief  Construct a new object for a site on a landscape.
            //!
            //! \param landscape  The landscape where the site is located.
            //! \param is_active  Is the site active?
            //!        dataIndex
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

            //!<  Are two sites the same site?
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

            //!<  Are two sites the same site?
            //!<  \brief  Are two sites different?
            //!
            //!  The sites are different if they are on the same landscape, but
            //!  their locations are different, or if they are on different
            //!  landscapes.
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
