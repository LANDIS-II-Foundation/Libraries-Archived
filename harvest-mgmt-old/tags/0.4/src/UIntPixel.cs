// This file is part of the Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest/trunk/

using Landis.SpatialModeling;

namespace Landis.Library.Harvest
{
    public class UIntPixel : Pixel
    {
        public Band<uint> MapCode  = "The numeric code for each raster cell";

        public UIntPixel()
        {
            SetBands(MapCode);
        }
    }
}
