using Landis.Landscape;
using Landis.Util;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Landis.Test
{
	public static class Data
	{
		public static readonly string UtilBoolTestDir =
						Path.Combine(Landis.Test.Util.Data.Directory, "Bool");

		//---------------------------------------------------------------------

		public static List<Location> ReadLocations(string path)
		{
			List<Location> sites = new List<Location>();
			FileLineReader reader = new FileLineReader(path);
			string line;
			while ((line = reader.ReadLine()) != null) {
				string[] rowAndCol = line.Split(null);
				Assert.AreEqual(2, rowAndCol.Length);
				uint row = uint.Parse(rowAndCol[0]);
				uint col = uint.Parse(rowAndCol[1]);
				Location loc = new Location(row, col);
				sites.Add(loc);
			}
			reader.Close();
			return sites;
		}
	}
}
