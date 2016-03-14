namespace Landis.RasterIO
{
	/// <summary>
	/// A PixelBand with a signed 8-bit integer value.
	/// </summary>
	public class PixelBandSByte
		: PixelBand<sbyte, Edu.Wisc.Forest.Flel.Util.ByteMethods.SByte>
	{
		/// <summary>
		/// Initializes a new instance with a default value of 0.
		/// </summary>
		public PixelBandSByte()
			: base()
		{
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance with a specific value.
		/// </summary>
		public PixelBandSByte(sbyte initialValue)
			: base(initialValue)
		{
		}
	}
}
