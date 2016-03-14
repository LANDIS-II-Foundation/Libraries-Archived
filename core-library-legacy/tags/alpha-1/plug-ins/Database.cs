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
			                           "plug-ins_database.txt");
		}

		//---------------------------------------------------------------------

		public static string DefaultPath
		{
			get {
				return defaultPath;
			}
		}
	}
}
