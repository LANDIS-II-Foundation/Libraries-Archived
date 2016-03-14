using Landis.PlugIns;

namespace Landis.Test.PlugIns
{
	//  A test plug-in.
	public class BarPlugIn
		: IOutput
	{
		public const string PlugInName = "Bar";

		public string Name
		{
			get {
				return PlugInName;
			}
		}

		public int NextTimeToRun
		{
			get {
				return 0;
			}
		}

		public void Initialize(string dataFile,
		                       int    startTime)
		{
		}

		public void Run(int currentTime)
		{
		}
	}
}
