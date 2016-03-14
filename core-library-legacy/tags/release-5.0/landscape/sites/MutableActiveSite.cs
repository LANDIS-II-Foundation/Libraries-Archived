namespace Landis.Landscape
{
	public class MutableActiveSite
		: ActiveSite
	{
		public override bool IsMutable
		{
			get {
				return true;
			}
		}

		//---------------------------------------------------------------------

		internal MutableActiveSite(ILandscape       landscape,
		                           LocationAndIndex locationAndIndex)
			: base(landscape, locationAndIndex)
		{
		}
	}
}
