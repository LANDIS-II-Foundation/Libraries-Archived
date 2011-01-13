using Landis.Landscape;

namespace Landis.Succession
{
	/// <summary>
	/// Site variables related to succession.
	/// </summary>
	public static class SiteVars
	{
		private static ISiteVar<int> timeOfLast;
		private static ISiteVar<byte> shade;
		private static ISiteVar<bool> disturbed;

		//---------------------------------------------------------------------

		public static ISiteVar<int> TimeOfLast
		{
			get {
				return timeOfLast;
			}
		}

		//---------------------------------------------------------------------

		public static ISiteVar<byte> Shade
		{
			get {
				return shade;
			}
		}

		//-----------------------------------------------------------------

		public static ISiteVar<bool> Disturbed
		{
			get {
				return disturbed;
			}
		}

		//---------------------------------------------------------------------

		internal static void Initialize()
		{
			timeOfLast = Model.Core.Landscape.NewSiteVar<int>();
			shade      = Model.Core.Landscape.NewSiteVar<byte>();
			disturbed  = Model.Core.Landscape.NewSiteVar<bool>();
		}
	}
}
