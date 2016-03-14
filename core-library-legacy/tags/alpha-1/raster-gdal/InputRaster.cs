using Gdal = GDAL;
using System.Diagnostics;

namespace Landis.Raster.GDAL
{
	public class InputRaster
		: Raster, IInputRaster
	{
		internal InputRaster(Gdal.Dataset dataset)
			: base(dataset)
		{
		}

		//---------------------------------------------------------------------

		public IInputBand<T> GetBand<T>(int bandIndex)
		{
			if (bandIndex < 1 || bandIndex > BandCount)
				throw new System.IndexOutOfRangeException();
			Gdal.RasterBand gdalBand = dataset.GetRasterBand(bandIndex);
			Debug.Assert( gdalBand != null );
			object band;
			if (typeof(T) == typeof(byte))
				band = new InputBand.Byte(gdalBand, this);
			else if (typeof(T) == typeof(sbyte))
				band = new InputBand.SByte(gdalBand, this);
			else if (typeof(T) == typeof(short))
				band = new InputBand.Short(gdalBand, this);
			else if (typeof(T) == typeof(ushort))
				band = new InputBand.UShort(gdalBand, this);
			else
				throw new System.ApplicationException("invalid band type");
			//  If the "if" statement above is constructed properly, then the
			//  cast in the statement below just never throw an exception.
			return (IInputBand<T>) band;
		}
	}
}
