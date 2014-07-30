//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Landis.SpatialModeling;

namespace Landis.Extension.Insects
{
    public class ShortPixel : Pixel
    {
        public Band<short> MapCode = "The numeric code for each raster cell";

        public ShortPixel()
        {
            SetBands(MapCode);
        }
    }
}
