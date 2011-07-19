using Edu.Wisc.Forest.Flel.Grids;
using Landis.Ecoregions;

namespace Landis.Test.Main
{
    /// <summary>
    /// An input raster of ecoregion pixels with 0 rows and 0 columns.
    /// </summary>
	public class InputRaster0by0
		: RasterIO.InputRaster,
	      RasterIO.IInputRaster<Pixel>
	{
		public InputRaster0by0(string             path,
	                           RasterIO.IMetadata metadata)
			: base(path)
		{
		    this.Dimensions = new Dimensions(0, 0);
		    this.Metadata = metadata;
		}

		//---------------------------------------------------------------------

		public Pixel ReadPixel()
		{
		    IncrementPixelsRead();
		    throw new System.ApplicationException("Expected the IncrementPixelsRead method to throw exception");
		}
	}
}
