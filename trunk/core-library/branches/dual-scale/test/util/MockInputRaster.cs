using Wisc.Flel.GeospatialModeling.Grids;
using Wisc.Flel.GeospatialModeling.RasterIO;

namespace Landis.Test.Util
{
    /// <summary>
    /// An array-backed input raster of single-band ushort pixels.
    /// </summary>
    public class MockInputRaster<TPixel>
        : InputRaster,
          IInputRaster<TPixel>
        where TPixel : SingleBandPixel<ushort>, new()
    {
        private ushort[,] data;
        private Location currentPixelLoc;
        private TPixel pixel;

        //---------------------------------------------------------------------

        public MockInputRaster(ushort[,] data)
            : base("" /* path */)
        {
            this.data = data;
            this.Dimensions = new Dimensions(data.GetLength(0),
                                             data.GetLength(1));

            //  Initialize current pixel location so that RowMajor.Next returns
            //  location (1,1).
            this.currentPixelLoc = new Location(1, 0);

            this.pixel = new TPixel();
        }

        //---------------------------------------------------------------------

        public Location CurrentPixelLocation
        {
            get {
                return currentPixelLoc;
            }
        }

        //---------------------------------------------------------------------

        public TPixel ReadPixel()
        {
            IncrementPixelsRead();
            currentPixelLoc = RowMajor.Next(currentPixelLoc, Dimensions.Columns);
            pixel.Band0 = data[currentPixelLoc.Row - 1,
                               currentPixelLoc.Column - 1];
            return pixel;
        }
    }
}
