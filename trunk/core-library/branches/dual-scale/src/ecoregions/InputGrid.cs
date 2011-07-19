using Edu.Wisc.Forest.Flel.Util;
using Wisc.Flel.GeospatialModeling.Grids;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;
using Wisc.Flel.GeospatialModeling.RasterIO;

using Location = Wisc.Flel.GeospatialModeling.Landscapes.DualScale.Location;

namespace Landis.Ecoregions
{
    /// <summary>
    /// An input grid of ecoregion codes from an ecoregions map.
    /// </summary>
    public class InputGrid
        : Grid, IInputGrid<EcoregionCode>
    {
        private IInputRaster<Pixel> raster;
        private IDataset ecoregions;
        private Location pixelLocation;
        private bool disposed = false;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance using an input raster with ecoregion
        /// pixels.
        /// </summary>
        public InputGrid(IInputRaster<Pixel> raster,
                         IDataset            ecoregions)
            : base(raster.Dimensions)
        {
            this.raster = raster;
            this.ecoregions = ecoregions;

            // Initialize pixel location so the next call to RowMajor.Next
            // will return upper-left location (1,1)
            this.pixelLocation = new Location(1,0);
        }

        //---------------------------------------------------------------------

        public EcoregionCode ReadValue()
        {
            if (disposed)
                throw new System.InvalidOperationException("Object has been disposed.");
            Pixel pixel = raster.ReadPixel();
            pixelLocation = RowMajor.Next(pixelLocation, raster.Dimensions.Columns);
            ushort mapCode = pixel.Band0;
            IEcoregion ecoregion = ecoregions.Find(mapCode);
            if (ecoregion != null)
                return new EcoregionCode(mapCode, ecoregion.Active);

            string mesg = string.Format("Error at map site {0}", pixelLocation);
            string innerMesg = string.Format("Unknown map code for ecoregion: {0}", mapCode);
            throw new MultiLineException(mesg, innerMesg);
        }

        //---------------------------------------------------------------------

        public void Close()
        {
            Dispose();
        }

        //---------------------------------------------------------------------

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        //---------------------------------------------------------------------

        protected void Dispose(bool disposing)
        {
            if (!disposed) {
                if (disposing) {
                    //  Dispose of managed resources.
                    raster.Close();
                }
                //  Cleanup unmanaged resources (none).
                disposed = true;
            }
        }

        //---------------------------------------------------------------------

        ~InputGrid()
        {
            Dispose(false);
        }
    }
}
