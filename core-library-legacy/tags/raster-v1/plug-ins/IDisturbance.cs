using Edu.Wisc.Forest.Flel.Util.PlugIns;

namespace Landis.PlugIns
{
	/// <summary>
	/// Interface between the model's main module and disturbance components.
	/// </summary>
	public interface IDisturbance
		: IPlugIn
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
		/// Initializes the component with a data file.
		/// </summary>
		/// <param name="dataFile">
		/// Path to the file with initialization data.
		/// </param>
		void Initialize(string dataFile);

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
