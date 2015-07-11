using Gdal = GDAL;
using System.Diagnostics;

namespace Landis.Raster.GDAL
{
	public class OutputRaster
		: Raster, IOutputRaster
	{
		internal OutputRaster(Gdal.Dataset dataset)
			: base(dataset)
		{
		}

		//---------------------------------------------------------------------

		public IOutputBand<T> GetBand<T>(int bandIndex)
		{
			if (bandIndex < 1 || bandIndex > BandCount)
				throw new System.IndexOutOfRangeException();
			Gdal.RasterBand gdalBand = dataset.GetRasterBand(bandIndex);
			Debug.Assert( gdalBand != null );
			object band;
			if (typeof(T) == typeof(byte))
				band = new OutputBand.Byte(gdalBand, this);
			else if (typeof(T) == typeof(sbyte))
				band = new OutputBand.SByte(gdalBand, this);
			else if (typeof(T) == typeof(short))
				band = new OutputBand.Short(gdalBand, this);
			else if (typeof(T) == typeof(ushort))
				band = new OutputBand.UShort(gdalBand, this);
			else
				throw new System.ApplicationException("invalid band type");
			//  If the "if" statement above is constructed properly, then the
			//  cast in the statement below just never throw an exception.
			return (IOutputBand<T>) band;
		}
	}
}
