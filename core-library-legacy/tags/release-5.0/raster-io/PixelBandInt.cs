namespace Landis.RasterIO
{
	/// <summary>
	/// A PixelBand with a signed 32-bit integer value.
	/// </summary>
	public class PixelBandInt
		: PixelBand<int, Edu.Wisc.Forest.Flel.Util.ByteMethods.Int>
	{
		/// <summary>
		/// Initializes a new instance with a default value of 0.
		/// </summary>
		public PixelBandInt()
			: base()
		{
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance with a specific value.
		/// </summary>
		public PixelBandInt(int initialValue)
			: base(initialValue)
		{
		}
	}
}
