using Edu.Wisc.Forest.Flel.Grids;

namespace Landis.Landscape
{
	public class MutableSite
		: Site
	{
		private bool isActive;

		//---------------------------------------------------------------------

		public override bool IsActive
		{
			get {
				return isActive;
			}
		}

		//---------------------------------------------------------------------

		public override bool IsMutable
		{
			get {
				return true;
			}
		}

		//---------------------------------------------------------------------

		internal MutableSite(ILandscape landscape,
		               	     Location   location,
		               	     bool       isActive,
		               	     uint       dataIndex)
			: base(landscape, location, dataIndex)
		{
			this.isActive = isActive;
		}

		//---------------------------------------------------------------------

		internal void SetLocation(Location location)
		{
			this.LocationAndIndex.Location = location;
		}

		//---------------------------------------------------------------------

		internal void SetAll(Landscape landscape,
		                     Location  location,
		                     bool      isActive,
		                     uint      dataIndex)
		{
			this.Landscape = landscape;
			this.LocationAndIndex.Location = location;
			this.LocationAndIndex.Index    = dataIndex;
			this.isActive = isActive;
		}
	}
}
