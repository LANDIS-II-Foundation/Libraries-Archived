
using System.IO;
using Landis.Raster;

namespace Landis.Raster.Erdas74
{
    public class WritableImage : Image
    {
        // track the count of pixels written to file for seeking
        private int pixelsWritten;
        
        // the file that will contain the data
        private FileStream file;
        
        // a filter on the file to write only in binary
        private BinaryWriter fileWriter;
        
        /// <summary>
        /// Open a writer on an existing file
        /// </summary>
        public WritableImage(string filename)
         : base(filename)
        {
            this.pixelsWritten = 0;
            
            // open file for writing
            this.file = new FileStream(filename,FileMode.Open);
            this.fileWriter = new BinaryWriter(this.file);
        }
        
        /// <summary>
        /// Open a writer on a new file
        /// </summary>
        public WritableImage(string filename,
                             Dimensions dimensions,
                             int bandCount,
                             System.TypeCode bandType,
                             IMetadata metadata)
          : base(filename, dimensions, bandCount, bandType, metadata)
        {
            this.pixelsWritten = 0;
            
            // open file for writing
            this.file = new FileStream(filename,FileMode.Open);
            this.fileWriter = new BinaryWriter(this.file);
           
        }
        
        /// <summary>
        /// Write a pixel to the file. Assumes pixels will be written
        /// by the caller consecutively from upper left to bottom
        /// right a row at a time
        /// </summary>
        public void WritePixel(IPixel pixel)
        {
            int bandCount = pixel.BandCount;
            for (int bandNum = 0; bandNum < bandCount; bandNum++)
            {
                IPixelBand band = pixel[bandNum];

                // calc this pixelband's location in file
                int location =
                    PixelBandLocation(this.pixelsWritten,bandNum);

                this.fileWriter.Seek(location,SeekOrigin.Begin);

                this.fileWriter.Write(band.GetBytes());
            }

            this.pixelsWritten++;
        }
        
        /// <summary>
        /// Close the associated file
        /// </summary>
        public void Close()
        {
            if (this.fileWriter != null)
            {
                // close file
                this.fileWriter.Close();
                // this.file automatically closed by prev line
                this.fileWriter = null;
            }
            else if (this.file != null)
            {
                this.file.Close();
                this.file = null;
            }
        }
        
        
        ~WritableImage()
        {
            Close();
        }
        
        /// <summary>
        /// Track the count of the number of pixels written to file.
        /// </summary>
        /// <remark>
        /// Used for error checking by clients.
        /// </remark>
        public int PixelsWritten
        {
            get { return pixelsWritten; }
        }

    }
}
