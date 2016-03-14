using System;

namespace Landis.RasterIO
{
	/// <summary>
	/// Helper class for defining pixel classes.
	/// </summary>
	/// <example>
	/// The following example defines a pixel class with 3 bands.  The bands'
	/// datatypes are byte (unsigned 8-bit integer), short (signed 16-bit
	/// integer) and double (64-bit floating point).
	/// <code>
	///	public class MyPixel
	///		: Landis.Raster.Pixel
	///	{
	///		private PixelBandByte   band0;
	///		private PixelBandShort  band1;
	///		private PixelBandDouble band2;
	///
	///		public byte Band0
	///		{
	///			get {
	///				return band0;
	///			}
	///			set {
	///				band0.Value = value;
	///			}
	///		}
	///
	///		public short Band1
	///		{
	///			get {
	///				return band1;
	///			}
	///			set {
	///				band1.Value = value;
	///			}
	///		}
	///
	///		public double Band2
	///		{
	///			get {
	///				return band2;
	///			}
	///			set {
	///				band2.Value = value;
	///			}
	///		}
	///
	///		private void InitializeBands()
	///		{
	///			band0 = new PixelBandByte();
	///			band1 = new PixelBandShort();
	///			band2 = new PixelBandDouble();
	///			SetBands(band0, band1, band2);
	///		}
	///
	///		public MyPixel()
	///		{
	///			InitializeBands();
	///		}
	///
	///		public MyPixel(byte   band0,
	///		               short  band1,
	///		               double band2)
	///		{
	///			InitializeBands();
	///			Band0 = band0;
	///			Band1 = band1;
	///			Band2 = band2;
	///		}
	///	}
	/// </code>
	/// </example>
	public abstract class Pixel
		: IPixel
	{
		private IPixelBand[] bands;

		//---------------------------------------------------------------------

		public int BandCount
		{
			get {
				return bands.Length;
			}
		}

		//---------------------------------------------------------------------

		public IPixelBand this[int index]
		{
			get {
				return bands[index];
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Sets the pixel's bands.
		/// </summary>
		/// <param name="band0">
		/// The first band.
		/// </param>
		/// <param name="otherBands">
		/// Other optional bands.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// One or more of the bands is null.
		/// </exception>
		protected void SetBands(IPixelBand          band0,
		                        params IPixelBand[] otherBands)
		{
			if (band0 == null)
				throw new ArgumentNullException("band 0 is null");
			if (otherBands == null)
				//  The method was called as SetBands(band0, null)
				throw new ArgumentNullException("band 1 is null");
			int bandIndex = 0;
			foreach (IPixelBand band in otherBands) {
				bandIndex++;
				if (band == null)
					throw new ArgumentNullException(string.Format("band {0} is null",
					                                              bandIndex));
			}

			bands = new IPixelBand[1 + otherBands.Length];
			bands[0] = band0;
			otherBands.CopyTo(bands, 1);
		}
	}
}
