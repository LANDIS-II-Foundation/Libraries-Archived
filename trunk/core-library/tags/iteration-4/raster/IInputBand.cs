namespace Landis.Raster
{
	public interface IInputBand<T>
		: IRasterBand
	{
		IInputRaster Raster
		{
			get;
		}

		//---------------------------------------------------------------------

		T this[int row,
		       int column]
		{
			get;
		}

		//---------------------------------------------------------------------

		T this[Landis.Landscape.Location location]
		{
			get;
		}


		//---------------------------------------------------------------------

		T this[Landis.Landscape.Site site]
		{
			get;
		}
	}
}
