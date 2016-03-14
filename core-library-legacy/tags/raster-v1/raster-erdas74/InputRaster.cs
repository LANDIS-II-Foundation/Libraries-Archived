using Landis.Raster;

namespace Landis.Raster.Erdas74
{
    /// <summary>
    ///  An InputRaster is a parameterized class that is used for
    ///  reading pixels in from image files. It makes the following
    ///  assumptions:
    ///  - the image data will be read starting at the upper left
    ///    of the image a row at a time
    ///  Trying to read more pixels than are present in the raster
    //   throws an exception. Reading too few pixels is not checked.
    /// </summary>
    public class InputRaster<T> : IInputRaster<T>
        where T : IPixel, new()
    {
        private bool disposed; // track whether resources have been released
        private ErdasImageFile image; // the underlying raster image
        private T pixel;  // a pixel: used for xfering data
        
        /// <summary>
        /// Constructor - takes an already constructed ERDAS image file
        /// </summary>
        public InputRaster(ErdasImageFile image)
        {
            this.disposed = false;
            this.image = image;
            this.pixel = new T();

            // make sure we've got valid input
            if (this.image == null)
                throw new System.ApplicationException("InputRaster constructor passed null image");
                
            // check if trying to read a WriteOnly file            
            if (this.image.Mode == RWFlag.Write)
                throw new System.ApplicationException("InputRaster can't be created for WriteOnly image");

            // pixel vs. image bandcount mismatch?            
            int pixelBandCount = this.pixel.BandCount;
            if (pixelBandCount != image.BandCount)
                throw new System.ApplicationException("InputRaster band count mismatch");

            // check bandtype compatibilities                
            for (int i = 0; i < pixelBandCount; i++)
            {
                IPixelBand band = this.pixel[i];
                
                if (image.BandType == System.TypeCode.Byte)
                {
                    if (band.TypeCode != System.TypeCode.Byte)
                        throw new System.ApplicationException("InputRaster band type mismatch");
                }
                else if (image.BandType == System.TypeCode.UInt16)
                {
                    if (band.TypeCode != System.TypeCode.UInt16)
                        throw new System.ApplicationException("InputRaster band type mismatch");
                }
                else
                    throw new System.ApplicationException("InputRaster - Unsupported band type");
                //  shouldn't really ever get to this exception
                //    ErdasImageFile construction code should have
                //    thrown an exception earlier
            }
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        private System.ObjectDisposedException CreateObjectDisposedException()
        {
            return new System.ObjectDisposedException(GetType().FullName);
        }

        /// <summary>
        /// Read a pixel from the InputRaster
        /// Must not call more than rows*cols times
        /// </summary>
        public T ReadPixel()
        {
            if (disposed)
                throw CreateObjectDisposedException();
            
            this.image.ReadPixel(this.pixel);
            
            return this.pixel;
        }
        
        /// <summary>
        /// Get the Dimensions of the InputRaster
        /// </summary>
        public Dimensions Dimensions
        {
            get {
                if (disposed)
                    throw CreateObjectDisposedException();
                    
                return this.image.Dimensions;
            }
        }
        
        /// <summary>
        /// Get the metadata for the raster.
        /// </summary>
        public IMetadata Metadata
        {
            get {
                if (disposed)
                    throw CreateObjectDisposedException();
                    
                return this.image.Metadata;
            }
        }

        /// <summary>
        /// Closes the raster, releasing any unmanaged resources.
        /// </summary>
        public void Close()
        {
            if (disposed)
                throw CreateObjectDisposedException();
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                image.Close();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Destructor - close resources if needed
        /// </summary>
        ~InputRaster()
        {
            Dispose(false);
        }
    }
}
