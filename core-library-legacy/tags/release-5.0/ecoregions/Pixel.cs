using Landis.RasterIO;

namespace Landis.Ecoregions
{
	public class Pixel
		: SingleBandPixel<ushort>
	{
		//---------------------------------------------------------------------

		public Pixel()
			: base()
		{
		}

		//---------------------------------------------------------------------

		public Pixel(ushort band0)
			: base(band0)
		{
		}
	}
}
