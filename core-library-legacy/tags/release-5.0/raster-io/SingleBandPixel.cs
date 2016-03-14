//  Author: Jimm Domingo

using System;

namespace Landis.RasterIO
{
	/// <summary>
	/// A helper class for defining pixel classes with just one band.  The
	/// band's data type is T, which must be one of these base numeric types:
	/// byte, sbyte, short, ushort, int, uint, float, or double.
	/// </summary>
	public class SingleBandPixel<T>
		: Pixel
		where T : struct
	{
		private IPixelBandValue<T> band0;

		//---------------------------------------------------------------------

		public T Band0
		{
			get {
				return band0.Value;
			}
			set {
				band0.Value = value;
			}
		}

		//---------------------------------------------------------------------

		private void InitializeBands()
		{
			band0 = NewPixelBand(typeof(T)) as IPixelBandValue<T>;
			SetBands(band0);
		}

		//---------------------------------------------------------------------

		private IPixelBand NewPixelBand(Type bandType)
		{
			switch (Type.GetTypeCode(bandType)) {
				case TypeCode.Byte:
					return new PixelBandByte();

				case TypeCode.SByte:
					return new PixelBandSByte();

				case TypeCode.Int16:
					return new PixelBandShort();

				case TypeCode.UInt16:
					return new PixelBandUShort();

				case TypeCode.Int32:
					return new PixelBandInt();

				case TypeCode.UInt32:
					return new PixelBandUInt();

				case TypeCode.Single:
					return new PixelBandFloat();

				case TypeCode.Double:
					return new PixelBandDouble();

				default:
					throw new ArgumentException(string.Format("SingleBandPixel does not support {0} for band type",
					                                          bandType.FullName));
			}
		}

		//---------------------------------------------------------------------

		public SingleBandPixel()
		{
			InitializeBands();
		}

		//---------------------------------------------------------------------

		public SingleBandPixel(T band0)
		{
			InitializeBands();
			Band0 = band0;
		}
	}
}
