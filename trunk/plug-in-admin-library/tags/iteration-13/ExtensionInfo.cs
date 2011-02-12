using System.Collections.Generic;

namespace Landis.PlugIns.Admin
{
	/// <summary>
	/// Information about an extension.
	/// </summary>
	public class ExtensionInfo
		: DatasetEntry
	{
		public string AssemblyPath;
		public List<string> LibraryPaths;

		//---------------------------------------------------------------------

		public ExtensionInfo()
			: base()
		{
			LibraryPaths = new List<string>();
		}
	}
}
