using Landis.Landscape;

namespace Landis.Output.Test
{
	public class OutputAsText
		: Landis.Output.IPlugIn
	{
		ILandscape landscape;
		private SiteVariable<int> foo;
		private int timestep;
		private int nextTimestep;

		//---------------------------------------------------------------------

		public OutputAsText()
		{
			foo = new SiteVariable<int>("foo");
		}

		//---------------------------------------------------------------------

		public void Initialize(Settings settings)
		{
			//MySettings mySettings = (MySettings) settings;
			timestep = 20; //mySettings.TimeStep;
			nextTimestep = timestep;
		}

		//---------------------------------------------------------------------

		public void AddSiteVars(ILandscape landscape)
		{
			landscape.Add(foo);
			this.landscape = landscape;
		}

		//---------------------------------------------------------------------

		public int NextTimestep {
			get {
				return nextTimestep;
			}
		}

		//---------------------------------------------------------------------

		public void Run(int timestep)
		{
			nextTimestep += this.timestep;

			foreach (Site site in landscape.AllSites) {
				System.Console.Write("  {0,6:##,##0}", foo[site]);
				if (site.Location.Column == landscape.Columns)
					System.Console.WriteLine();
			}
		}
	}
}
