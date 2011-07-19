using Landis.Raster;

namespace Landis.Test.Ecoregions
{
	public class Pixel8Bit
		: Raster.Pixel
	{
		private PixelBandByte band0;

		//---------------------------------------------------------------------

		public byte Band0
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
			band0 = new PixelBandByte();
			SetBands(band0);
		}

		//---------------------------------------------------------------------

		public Pixel8Bit()
		{
			InitializeBands();
		}

		//---------------------------------------------------------------------

		public Pixel8Bit(byte band0)
		{
			InitializeBands();
			Band0 = band0;
		}
	}
}
