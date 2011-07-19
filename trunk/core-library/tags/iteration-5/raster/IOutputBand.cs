namespace Landis.Raster
{
	public interface IOutputBand<T>
		: IRasterBand
	{
		IOutputRaster Raster
		{
			get;
		}

		//---------------------------------------------------------------------

		T this[int row,
		       int column]
		{
			set;
		}

		//---------------------------------------------------------------------

		T this[Landis.Landscape.Location location]
		{
			set;
		}


		//---------------------------------------------------------------------

		T this[Landis.Landscape.Site site]
		{
			set;
		}
	}
}
