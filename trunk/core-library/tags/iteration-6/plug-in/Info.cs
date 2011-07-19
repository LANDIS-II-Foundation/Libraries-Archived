namespace Landis.PlugIn
{
	/// <summary>
	/// Information about a plug-in.
	/// </summary>
	public struct Info
	{
		/// <summary>
		/// The type that represents the plug-in.
		/// </summary>
		public string TypeName;

		//---------------------------------------------------------------------

		/// <summary>
		/// The path to the data file to initialize the plug-in with.
		/// </summary>
		public string InitFile;
	}
}
