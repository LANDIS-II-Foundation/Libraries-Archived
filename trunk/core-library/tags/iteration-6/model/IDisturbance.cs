namespace Landis
{
	/// <summary>
	/// Interface between the model's main module and disturbance components.
	/// </summary>
	public interface IDisturbance
	{
		/// <summary>
		/// The next timestep where the component should run.
		/// </summary>
		int NextTimeToRun
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Runs the component for a particular timestep.
		/// </summary>
		/// <param name="currentTime">
		/// The current model timestep.
		/// </param>
		void Run(int currentTime);
	}
}
