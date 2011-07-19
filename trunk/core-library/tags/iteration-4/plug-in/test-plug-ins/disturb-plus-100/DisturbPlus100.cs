using Landis.Landscape;

namespace Landis.Disturbance.Test
{
	///<summary>
	/// A sample disturbance plug-in that adds 100 to "foo" site variable.
	/// </summary>
	public class DisturbPlus100
		: Landis.Disturbance.IPlugIn
	{
		ILandscape landscape;
		private SiteVariable<int> foo;
		private int timestep;
		private int nextTimestep;

		//---------------------------------------------------------------------

		///<summary>
		/// Create a new plug-in.
		///</summary>
		public DisturbPlus100()
		{
			foo = new SiteVariable<int>("foo");
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Initialize the plug-in (timestep is hard-wired at 20 years).
		///</summary>
		public void Initialize(/*Settings settings*/)
		{
			//MySettings mySettings = (MySettings) settings;
			timestep = 30; //mySettings.TimeStep;
			nextTimestep = timestep;
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Add the site variables used by plug-in to landscape.
		///</summary>
		public void AddSiteVars(ILandscape landscape)
		{
			landscape.Add(foo);
			this.landscape = landscape;
		}

		//---------------------------------------------------------------------

		///<summary>
		/// The next model timestep that the plug-in should be run.
		///</summary>
		public int NextTimestep {
			get {
				return nextTimestep;
			}
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Initialize the plug-in in preparation for running it at a
		/// particular timestep.
		///</summary>
		public void Initialize(int timestep)
		{
			//	Do nothing
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Run the plug-in at a particular timestep.
		///</summary>
		public void Run(int timestep)
		{
			nextTimestep += this.timestep;

			foreach (ActiveSite site in landscape) {
				if (site.Location.Column % 2 == 0)
					foo[site] += 100;
			}
		}
	}
}
