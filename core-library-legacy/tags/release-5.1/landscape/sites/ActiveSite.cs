using Edu.Wisc.Forest.Flel.Grids;

namespace Landis.Landscape
{
	public abstract class ActiveSite
		: Site
	{
		public override bool IsActive
		{
			get {
				return true;
			}
		}

		//---------------------------------------------------------------------

		internal ActiveSite(ILandscape landscape,
		               	    Location   location,
		               	    uint       dataIndex)
			: base(landscape, location, dataIndex)
		{
		}

		//---------------------------------------------------------------------

		internal ActiveSite(ILandscape       landscape,
		               	    LocationAndIndex locationAndIndex)
			: base(landscape, locationAndIndex)
		{
		}
	}
}
