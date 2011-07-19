using Landis.Landscape;
using Landis.Species;
using System.Collections.Generic;

namespace Landis
{
	/// <summary>
	/// Core succession component.
	/// </summary>
	public class Succession
	{
		private ISuccession plugIn;
		private int nextTimeToRun;

		//---------------------------------------------------------------------

		public int Timestep
		{
			get {
				return plugIn.Timestep;
			}
		}

		//---------------------------------------------------------------------

		public int NextTimeToRun
		{
			get {
				return nextTimeToRun;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance using a particular succession plug-in.
		/// </summary>
		/// <param name="plugIn">
		/// The succession plug-in to use.
		/// </param>
		public Succession(ISuccession plugIn)
		{
			this.plugIn = plugIn;
			this.nextTimeToRun = plugIn.Timestep;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Updates the component's NextTimeToRun to the next timestep.
		/// </summary>
		public void UpdateNextTimeToRun()
		{
			nextTimeToRun += Timestep;
		}
	
		//---------------------------------------------------------------------

		/// <summary>
		/// Advances the age of all the cohorts at certain specified sites.
		/// </summary>
		/// <param name="sites">
		/// The sites whose cohorts are to be aged.
		/// </param>
		/// <param name="currentTime">
		/// The current model timestep.
		/// </param>
		public void AgeCohorts(IEnumerable<ActiveSite> sites,
		                       int                     currentTime)
		{
			foreach (ActiveSite site in sites) {
				int deltaTime = currentTime - Model.SiteVars.TimeLastSuccession[site];
				plugIn.AgeCohorts(site, deltaTime);
				Model.SiteVars.TimeLastSuccession[site] = currentTime;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Computes the shade at certain specified sites.
		/// </summary>
		/// <param name="sites">
		/// The sites where shade is to be computed.
		/// </param>
		public void ComputeShade(IEnumerable<ActiveSite> sites)
		{
			foreach (ActiveSite site in sites)
				Model.SiteVars.Shade[site] = plugIn.ComputeShade(site);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Does plant reproduction at certain specified sites.
		/// </summary>
		/// <param name="sites">
		/// The sites where plant reproduction should be done.
		/// </param>
		public void ReproducePlants(IEnumerable<ActiveSite> sites)
		{
			foreach (ActiveSite site in sites)
				Model.SiteVars.Reproduction[site].Do(site);
		}
	}
}
