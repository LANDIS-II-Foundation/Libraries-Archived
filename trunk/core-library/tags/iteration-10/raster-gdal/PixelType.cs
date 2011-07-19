using GDAL;

namespace Landis.Raster.GDAL
{
	/// <summary>
	/// Information about a pixel data type.
	/// </summary>
	public class PixelType
	{
		/// <summary>
		/// The GDAL data type associated with the pixel type.
		/// </summary>
		public readonly DataType GDALType;

		//---------------------------------------------------------------------

		/// <summary>
		/// The system type associated with the pixel type.
		/// </summary>
		public readonly System.Type SystemType;

		//---------------------------------------------------------------------

		/// <summary>
		/// The size (in bytes) of the associated system type.
		/// </summary>
		public readonly int SizeOf;

		//---------------------------------------------------------------------

		/// <summary>
		/// Brief description of the pixel type.
		/// </summary>
		public readonly string Description;

		//---------------------------------------------------------------------

		private static PixelType[] pixelTypes;

		//---------------------------------------------------------------------

		static PixelType()
		{
			pixelTypes = new PixelType[]{
								new PixelType(DataType.Byte,
				              				  typeof(byte),
											  sizeof(byte),
											  "8-bit integer (unsigned)"),
								new PixelType(DataType.UInt16,
											  typeof(ushort),
											  sizeof(ushort),
											  "16-bit integer (unsigned)"),
								new PixelType(DataType.Int16,
											  typeof(short),
											  sizeof(short),
											  "16-bit integer"),
								new PixelType(DataType.UInt32,
											  typeof(uint),
											  sizeof(uint),
											  "32-bit integer (unsigned)"),
								new PixelType(DataType.Int32,
											  typeof(int),
											  sizeof(int),
											  "32-bit integer"),
								new PixelType(DataType.Float32,
											  typeof(float),
											  sizeof(float),
											  "32-bit floating-point"),
								new PixelType(DataType.Float64,
											  typeof(double),
											  sizeof(double),
											  "64-bit floating-point") };
		}

		//---------------------------------------------------------------------

		private PixelType(DataType    gdalType,
		                  System.Type systemType,
		                  int         sizeOf,
		                  string      description)
		{
			this.GDALType = gdalType;
			this.SystemType = systemType;
			this.SizeOf = sizeOf;
			this.Description = description;
		}

		//---------------------------------------------------------------------

		public override string ToString()
		{
			return Description;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Get the pixel type associated with a particular GDAL data type.
		/// </summary>
		public static PixelType Get(DataType gdalType)
		{
			foreach (PixelType pixelType in pixelTypes)
				if (pixelType.GDALType == gdalType)
					return pixelType;
			return null;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Get the pixel type associated with a particular system type.
		/// </summary>
		public static PixelType Get(System.Type systemType)
		{
			foreach (PixelType pixelType in pixelTypes)
				if (pixelType.SystemType == systemType)
					return pixelType;
			return null;
		}
	}
}
