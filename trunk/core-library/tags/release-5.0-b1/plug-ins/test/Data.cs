using Edu.Wisc.Forest.Flel.Util;
using System.IO;

namespace Landis.Test.PlugIns
{
	public static class Data
	{
		private static NUnitInfo myNUnitInfo = new NUnitInfo();

		//---------------------------------------------------------------------

		public static readonly string Directory = myNUnitInfo.GetDataDir();

		public static string MakeInputPath(string filename)
		{
			return Path.Combine(Directory, filename);
		}

		//---------------------------------------------------------------------

		private static TextWriter writer = myNUnitInfo.GetTextWriter();

		public static TextWriter Output
		{
			get {
				return writer;
			}
		}
	}
}
