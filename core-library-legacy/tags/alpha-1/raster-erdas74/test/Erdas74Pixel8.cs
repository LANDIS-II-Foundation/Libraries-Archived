
using Landis.Raster;

namespace Landis.Test.Raster.Erdas74
{
    class Erdas74Pixel8 : IPixel
    {
        PixelBandByte[] bands;
        
        public Erdas74Pixel8()
        {
            bands = new PixelBandByte[1];
            bands[0] = new PixelBandByte();
        }
        
        public Erdas74Pixel8(int bandCount)
        {
            // if (bandCount < 1) throw new ArgumentException();
            bands = new PixelBandByte[bandCount];
            for (int i = 0; i < bands.Length; i++)
                bands[i] = new PixelBandByte();
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
