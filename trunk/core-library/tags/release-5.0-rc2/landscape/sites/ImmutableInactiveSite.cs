namespace Landis.Landscape
{
	public class ImmutableInactiveSite
		: InactiveSite
	{
		public override bool IsMutable
		{
			get {
				return false;
			}
		}

		//---------------------------------------------------------------------

		internal ImmutableInactiveSite(ILandscape landscape,
		               	               Location   location,
		               	               uint       dataIndex)
			: base(landscape, location, dataIndex)
		{
		}
	}
}
