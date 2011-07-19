using Landis.Raster;

namespace Landis.Ecoregions
{
	public class Pixel
		: Raster.Pixel
	{
		private PixelBandShort band0;

		//---------------------------------------------------------------------

		public short Band0
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
			band0 = new PixelBandShort();
			SetBands(band0);
		}

		//---------------------------------------------------------------------

		public Pixel()
		{
			InitializeBands();
		}

		//---------------------------------------------------------------------

		public Pixel(short band0)
		{
			InitializeBands();
			Band0 = band0;
		}
	}
}
