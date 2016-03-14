namespace Landis
{
	/// <summary>
	/// A plug-in and its initialization file.
	/// </summary>
	public class PlugIn
		: IPlugIn
	{
		private Edu.Wisc.Forest.Flel.Util.PlugIns.IInfo info;
		private string initFile;

		//---------------------------------------------------------------------

		public Edu.Wisc.Forest.Flel.Util.PlugIns.IInfo Info
		{
			get {
				return info;
			}
		}

		//---------------------------------------------------------------------

		public string InitFile
		{
			get {
				return initFile;
			}
		}

		//---------------------------------------------------------------------

		public PlugIn(Edu.Wisc.Forest.Flel.Util.PlugIns.IInfo info,
		              string initFile)
		{
			this.info = info;
			this.initFile = initFile;
		}
	}
}
