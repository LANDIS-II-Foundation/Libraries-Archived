using Edu.Wisc.Forest.Flel.Grids;

using Landis.Ecoregions;
using Landis.Landscape;

namespace Landis.Test.Ecoregions
{
    /// <summary>
    /// An array-backed input raster of ecoregion pixels.
    /// </summary>
	public class InputRaster<TValue>
		: RasterIO.InputRaster,
	      RasterIO.IInputRaster<Pixel>
	    where TValue : struct
	{
	    private TValue[,] data;
	    private Location currentPixelLoc;
	    private Pixel pixel;
	    public delegate ushort ConvertToUShort<T>(T value);
	    private ConvertToUShort<TValue> convertToUShort;

		//---------------------------------------------------------------------

		public InputRaster(string                  path,
		                   TValue[,]               data,
		                   ConvertToUShort<TValue> convertToUShort)
			: base(path)
		{
		    if (! (typeof(TValue) == typeof(byte) || typeof(TValue) == typeof(ushort)))
		        throw new System.ApplicationException("Type parameter TValue is not byte or ushort");
		    this.data = data;
		    this.Dimensions = new Dimensions(data.GetLength(0),
		                                     data.GetLength(1));

		    //  Initialize current pixel location so that RowMajor.Next returns
		    //  location (1,1).
		    this.currentPixelLoc = new Location(1, 0);

		    this.pixel = new Pixel();
		    this.convertToUShort = convertToUShort;
		}

		//---------------------------------------------------------------------

		public Pixel ReadPixel()
		{
		    IncrementPixelsRead();
		    currentPixelLoc = RowMajor.Next(currentPixelLoc, (uint) Dimensions.Columns);
		    pixel.Band0 = convertToUShort(data[currentPixelLoc.Row - 1,
		                                       currentPixelLoc.Column - 1]);
		    return pixel;
		}
	}
}
