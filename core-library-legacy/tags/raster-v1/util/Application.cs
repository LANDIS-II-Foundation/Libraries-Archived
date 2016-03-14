using System;

namespace Landis
{
	/// <summary>
	/// Information about the Landis application.
	/// </summary>
	public static class Application
	{
		private static string baseDir;

		//---------------------------------------------------------------------

		static Application()
		{
			baseDir = AppDomain.CurrentDomain.BaseDirectory;
		}

		//---------------------------------------------------------------------

		public static string Directory
		{
			get {
				return baseDir;
			}
		}
	}
}
