using Landis.Landscape;
using System.Collections.Generic;

namespace Landis.Succession.Test
{
	public class SuccessionA
		: Landis.Succession.IPlugIn
	{
		private SiteVariable<int> foo;
		private int timestep;
		private int nextTimestep;

		//---------------------------------------------------------------------

		public SuccessionA()
		{
			foo = new SiteVariable<int>("foo");
		}

		//---------------------------------------------------------------------

		public void Initialize(Settings settings)
		{
			timestep = settings.TimeStep;
			nextTimestep = timestep;
		}

		//---------------------------------------------------------------------

		public void AddSiteVars(ILandscape landscape)
		{
			landscape.Add(foo);

			foreach (ActiveSite site in landscape)
				foo[site] = (int) (site.DataIndex + 1) * 1000;
		}

		//---------------------------------------------------------------------

		public int NextTimestep {
			get {
				return nextTimestep;
			}
		}

		//---------------------------------------------------------------------

		public void Initialize(int timestep)
		{
			//	Do nothing.
		}

		//---------------------------------------------------------------------

		public void Run(int 					timestep,
		                IEnumerable<ActiveSite> sites)
		{
			nextTimestep += this.timestep;

			foreach (ActiveSite site in sites)
				foo[site] += this.timestep;
		}
	}
}
