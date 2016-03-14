namespace Landis.Landscape
{
	public class ImmutableActiveSite
		: ActiveSite
	{
		public override bool IsMutable
		{
			get {
				return false;
			}
		}

		//---------------------------------------------------------------------

		internal ImmutableActiveSite(ILandscape landscape,
		               	             Location   location,
		               	             uint       dataIndex)
			: base(landscape, location, dataIndex)
		{
		}
	}
}
