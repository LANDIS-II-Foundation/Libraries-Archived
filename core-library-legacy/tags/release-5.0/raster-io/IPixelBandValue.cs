namespace Landis.RasterIO
{
	/// <summary>
	/// An individual band value of data type T in a raster pixel.
	/// </summary>
	public interface IPixelBandValue<T>
		: IPixelBand
	{
		/// <summary>
		/// The band's value.
		/// </summary>
		T Value
		{
			get;
			set;
		}
	}
}
