using Gdal = GDAL;

namespace Landis.Raster.GDAL
{
	public class RasterBand<T>
		: Landis.Raster.IRasterBand
		where T: struct
	{
		public readonly int BlockXSize;
		public readonly int BlockYSize;
		public readonly int BufferSize;

		protected Gdal.RasterBand gdalBand;
		protected byte[] buffer;
		protected BlockLocation currentBlockLoc;

		private PixelType pixelType;
		private Landis.Raster.Dimensions dimensions;

		//---------------------------------------------------------------------

		public PixelType PixelType
		{
			get {
				return pixelType;
			}
		}

		//---------------------------------------------------------------------

		public Landis.Raster.Dimensions Dimensions
		{
			get {
				return dimensions;
			}
		}

		//---------------------------------------------------------------------

		public RasterBand(Gdal.RasterBand band)
		{
			this.gdalBand = band;
			band.GetBlockSize(out BlockXSize, out BlockYSize);
			pixelType = PixelType.Get(typeof(T));
			BufferSize = BlockXSize * BlockYSize * pixelType.SizeOf;
			buffer = new byte[BufferSize];
			currentBlockLoc = new BlockLocation(-1, -1);

			int rows = band.GetYSize();
			int columns = band.GetXSize();
			dimensions = new Landis.Raster.Dimensions(rows, columns);
		}

		//---------------------------------------------------------------------

		public BlockLocation GetBlockLocation(int row,
			                                  int column,
			                                  out int index)
		{
			int xOffset = column / BlockXSize;
			int yOffset = row / BlockYSize;
			int xOffsetInBlock = column % BlockXSize;
			int yOffsetInBlock = row % BlockYSize;
			index = (yOffsetInBlock * BlockXSize) + xOffsetInBlock;
			return new BlockLocation(xOffset, yOffset);
		}
	}
}
