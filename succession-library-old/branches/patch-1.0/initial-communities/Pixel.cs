using Landis.RasterIO;

namespace Landis.InitialCommunities
{
	public class Pixel
		: SingleBandPixel<ushort>
	{
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
