using Landis.RasterIO;

namespace Landis.Test.Ecoregions
{
	public class Pixel16Bit
		: SingleBandPixel<ushort>
	{
		public Pixel16Bit()
			: base()
		{
		}

		//---------------------------------------------------------------------

		public Pixel16Bit(ushort band0)
			: base(band0)
		{
		}
	}
}
