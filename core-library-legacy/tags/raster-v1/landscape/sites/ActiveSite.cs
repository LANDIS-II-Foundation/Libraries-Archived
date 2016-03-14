namespace Landis.Landscape
{
	public class ActiveSite
		: Site
	{
		public override bool IsActive {
			get {
				return true;
			}
		}

		//---------------------------------------------------------------------

		internal ActiveSite(ILandscape landscape,
		               	    Location   location,
		               	    uint		 dataIndex)
			: base(landscape, location, dataIndex)
		{
		}
	}
}
