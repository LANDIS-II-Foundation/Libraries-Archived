using Edu.Wisc.Forest.Flel.Util.PlugIns;

namespace Landis.PlugIns
{
	/// <summary>
	/// A plug-in that has to do some cleaning up when a scenario terminates.
	/// </summary>
	/// <remarks>
	/// The clean-up tasks may include releasing unmanaged resources,
	/// for example, closing a log file.  Another example of clean-up activies
	/// may be computing some statistics for the scenario run.
	/// </remarks>
	public interface ICleanUp
	{
		/// <summary>
		/// Performs the clean-up tasks for the plug-in.
		/// </summary>
		void CleanUp();
	}
}
