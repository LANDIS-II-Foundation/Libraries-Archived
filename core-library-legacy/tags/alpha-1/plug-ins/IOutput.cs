namespace Landis.PlugIns
{
	/// <summary>
	/// Interface between the model's main module and output components.
	/// </summary>
	public interface IOutput
		: IPlugInWithTimestep
	{
		/// <summary>
		/// Runs the component for a particular timestep.
		/// </summary>
		/// <param name="currentTime">
		/// The current model timestep.
		/// </param>
		void Run(int currentTime);
	}
}
