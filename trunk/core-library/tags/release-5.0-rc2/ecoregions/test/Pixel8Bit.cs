using Landis.RasterIO;

namespace Landis.Test.Ecoregions
{
	public class Pixel8Bit
		: SingleBandPixel<byte>
	{
		public Pixel8Bit()
			: base()
		{
		}

		//---------------------------------------------------------------------

		public Pixel8Bit(byte band0)
			: base(band0)
		{
		}
	}
}
