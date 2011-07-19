using Landis.Landscape;
using System.Collections.Generic;

namespace Landis.Succession
{
	/// <summary>
	/// A succession component.
	/// </summary>
	public interface IPlugIn
	{
		/// <summary>
		/// Initialize the plug-in.
		/// </summary>
		void Initialize(Settings settings);

		//---------------------------------------------------------------------

		/// <summary>
		/// Add the site variables used by the plug-in to the shared landscape.
		/// </summary>
		/// <param name="landscape">The shared landscape.</param>
		void AddSiteVars(ILandscape landscape);

		//---------------------------------------------------------------------

		/// <summary>
		/// The next timestep at which the plug-in should be run.
		/// </summary>
		int NextTimestep {
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initialize the plug-in for a particular timestep.
		/// </summary>
		/// <param name="timestep">The current model timestep.</param>
		void Initialize(int timestep);

		//---------------------------------------------------------------------

		/// <summary>
		/// Run the plug-in for a particular timestep.
		/// </summary>
		/// <param name="timestep">The current timestep.</param>
		/// <param name="sites">Collection of sites where the plug-in should
		/// run.</param>
		void Run(int 					 timestep,
		         IEnumerable<ActiveSite> sites);
	}
}
