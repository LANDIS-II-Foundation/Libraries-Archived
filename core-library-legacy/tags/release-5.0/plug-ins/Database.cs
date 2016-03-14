using Edu.Wisc.Forest.Flel.Util;
using System.IO;

namespace Landis.PlugIns
{
	/// <summary>
	/// Information about the plug-ins database.
	/// </summary>
	public static class Database
	{
		private static string defaultPath;

		//---------------------------------------------------------------------

		static Database()
		{
			defaultPath = Path.Combine(Application.Directory,
			                           "plug-ins_database.txt"); //"extensions.landis");
		}

		//---------------------------------------------------------------------

		public static string DefaultPath
		{
			get {
				return defaultPath;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Identifier that indicates a Landis-II binary file contains a
		/// plug-in database.
		/// </summary>
		public const string BinaryFileIdentifier = "Plug-ins database";
	}
}
