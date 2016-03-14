using Edu.Wisc.Forest.Flel.Util.PlugIns;

namespace Landis.PlugIns
{
	/// <summary>
	/// A plug-in whose initialization has two phases.
	/// </summary>
	/// <remarks>
	/// The first phase of the initialization is done when the the plug-in's
	/// Initialize method is called (see IPlugInWithTimestep).
	/// 
	/// Typically, a plug-in has a 2-phase initialization when it needs to
	/// check for the presence of site variables of another plug-in which may
	/// not be loaded in a scenario.
	/// </remarks>
	public interface I2PhaseInitialization
	{
		/// <summary>
		/// Performs the 2nd phase of the plug-in's initialization.
		/// </summary>
		void InitializePhase2();
	}
}
