
using Landis.Raster;

namespace Landis.Raster.Erdas74
{
    /// <summary>
    /// Factory for accessing ERDAS 7.4 version rasters (.gis and .lan)
    /// </summary>
    public class Library : ILibrary
    {
        // Constructor - does nothing but needed as I could not modify
        //   the interface (ILibrary) to have static methods
        public Library()
        {
            // nothing to do
        }

        /// <summary>
        ///         
        /// </summary>
        public IInputRaster<T> Open<T>(string path)
            where T : IPixel, new()
        {
            // open image file for reading
            ReadableImage image = new ReadableImage(path);

            // construct an InputRaster using that
            return new InputRaster<T>(image);
        }
        
        /// <summary>
        ///         
        /// </summary>
        public IOutputRaster<T> Create<T>(string path,
                                            Dimensions dimensions,
                                            IMetadata metadata)
            where T : IPixel, new()
        {
            // extract necessary parameters from pixel for image creation
            T desiredLayout = new T();
            
            int bandCount = desiredLayout.BandCount;

            System.TypeCode bandType = desiredLayout[0].TypeCode;
            
            // open image file for writing
            WritableImage image
              = new
                WritableImage(path,dimensions,bandCount,bandType,metadata);
                
            // construct an OutputRaster from that
            return new OutputRaster<T>(image);
        }
    }
}
