using Landis.SpatialModeling;

namespace Landis.Library.Succession.DemographicSeeding
{
    public class IntPixel : Pixel
    {
        public Band<int> Band0 = "The integer value for each raster cell";

        public IntPixel()
        {
            SetBands(Band0);
        }
    }
}
