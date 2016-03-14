
/// Erdas GIS/LAN raster interface
///   Documented in the 1990 version of the ERDAS Field Guide
///   This file format can be found on the web. Baqrry has a PDF
///   documents that describes it.

// Notes
//   Break into two classes?
//   On image read determine if ENDIAN is messed up and handle?

using Landis.Raster;
using System.IO;

namespace Landis.Raster.Erdas74
{
    public class ErdasImageFile
    {
        // ERDAS GIS/LAN file header size
        const int HeaderSize = 128;
        
        // Metadata ID's
        public const string RASTER_ULX  = "Raster Upper Left X";
        public const string RASTER_ULY  = "Raster Upper Left Y";
        public const string WORLD_ULX   = "World Upper Left X";
        public const string WORLD_ULY   = "World Upper Left Y";
        public const string X_SCALE     = "X Scale";
        public const string Y_SCALE     = "Y Scale";
        public const string SCALE_UNITS = "Scale Units";
        public const string PROJECTION  = "Projection";
        
        // Instance variables
        Dimensions       dimensions;  // size of map in rows & cols
        System.TypeCode  bandType;    // the type of each band in raster
        int              bandSize;    // the number of bytes per band entry
        int              bandCount;   // the number of bands
        int              currPixel;   // id of current read/write pixel
        int              totalPixels; // rows*cols
        IMetadata        metadata;    // the metadata assoc w/ raster
        FileStream       file;        // the underlying raster file
        BinaryWriter     fileWriter;  // helper
        BinaryReader     fileReader;  // helper
        bool             open;        // file status: true if open
        RWFlag           mode;        // file open mode: Read or Write

        /// <summary>
        /// Create a file given specs        
        /// </summary>
        public ErdasImageFile(string filename,
                              Dimensions dimensions,
                              int bandCount,
                              System.TypeCode bandType,
                              IMetadata metadata)
        {
            // set instance variables
            this.open        = false;
            this.mode        = RWFlag.Write;
            this.dimensions  = dimensions;
            this.bandType    = bandType;
            this.bandCount   = bandCount;
            this.currPixel   = 0;
            this.totalPixels = dimensions.Rows*dimensions.Columns;
            this.metadata    = metadata;
            
            // if filename does not end in .gis or .lan throw exception
            string extension = Path.GetExtension(filename).ToLower();
            if (!(extension.Equals(".gis")) && !(extension.Equals(".lan")))
                throw new System.ApplicationException("Erdas file must have either GIS or LAN as extension");
                
            // if dimensions are messed up throw exception
            if ((dimensions.Rows < 1) || (dimensions.Columns < 1))
                throw new System.ApplicationException("Erdas file given invalid dimensions");
                
            // if bandCount messed up throw exception
            if ((bandCount < 1) || (bandCount > 0xffff))
                throw new System.ApplicationException("Erdas file given invalid band count");
            
            // more bandCount checking
            if (extension.Equals(".gis"))
            {
                if (bandCount > 1)
                    throw new System.ApplicationException("Erdas GIS files cannot support multiband images");
                if (bandType != System.TypeCode.Byte)
                    throw new System.ApplicationException("Erdas GIS files only suupport byte for bandtype");
            }
                
            // if bandType not System.Byte or System.UInt16 throw exception
            if (bandType == System.TypeCode.Byte)
                this.bandSize = 1;
            else if (bandType == System.TypeCode.UInt16)
                this.bandSize = 2;
            else
                throw new System.ApplicationException("Erdas file given unsupported band type");
             
            // open file for writing
            this.file = new FileStream(filename,FileMode.OpenOrCreate);
            this.fileWriter = new BinaryWriter(this.file);
            this.open = true;
            
            // write header (using metadata whenever possible)
            
            try
            {

                // sentinel
                byte[] sentinel = new byte[6];
                sentinel[0] = (byte) 'H';
                sentinel[1] = (byte) 'E';
                sentinel[2] = (byte) 'A';
                sentinel[3] = (byte) 'D';
                sentinel[4] = (byte) '7';
                sentinel[5] = (byte) '4';
                this.fileWriter.Write(sentinel);

                // packing
                System.UInt16 ipack;
                if (bandType == System.TypeCode.Byte)
                    ipack = 0;
                else
                    ipack = 2;
                this.fileWriter.Write(ipack);

                // nbands
                System.UInt16 nbands = (System.UInt16)bandCount;
                this.fileWriter.Write(nbands);

                // unused
                for (int i = 0; i < 6; i++)
                    this.fileWriter.Write((byte)0);

                // icols
                System.UInt32 icols = (System.UInt32)dimensions.Columns;
                this.fileWriter.Write(icols);

                // irows
                System.UInt32 irows = (System.UInt32)dimensions.Rows;
                this.fileWriter.Write(irows);

                // xstart
                System.Int32 xstart = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<System.Int32>(RASTER_ULX,ref xstart)))
                    {
                    }
                this.fileWriter.Write(xstart);

                // ystart
                System.Int32 ystart = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<System.Int32>(RASTER_ULY,ref ystart)))
                {
                }
                this.fileWriter.Write(ystart);

                // unused
                for (int i = 0; i < 56; i++)
                    this.fileWriter.Write((byte)0);

                // maptyp
                System.UInt16 maptyp = 99;  // 99 means NONE
                string projection = null;
                if ((metadata != null) &&
                    (metadata.TryGetValue<string>(PROJECTION,ref projection)))
                {
                    int projNum = Projections.find(projection);
                    if (projNum != -1)
                        maptyp = (System.UInt16)projNum;
                }
                this.fileWriter.Write(maptyp);

                // nclass : calc if needed but never has been in past
                System.UInt16 nclass = 0;
                this.fileWriter.Write(nclass);

                // unused
                for (int i = 0; i < 14; i++)
                    this.fileWriter.Write((byte)0);

                // iautyp : first need xcell and ycell and then acre
                System.Single xcell = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<System.Single>(X_SCALE,ref xcell)))
                {
                }
                if (maptyp == 99)
                    xcell = 0;
                System.Single ycell = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<System.Single>(Y_SCALE,ref ycell)))
                {
                }
                if (maptyp == 99)
                    ycell = 0;
                System.UInt16 iautyp = 0;
                System.Single acre = 0;
                switch (maptyp)  // iautyp depends upon maptyp indirectly
                {
                    case 0: // Lat/Long -> dist unit == degrees
                        iautyp = 0;  // default to no area unit
                        break;
                    case 2: // State Plane ->  dist unit == feet
                        iautyp = 1;  // default to acres
                        acre = xcell * ycell;
                        // acre = sq.feet at this pt
                        //   so now convert to acres
                        acre = (float) ((double)acre * 0.0000229568411386593);
                        break;
                    default: //  dist unit == meters
                        iautyp = 2;  // default to hectares
                        acre = xcell * ycell;
                        // acre = sq.meters at this pt
                        //   so now convert to hectares
                        acre *= 0.0001f;
                        break;
                }
                this.fileWriter.Write(iautyp);

                // acre
                this.fileWriter.Write(acre);

                // xmap
                System.Single xmap = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<System.Single>(WORLD_ULX,ref xmap)))
                {
                }
                this.fileWriter.Write(xmap);

                // ymap
                System.Single ymap = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<System.Single>(WORLD_ULY,ref ymap)))
                {
                }
                this.fileWriter.Write(ymap);

                // xcell
                this.fileWriter.Write(xcell);

                // ycell
                this.fileWriter.Write(ycell);

                // now create pixel data as zeroes for now
                // many nested for loops avoids index calc overflows
                for (int row = 0; row < dimensions.Rows; row++)
                    for (int bandNum = 0; bandNum < this.bandCount; bandNum++)
                        for (int col = 0; col < dimensions.Columns; col++)
                            for (int byteNum = 0; byteNum < this.bandSize; byteNum++)
                                this.fileWriter.Write((byte)0);
            }
            catch (System.Exception)
            {
                Close();
                throw;
            }
        }
        
        /// <summary>
        /// Open an existing file
        /// </summary>
        public ErdasImageFile(string filename, RWFlag mode)
        {
            this.open = false;
            this.mode = mode;
            
            // if filename does not end in .gis or .lan throw exception
            string extension = Path.GetExtension(filename).ToLower();
            if (!(extension.Equals(".gis")) && !(extension.Equals(".lan")))
                throw new System.ApplicationException("Erdas file must have either GIS or LAN as extension");
                
            // open file
            this.file = new FileStream(filename,FileMode.Open);
            this.fileReader = new BinaryReader(this.file);
            this.open = true;

            try
            {
                // prepare to build metadata while reading
                Metadata metadata = new Metadata();

                // Read Header

                // if not start with "HEAD74" throw exception
                byte[] sentinel = fileReader.ReadBytes(6);
                if ((sentinel[0] != (byte)'H') ||
                    (sentinel[1] != (byte)'E') ||
                    (sentinel[2] != (byte)'A') ||
                    (sentinel[3] != (byte)'D') ||
                    (sentinel[4] != (byte)'7') ||
                    (sentinel[5] != (byte)'4'))
                    throw new System.ApplicationException(filename+" is not an ERDAS 7.4 compatible image file");

                // packing
                System.UInt16 ipack = this.fileReader.ReadUInt16();
                if ((ipack != 0) && (ipack != 2))
                    throw new System.ApplicationException("Only 8 and 16 bit bands are supported by Erdas reader");

                // nbands
                System.UInt16 nbands = this.fileReader.ReadUInt16();

                // unused
                byte[] unused = this.fileReader.ReadBytes(6);

                // icols
                System.UInt32 icols = this.fileReader.ReadUInt32();

                // irows
                System.UInt32 irows = this.fileReader.ReadUInt32();

                // xstart
                System.Int32 xstart = this.fileReader.ReadInt32();
                metadata[RASTER_ULX] = xstart;

                // ystart
                System.Int32 ystart = this.fileReader.ReadInt32();
                metadata[RASTER_ULY] = ystart;

                // unused
                unused = this.fileReader.ReadBytes(56);

                // maptyp
                System.UInt16 maptyp = this.fileReader.ReadUInt16();
                string projection = Projections.find(maptyp);
                if (projection != null)
                    metadata[PROJECTION] = projection;
                if (maptyp == 0)
                    metadata[SCALE_UNITS] = "degrees";
                else if (maptyp == 2)
                    metadata[SCALE_UNITS] = "feet";
                else
                    metadata[SCALE_UNITS] = "meters";

                // nclass : calc if needed but never has been in past
                System.UInt16 nclass = this.fileReader.ReadUInt16();

                // unused
                unused = this.fileReader.ReadBytes(14);

                // iautyp
                System.UInt16 iautyp = this.fileReader.ReadUInt16();

                // acre
                System.Single acre = this.fileReader.ReadSingle();

                // xmap
                System.Single xmap = this.fileReader.ReadSingle();
                metadata[WORLD_ULX] = xmap;

                // ymap
                System.Single ymap = this.fileReader.ReadSingle();
                metadata[WORLD_ULY] = ymap;

                // xcell
                System.Single xcell = this.fileReader.ReadSingle();
                metadata[X_SCALE] = xcell;

                // ycell
                System.Single ycell = this.fileReader.ReadSingle();
                metadata[Y_SCALE] = ycell;

                // construct instance variables based upon hedaer info
                this.dimensions  = new Dimensions((int)irows,(int)icols);
                if (ipack == 0)
                {
                    this.bandType    = System.TypeCode.Byte;
                    this.bandSize    = 1;
                }
                else  // ipack == 2 due to earlier screening
                {
                    this.bandType    = System.TypeCode.UInt16;
                    this.bandSize    = 2;
                }
                this.bandCount   = nbands;
                this.currPixel   = 0;
                this.totalPixels = (int)irows * (int)icols;
                this.metadata    = metadata;

                if (mode == RWFlag.Write)
                {
                    this.fileReader.Close();
                    this.fileReader = null;
                    // need to reopen stream - fileReader.Close() shuts it
                    this.file = new FileStream(filename,FileMode.Open);
                    this.fileWriter = new BinaryWriter(this.file);
                }
            }
            catch (System.Exception)
            {
                Close();
                throw;
            }
        }
        
        /// <summary>
        /// Write a pixel to the file. Assumes pixels will be written
        /// by the caller consecutively from upper left to bottom
        /// right a row at a time
        /// </summary>
        public void WritePixel(IPixel pixel)
        {
            try
            {
                if ((fileWriter == null) || (!this.open))
                     throw new System.ApplicationException("Raster not open for writing");

                if (currPixel >= totalPixels)
                    throw new System.ApplicationException("Writing beyond end of pixel data");

                int bandCount = pixel.BandCount;
                for (int bandNum = 0; bandNum < bandCount; bandNum++)
                {
                    IPixelBand band = pixel[bandNum];

                    // calc this pixelband's location in file
                    int location = PixelBandLocation(currPixel,bandNum);

                    this.fileWriter.Seek(location,SeekOrigin.Begin);

                    this.fileWriter.Write(band.GetBytes());
                }

                currPixel++;
            }
            catch (System.Exception)
            {
                Close();
                throw;
            }
        }

        /// <summary>
        /// Read a pixel from the file. Assumes pixels will be read
        /// by the caller consecutively from upper left to bottom
        /// right a row at a time
        /// </summary>
        public void ReadPixel(IPixel pixel)
        {
            try
            {
                if ((fileReader == null) || (!this.open))
                     throw new System.ApplicationException("Raster not open for reading");

                if (currPixel >= totalPixels)
                    throw new System.ApplicationException("Reading beyond end of pixel data");

                int bandCount = pixel.BandCount;
                for (int bandNum = 0; bandNum < bandCount; bandNum++)
                {
                    IPixelBand band = pixel[bandNum];

                    // calc this pixelband's location in file
                    int location = PixelBandLocation(currPixel,bandNum);

                    // seek to correct pixel spot
                    this.fileReader.BaseStream.Seek(location,SeekOrigin.Begin);

                    byte[] bytes = this.fileReader.ReadBytes(bandSize);

                    band.SetBytes(bytes,0);

                }

                currPixel++;
            }
            catch (System.Exception)
            {
                Close();
                throw;
            }
        }

        /// <summary>
        /// Find the file offset of the subpixel band of a particular pixel
        /// </summary>
        private int PixelBandLocation(int pixelNum, int bandNum)
        {
            int cols = dimensions.Columns;
            
            int currRow = pixelNum / cols;
            
            int currCol = pixelNum % cols;

            // all pxels start beyond header            
            int location = HeaderSize;
            
            // increment for each complete row
            location += currRow * bandCount * cols * bandSize;
            
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
            get { return dimensions; }
        }

        /// <summary>
        /// An Erdas GIS or LAN file has a certain number of bands present        
        /// </summary>
        public int BandCount
        {
            get { return bandCount; }
        }
        
        /// <summary>
        /// An Erdas GIS or LAN file has one type that applies equally to all bands
        /// </summary>
        public System.TypeCode BandType
        {
            get { return bandType; }
        }

        /// <summary>
        /// The metadata associated with this raster
        /// </summary>
        public IMetadata Metadata
        {
            get { return metadata; }
        }

        /// <summary>
        /// close file
        /// </summary>
        public void Close()
        {
            if (this.open)
            {
                if (this.fileWriter != null) this.fileWriter.Close();
                if (this.fileReader != null) this.fileReader.Close();
                // next line unecessary : prev lines Close's shut it
                //if (this.file != null)       this.file.Close();
                this.open = false;
            }
        }
        
        /// <summary>
        /// Returned read/write mode of file
        /// </summary>
        public RWFlag Mode
        {
            get { return this.mode; }
        }
        
        /// <summary>
        /// custom destructor needed to close resources        
        /// </summary>
        ~ErdasImageFile()
        {
            Close();
        }
    }
}
