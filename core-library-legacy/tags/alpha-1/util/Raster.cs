using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;

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

		//---------------------------------------------------------------------

 		public static Landis.Raster.IOutputRaster<T> Create<T>(string     path,
									                           Landis.Raster.Dimensions dimensions,
									                           Landis.Raster.IMetadata  metadata)
 			where T : Landis.Raster.IPixel, new()
		{
			try {
				string dir = System.IO.Path.GetDirectoryName(path);
				if (dir.Length > 0)
					Directory.EnsureExists(dir);
				Landis.Raster.IOutputRaster<T> raster = lib.Create<T>(path, dimensions, metadata);
				return raster;
			}
			catch (System.IO.IOException exc) {
				string mesg = string.Format("Error opening map \"{0}\"", path);
				throw new MultiLineException(mesg, exc);
			}
		}

		//---------------------------------------------------------------------

		public static MultiLineException PixelException(Location        location,
		                                                string          message,
		                                                params object[] mesgArgs)
		{
			string mesg = string.Format("Error at pixel {0}", location);
			string innerMesg = string.Format(message, mesgArgs);
			return new MultiLineException(mesg, innerMesg);
		}
	}
}
