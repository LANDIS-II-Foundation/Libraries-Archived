using Landis.Raster;

namespace Landis.Raster.Erdas74
{
    /// <summary>
    ///  An OutputRaster is a parameterized class that is used for
    ///  writing pixels to image files. It makes the following
    ///  assumptions:
    ///  - the image data will be written starting at the upper left
    ///    of the image a row at a time
    ///  Trying to write more pixels than are defined by the raster
    //   throws an exception. Writing too few pixels is not checked.
    /// </summary>
    public class OutputRaster<T> : IOutputRaster<T>
        where T : IPixel, new()
    {
        private ErdasImageFile image;  // the underlying image
        private bool disposed = false; // track whether resources have been released
        
        /// <summary>
        /// Constructor - takes an already constructed ERDAS image file
        /// </summary>
        public OutputRaster(ErdasImageFile image)
        {
            this.image = image;
            
            // make sure we got valid image
            if (this.image == null)
                throw new System.ApplicationException("OutputRaster constructor passed null image");
            
            // make sure image is not readonly
            if (this.image.Mode == RWFlag.Read)
                throw new System.ApplicationException("OutputRaster cannot be constructed on ReadOnly image");

            // Begin test bandtype compatibilities
            
            T desiredLayout = new T();
            
            int bandCount = desiredLayout.BandCount;

            System.TypeCode bandType = desiredLayout[0].TypeCode;
            
            // check band 0
            if (bandType != image.BandType)
                throw new System.ApplicationException("OutputRaster band type mismatch");

            // check bands 1 to n-1            
            for (int i = 1; i < bandCount; i++)
            {
                IPixelBand band = desiredLayout[i];
                
                if (band.TypeCode != bandType)
                    throw new System.ApplicationException("OutputRasters with mixed band types not supported");
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
        /// Write a pixel to the OutputRaster
        /// Must not call more than rows*cols times
        /// </summary>
        public void WritePixel(T pixel)
        {
            if (disposed)
                throw CreateObjectDisposedException();
            
            image.WritePixel(pixel);
        }
        
        /// <summary>
        /// Get the Dimensions of the OutputRaster
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
            get { return this.image.Metadata; }
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
        ~OutputRaster()
        {
            Dispose(false);
        }
    }
}
