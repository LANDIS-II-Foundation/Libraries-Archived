using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Util
{
	/// <summary>
	/// Raster-related information.
	/// </summary>
	public static class Raster
	{
		private static Landis.Raster.ILibrary lib;

		//---------------------------------------------------------------------

		static Raster()
		{
			lib = new Landis.Raster.Erdas74.Library();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The raster library for reading and writing raster files.
		/// </summary>
		public static Landis.Raster.ILibrary Library
		{
			get {
				return lib;
			}
		}

		//---------------------------------------------------------------------

		public static Landis.Raster.IInputRaster<T> Open<T>(string path)
			where T : Landis.Raster.IPixel, new()
		{
			try {
				Landis.Raster.IInputRaster<T> raster = lib.Open<T>(path);
				return raster;
			}
			catch (System.IO.IOException exc) {
				string mesg = string.Format("Error opening map \"{0}\"", path);
				throw new MultiLineException(mesg, exc);
			}
		}
	}
}
