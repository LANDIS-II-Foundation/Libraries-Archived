namespace Landis
{
	/// <summary>
	/// A plug-in and its initialization file.
	/// </summary>
	public interface IPlugIn
	{
		/// <summary>
		/// Information about the plug-in.
		/// </summary>
		Edu.Wisc.Forest.Flel.Util.PlugIns.IInfo Info
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The path to the data file to initialize the plug-in with.
		/// </summary>
		string InitFile
		{
			get;
		}
	}
}
