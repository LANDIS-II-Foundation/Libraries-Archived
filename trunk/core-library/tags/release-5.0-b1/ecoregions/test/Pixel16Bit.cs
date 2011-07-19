using Landis.Raster;

namespace Landis.Test.Ecoregions
{
	public class Pixel16Bit
		: Raster.Pixel
	{
		private PixelBandUShort   band0;

		//---------------------------------------------------------------------

		public ushort Band0
		{
			get {
				return band0;
			}
			set {
				band0.Value = value;
			}
		}

		//---------------------------------------------------------------------

		private void InitializeBands()
		{
			band0 = new PixelBandUShort();
			SetBands(band0);
		}

		//---------------------------------------------------------------------

		public Pixel16Bit()
		{
			InitializeBands();
		}

		//---------------------------------------------------------------------

		public Pixel16Bit(ushort band0)
		{
			InitializeBands();
			Band0 = band0;
		}
	}
}
