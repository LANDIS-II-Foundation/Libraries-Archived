namespace Landis.Landscape
{
	public class InactiveSite
		: Site
	{
		public override bool IsActive {
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
