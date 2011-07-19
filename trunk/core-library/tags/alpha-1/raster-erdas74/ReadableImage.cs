
using System.IO;
using Landis.Raster;

namespace Landis.Raster.Erdas74
{
    public class ReadableImage : Image
    {
        private int pixelsRead;  // internal counter used for pixelband location
        private FileStream file;  // file being read from
        private BinaryReader fileReader;  // filter that actually reads pixels
        
        /// <summary>
        /// Open an existing file
        /// </summary>
        public ReadableImage(string filename)
          : base(filename)
        {
            this.pixelsRead = 0;
            
            // open file for writing
            this.file = new FileStream(filename,FileMode.Open);
            this.fileReader = new BinaryReader(this.file);
        }
        
        /// <summary>
        /// Read a pixel from the file. Assumes pixels will be read
        /// by the caller consecutively from upper left to bottom
        /// right a row at a time
        /// </summary>
        public void ReadPixel(IPixel pixel)
        {
            int bandCount = pixel.BandCount;
            for (int bandNum = 0; bandNum < bandCount; bandNum++)
            {
                IPixelBand band = pixel[bandNum];

                // calc this pixelband's location in file
                int location = PixelBandLocation(pixelsRead,bandNum);

                // seek to correct pixel spot
                this.fileReader.BaseStream.Seek(location,SeekOrigin.Begin);

                byte[] bytes = this.fileReader.ReadBytes(this.BandSize);

                band.SetBytes(bytes,0);

            }
            
            pixelsRead++;
        }

        /// <summary>
        /// Close the associated file
        /// </summary>
        public void Close()
        {
            if (this.fileReader != null)
            {
                // close file
                this.fileReader.Close();
                // this.file automatically closed by prev line
                this.fileReader = null;
            }
            else if (this.file != null)
            {
                this.file.Close();
                this.file = null;
            }
        }
        
        ~ReadableImage()
        {
            Close();
        }
        
    }
}
