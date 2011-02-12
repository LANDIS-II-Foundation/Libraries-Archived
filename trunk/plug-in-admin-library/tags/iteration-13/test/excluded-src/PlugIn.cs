namespace Landis.Test.PlugIns.Admin
{
	public class FooBarPlugIn
		: Landis.PlugIns.IOutput
	{
		/// <summary>
		/// The name that users refer to the plug-in by.
		/// </summary>
		public string Name
		{
			get {
				return "Foo Bar";
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The next timestep where the component should run.
		/// </summary>
		public int NextTimeToRun
		{
			get {
				return 0;
			}
		}

		//---------------------------------------------------------------------

		public FooBarPlugIn()
		{
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
		public void Initialize(string dataFile,
		                       int    startTime)
		{
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Runs the component for a particular timestep.
		/// </summary>
		/// <param name="currentTime">
		/// The current model timestep.
		/// </param>
		public void Run(int currentTime)
		{
		}
	}
}
