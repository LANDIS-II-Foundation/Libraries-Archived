using Edu.Wisc.Forest.Flel.Util.PlugIns;

namespace Landis.PlugIns
{
	/// <summary>
	/// A plug-in with a timestep.
	/// </summary>
	public interface IPlugInWithTimestep
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
		/// <param name="startTime">
		/// Initial timestep (year): the timestep that will be passed to the
		/// first call to the component's Run method.
		/// </param>
		void Initialize(string dataFile,
		                int    startTime);
	}
}
