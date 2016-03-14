using Edu.Wisc.Forest.Flel.Grids;

namespace Landis.Landscape
{
	public abstract class InactiveSite
		: Site
	{
		public override bool IsActive
		{
			get {
				return false;
			}
		}

		//---------------------------------------------------------------------

		internal InactiveSite(ILandscape landscape,
		               	  	  Location   location,
			               	  uint		 dataIndex)
			: base(landscape, location, dataIndex)
		{
		}
	}
}
