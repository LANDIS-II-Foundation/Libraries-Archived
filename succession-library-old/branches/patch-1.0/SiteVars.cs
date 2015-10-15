//using Landis.Cohorts;
using Landis.Landscape;

namespace Landis.Succession
{
	/// <summary>
	/// Site variables related to succession.
	/// </summary>
	public static class SiteVars
	{
		private static Landscape.ISiteVar<int> timeOfLast;
		private static Landscape.ISiteVar<byte> shade;

		//---------------------------------------------------------------------

		public static Landscape.ISiteVar<int> TimeOfLast
		{
			get {
				return timeOfLast;
			}
		}

		//---------------------------------------------------------------------

		public static Landscape.ISiteVar<byte> Shade
		{
			get {
				return shade;
			}
		}

		//---------------------------------------------------------------------

		internal static void Initialize()
		{
			timeOfLast = Model.Landscape.NewSiteVar<int>();
			shade      = Model.Landscape.NewSiteVar<byte>();
		}
	}
}
