
using Landis.Raster;

namespace Landis.Test.Raster.Erdas74
{
    class Erdas74Pixel16 : IPixel
    {
        PixelBandUShort[] bands;
        
        public Erdas74Pixel16()
        {
            bands = new PixelBandUShort[1];
            bands[0] = new PixelBandUShort();
        }
        
        public Erdas74Pixel16(int bandCount)
        {
            //if (bandCount < 1) throw new ArgumentException();
            bands = new PixelBandUShort[bandCount];
            for (int i = 0; i < bands.Length; i++)
                bands[i] = new PixelBandUShort();
        }
        
        public int BandCount
        {
            get {
                if (bands == null)
                    return 0;
                return bands.Length;
            }
        }

        public IPixelBand this[int index]
        {
            get {
                if (bands == null)
                    return null;
                return bands[index];
            }
        }
    }
}
