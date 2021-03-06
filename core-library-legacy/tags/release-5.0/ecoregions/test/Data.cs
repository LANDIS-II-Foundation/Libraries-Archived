using Edu.Wisc.Forest.Flel.Util;
using System.IO;

namespace Landis.Test.Ecoregions
{
	public static class Data
	{
		private static NUnitInfo myNUnitInfo = new NUnitInfo();

		//---------------------------------------------------------------------

		public static readonly string Directory = myNUnitInfo.GetDataDir();

		//---------------------------------------------------------------------

		public static readonly string OutputDir = myNUnitInfo.GetOutDir(true);

		public static string MakeOutputPath(string filename)
		{
			return Path.Combine(OutputDir, filename);
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
