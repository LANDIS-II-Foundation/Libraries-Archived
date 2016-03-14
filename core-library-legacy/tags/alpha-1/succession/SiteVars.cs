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
		private static Landscape.ISiteVar<IReproduction> reproduction;

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

		public static Landscape.ISiteVar<IReproduction> Reproduction
		{
			get {
				return reproduction;
			}
		}

		//---------------------------------------------------------------------

		internal static void Initialize()
		{
			timeOfLast   = Model.Landscape.NewSiteVar<int>();
			shade        = Model.Landscape.NewSiteVar<byte>();
			reproduction = Model.Landscape.NewSiteVar<IReproduction>();
		}
	}
}
