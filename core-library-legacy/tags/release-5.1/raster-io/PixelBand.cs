using Edu.Wisc.Forest.Flel.Util;

namespace Landis.RasterIO
{
	/// <summary>
	/// An individual band value in a raster pixel.
	/// </summary>
	public class PixelBand<T, TByteMethods>
		: IPixelBandValue<T>
		where T: struct
		where TByteMethods: IByteMethods<T>, new()
	{
		private static IByteMethods<T> byteMethods;
		private T bandValue;

		//---------------------------------------------------------------------

		/// <summary>
		/// The band's value.
		/// </summary>
		public T Value
		{
			get {
				return bandValue;
			}

			set {
				bandValue = value;
			}
		}

		//---------------------------------------------------------------------

		static PixelBand()
		{
			byteMethods = new TByteMethods();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance with the default value (0) for the
		/// band's data type.
		/// </summary>
		public PixelBand()
		{
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance with a specific  value.
		/// </summary>
		public PixelBand(T initialValue)
		{
			bandValue = initialValue;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Converts a pixel band to its underlying data type.
		/// </summary>
		public static implicit operator T(PixelBand<T,TByteMethods> pixelBand)
		{
			return pixelBand.bandValue;
		}

		//---------------------------------------------------------------------

		public System.TypeCode TypeCode
		{
			get {
				return System.Type.GetTypeCode(typeof(T));
			}
		}

		//---------------------------------------------------------------------

		public byte[] GetBytes()
		{
			return byteMethods.ToBytes(bandValue);
		}

		//---------------------------------------------------------------------

		public void SetBytes(byte[] bytes,
							 int	startIndex)
		{
			try {
				bandValue = byteMethods.FromBytes(bytes, startIndex);
			}
			catch (System.ArgumentException exc) {
				//  Documentation for System.BitConverter.ToXXX says that
				//  ArgumentOutOfRangeException should be thrown for start
				//	indexes that are too big, but ArgumentException is being
				//	thrown instead.
				int size;
				switch (TypeCode) {
					case System.TypeCode.Byte:
					case System.TypeCode.SByte:
						size = 1;
						break;
					case System.TypeCode.Int16:
					case System.TypeCode.UInt16:
						size = 2;
						break;
					case System.TypeCode.Double:
						size = 8;
						break;
					default:  // Int32, UInt32, Single
						size = 4;
						break;
				}
				if (bytes != null && (startIndex + size - 1) > (bytes.Length - 1))
					throw new System.ArgumentOutOfRangeException(exc.Message);
				else
					throw;
			}
		}

		//---------------------------------------------------------------------

		public override string ToString()
		{
			return bandValue.ToString();
		}
	}
}
