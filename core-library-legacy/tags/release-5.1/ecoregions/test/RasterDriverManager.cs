using Edu.Wisc.Forest.Flel.Grids;

using Landis.Ecoregions;
using Landis.RasterIO;

using Pixel = Landis.Ecoregions.Pixel;

using System;
using System.Collections.Generic;

namespace Landis.Test.Ecoregions
{
    /// <summary>
    /// A raster-driver manager that handles just array-backed input rasters
    /// of ecoregion pixels.
    /// </summary>
	public class RasterDriverManager
		: IDriverManager
	{
	    private Dictionary<string, object> dataArrays;

		//---------------------------------------------------------------------

		public RasterDriverManager()
		{
		    this.dataArrays = new Dictionary<string, object>();
		}

		//---------------------------------------------------------------------

	    public void SetData(string  path,
		                    byte[,] data)
		{
		    dataArrays[path] = data;
		}

		//---------------------------------------------------------------------

	    public void SetData(string    path,
		                    ushort[,] data)
		{
		    dataArrays[path] = data;
		}

		//---------------------------------------------------------------------

		public IInputRaster<TPixel> OpenRaster<TPixel>(string path)
 			where TPixel : IPixel, new()
	    {
	        if (typeof(TPixel) != typeof(Pixel))
	            throw new ApplicationException("Only valid pixel type is Landis.Ecoregions.Pixel");

	        object data;
	        if (! dataArrays.TryGetValue(path, out data))
	            throw new ApplicationException("Unknown path: " + path);
	        
	        ///byte[,] data = new byte[,]{ { 1, 2, 3}, {4, 5, 6} };
	        IInputRaster<Pixel> raster;
            if (data is byte[,])
                raster = new InputRaster<byte>(path,
                                               (byte[,]) data,
                                               Convert.ToUInt16);
            else
                raster = new InputRaster<ushort>(path,
                                                 (ushort[,]) data,
                                                 Convert.ToUInt16);
            return (IInputRaster<TPixel>) raster;
	    }

		//---------------------------------------------------------------------

 		public IOutputRaster<TPixel> CreateRaster<TPixel>(string     path,
		                                                  Dimensions dimensions,
		                                                  IMetadata  metadata)
 			where TPixel : IPixel, new()
		{
		    throw new NotSupportedException(GetType().FullName + " cannot write rasters");
		}
	}
}
