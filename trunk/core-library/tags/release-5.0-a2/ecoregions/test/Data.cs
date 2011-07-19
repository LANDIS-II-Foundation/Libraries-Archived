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

		private static TextWriter writer = myNUnitInfo.GetTextWriter();

		public static TextWriter Output
		{
			get {
				return writer;
			}
		}
	}
}
