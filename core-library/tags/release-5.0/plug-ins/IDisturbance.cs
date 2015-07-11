namespace Landis.PlugIns
{
	/// <summary>
	/// Interface between the model's main module and disturbance components.
	/// </summary>
	public interface IDisturbance
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
