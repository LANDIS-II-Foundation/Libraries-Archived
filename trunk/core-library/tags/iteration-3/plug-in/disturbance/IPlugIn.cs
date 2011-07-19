using Landis.Landscape;

namespace Landis.Disturbance
{
	/// <summary>
	/// A disturbance component.
	/// </summary>
	public interface IPlugIn
	{
		/// <summary>
		/// Initialize the plug-in.
		/// </summary>
		void Initialize(/* Settings settings */);

		//---------------------------------------------------------------------

		/// <summary>
		/// Add the site variables the plug-in uses to the shared landscape.
		/// </summary>
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
		/// <param name="timestep">The current model timestep.</param>
		void Run(int timestep);
	}
}
