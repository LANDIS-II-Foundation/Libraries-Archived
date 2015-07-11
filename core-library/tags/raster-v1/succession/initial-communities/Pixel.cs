using Landis.Raster;

namespace Landis.InitialCommunities
{
	public class Pixel
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

		public Pixel()
		{
			InitializeBands();
		}

		//---------------------------------------------------------------------

		public Pixel(byte band0)
		{
			InitializeBands();
			Band0 = band0;
		}
	}
}
