namespace Landis.PlugIn
{
	/// <summary>
	/// A component that plugs into the core framework.
	/// </summary>
	public interface IPlugIn
	{
		/// <summary>
		/// Initializes a plug-in that's been loaded.
		/// </summary>
		/// <param name="initDataFile">
		/// Path to a file with the data to initialize the plug-in.
		/// </param>
		void Initialize(string initDataFile);
	}
}
