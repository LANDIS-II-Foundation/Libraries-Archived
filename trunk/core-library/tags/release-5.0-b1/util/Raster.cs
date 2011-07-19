using Edu.Wisc.Forest.Flel.Util;
using PlugIns = Edu.Wisc.Forest.Flel.Util.PlugIns;
using Landis.Landscape;

namespace Landis.Util
{
	/// <summary>
	/// Raster-related information.
	/// </summary>
	public static class Raster
	{
		private static Landis.Raster.IDriver driver;

		//---------------------------------------------------------------------

		static Raster()
		{
			PlugIns.Info plugInInfo = new PlugIns.Info("Erdas 7.4 Raster Driver",
			                                           typeof(Landis.Raster.IDriver),
			                                           "Landis.Raster.Drivers.Erdas74.Driver,Landis.Raster.Drivers.Erdas74");
			driver = PlugIns.Loader.Load<Landis.Raster.IDriver>(plugInInfo);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The raster library for reading and writing raster files.
		/// </summary>
		public static Landis.Raster.IDriver Driver
		{
			get {
				return driver;
			}
		}

		//---------------------------------------------------------------------

		public static Landis.Raster.IInputRaster<T> Open<T>(string path)
			where T : Landis.Raster.IPixel, new()
		{
			try {
				Landis.Raster.IInputRaster<T> raster = driver.Open<T>(path);
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
				Landis.Raster.IOutputRaster<T> raster = driver.Create<T>(path, dimensions, metadata);
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
