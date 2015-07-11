using GDAL;
using System.Collections.Generic;

namespace Landis.Raster.GDAL
{
	public class RasterFactory
		: IRasterFactory
	{
		private static Dictionary<string, string> extensionDriverDict;

		//---------------------------------------------------------------------

		static RasterFactory()
		{
			extensionDriverDict = new Dictionary<string, string>();
			extensionDriverDict.Add(".gis", "LAN");
			extensionDriverDict.Add(".jpg", "JPEG");
			extensionDriverDict.Add(".lan", "LAN");
			extensionDriverDict.Add(".tif", "GTiff");
			//  Not complete, but you get the idea.
		}

		//---------------------------------------------------------------------

		public RasterFactory()
		{
		}

		//---------------------------------------------------------------------

		public IInputRaster Open(string path)
		{
			Dataset dataset = Dataset.Open(path, Access.ReadOnly);
			if (null == dataset)
				return null;
			return new InputRaster(dataset);
		}

		//---------------------------------------------------------------------

		public IOutputRaster Create(string        path,
				                    Dimensions    dimensions,
				                    System.Type[] bandTypes)
		{
			//  Use extension of the file to determine the raster format.
			string fileExt = System.IO.Path.GetExtension(path);
			string driverName;
			if (! extensionDriverDict.TryGetValue(fileExt, out driverName))
				throw new System.ApplicationException("Unknown file extension");
			Driver driver = DriverManager.GetDriverByName(driverName);

			if (driver.HasCreate) {
				//  TODO:  GDAL requires (assumes?) that all the bands are the
				//	same type (don't why).  So we should make sure the array
				//  has all the same type.
				PixelType pixelType = PixelType.Get(bandTypes[0]);
				Dataset dataset = driver.Create(path,
				                                dimensions.Columns,
				                                dimensions.Rows,
											    bandTypes.Length,
												pixelType.GDALType,
												null); //  string[] parmList
				if (null == dataset)
					return null;
				return new OutputRaster(dataset);
			}

			else if (driver.HasCreateCopy) {
				//  TODO: Maybe create an in-memory dataset first, and then
				//  pass that dataset to the driver's CreateCopy method.
				return null;
			}

			else
				//  Format doesn't support writing.
				//  TODO:  Maybe throw an exception to indicate that?
				return null;
		}
	}
}
