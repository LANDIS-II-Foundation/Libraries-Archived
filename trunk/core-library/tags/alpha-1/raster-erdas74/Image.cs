
/// Erdas GIS/LAN raster format interface
///   Documented in the 1990 version of the ERDAS Field Guide
///   This file format can be found on the web. Barry has a PDF
///   document that describes it also. ErdasImageHeader defines
///   the format of the file header. The pixel layout format is
///   described below (see PixelBandLocation)
///<remark>
/// Note that this code as designed cannot support 4-bit encoded files.
/// It was decided that we would restrict users to 8 or 16 bit files.
///</remark>


using System;
using System.IO;
using Landis.Raster;

namespace Landis.Raster.Erdas74
{
    public class Image
    {
        // Instance variables
        private ImageHeader imageHeader;
        
        /// <summary>
        /// Create an Erdas image file given specs
        /// </summary>
        public Image(string filename,
                     Dimensions dimensions,
                     int bandCount,
                     System.TypeCode bandType,
                     IMetadata metadata)
        {
            // if filename does not end in .gis or .lan throw exception
            string extension = Path.GetExtension(filename).ToLower();
            if (!(extension.Equals(".gis")) && !(extension.Equals(".lan")))
                throw new ApplicationException("Erdas image must have either GIS or LAN as extension");
                
            // if dimensions are messed up throw exception
            if ((dimensions.Rows < 1) || (dimensions.Columns < 1))
                throw new ApplicationException("Erdas image given invalid dimensions");
                
            // if bandCount messed up throw exception
            if ((bandCount < 1) || (bandCount > 0xffff))
                throw new ApplicationException("Erdas image given invalid band count");
            
            // more bandCount checking
            if (extension.Equals(".gis"))
            {
                if (bandCount > 1)
                    throw new ApplicationException("Erdas GIS files cannot support multiband images");
            }
                
            this.imageHeader =
                new ImageHeader(dimensions, bandType, bandCount, metadata);

            this.imageHeader.Write(filename);            
        }
        
        /// <summary>
        /// Open an existing Erdas image file
        /// </summary>
        public Image(string filename)
        {
            // if filename does not end in .gis or .lan throw exception
            string extension = Path.GetExtension(filename).ToLower();
            if (!(extension.Equals(".gis")) && !(extension.Equals(".lan")))
                throw new ApplicationException("Erdas file must have either GIS or LAN as extension");
                
            this.imageHeader = new ImageHeader();

            this.imageHeader.Read(filename);
        }
        
        /// <summary>
        /// Find the file offset of the subpixel band of a particular pixel
        /// </summary>
        /// Pixels stored like this:
        ///   For row = 1 to numRows
        ///     For bands = 1 to numBands
        ///       For col = 1 to numCols
        ///          get sample (8 or 16 bit)
        
        protected int PixelBandLocation(int pixelNum, int bandNum)
        {
            int bandSize = BandSize;  // premature(?) optimization  :)  Hi Jimm
            
            int cols = Dimensions.Columns;
            
            int currRow = pixelNum / cols;
            
            int currCol = pixelNum % cols;

            // all pixels start beyond header
            int location = ImageHeader.Size;
            
            // increment for each complete row
            location += currRow * BandCount * cols * bandSize;
            
            // increment for each complete band in curr row
            location += bandNum * cols * bandSize;
            
            // increment for completed cols in band
            location += currCol * bandSize;
            
            return location;
        }

        /// <summary>
        /// row,col dimensions of the raster
        /// </summary>
        public Dimensions Dimensions
        {
            get { return imageHeader.Dimensions; }
        }

        /// <summary>
        /// An Erdas GIS or LAN file has a certain number of bands present
        /// </summary>
        public int BandCount
        {
            get { return imageHeader.BandCount; }
        }
        
        /// <summary>
        /// An Erdas GIS or LAN file has one type that applies equally to all bands
        /// </summary>
        public System.TypeCode BandType
        {
            get { return imageHeader.BandType; }
        }

        /// <summary>
        /// The size of each subpixel band in bytes
        /// </summary>
        public int BandSize
        {
            get { return imageHeader.BandSize; }
        }

        /// <summary>
        /// The metadata associated with this raster
        /// </summary>
        public IMetadata Metadata
        {
            get { return imageHeader.Metadata; }
        }

    }
}
