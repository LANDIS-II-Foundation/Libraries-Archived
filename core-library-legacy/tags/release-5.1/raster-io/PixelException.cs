using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.RasterIO
{
    /// <summary>
    /// An error that occurred at a particular pixel in a raster.
    /// </summary>
    public class PixelException
        : MultiLineException
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public PixelException(Location        location,
                              string          message,
                              params object[] mesgArgs)
            : base(string.Format("Error at pixel {0}", location),
                   string.Format(message, mesgArgs))
        {
        }
    }
}
