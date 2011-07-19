
using System;
using System.IO;
using Landis.Raster;

namespace Landis.Raster.Erdas74
{
    /// Erdas file header structure
    ///
    /// Bytes 1-6     : HDWORD - a 6 byte array containing 'HEAD74'
    /// Bytes 7-8     : IPACK - an integer val which indicates the bits per
    ///                   subpixel band:  0 = 8 bit   1 = 4 bit   2 = 16 bit
    /// Bytes 9-10    : NBANDS - integer that indicates the number of bands per pixel
    ///                   Always 1 for GIS files, >= 1 for LAN files
    /// Bytes 11-16   : unused
    /// Bytes 17-20   : ICOLS - an integer specifying the width of file in pixels
    /// Bytes 21-24   : ICOLS - an integer specifying the length of file in pixels
    /// Bytes 25-28   : XSTART - integer specifying the database x-coord of upper
    ///                   left pixel. Raster coord space.
    /// Bytes 29-32   : YSTART - integer specifying the database y-coord of upper
    ///                   left pixel. Raster coord space.
    /// Bytes 33-88   : unused
    /// Bytes 89-90   : MAPTYP - the projection number - see Projections.cs for defs
    /// Bytes 91-92   : NCLASS - the number of classes present in map. We just make 0.
    /// Bytes 93-106  : unused
    /// Bytes 107-108 : IAUTYP - integer indicating area unit associated with each
    ///                   pixel. 0 = None, 1 = Acre, 2 = Hectare, 3 = Other
    /// Bytes 109-112 : ACRE - A float specifying number of area units of each pixel
    /// Bytes 113-116 : XMAP - A float giving x map coord for the upper left pixel.
    ///                   World coord space.
    /// Bytes 117-120 : YMAP - A float giving y map coord for the upper left pixel.
    ///                   World coord space.
    /// Bytes 121-124 : XCELL - A float specifying the x size of each cell. Units
    ///                   depend on MAPTYP: 2 (State Plane) = feet, 0 (Lat/Lon) =
    ///                   degrees, all others = meters
    /// Bytes 125-128 : YCELL - A float specifying the y size of each cell. Units
    ///                   depend on MAPTYP: 2 (State Plane) = feet, 0 (Lat/Lon) =
    ///                   degrees, all others = meters
    
    public class ImageHeader
    {
        // ERDAS GIS/LAN file header size (in bytes)
        public const int Size = 128;

        // Instance variables
        private Dimensions       dimensions;  // size of map in rows & cols
        private System.TypeCode  bandType;    // the type of each band in raster
        private int              bandSize;    // the number of bytes per band entry
        private int              bandCount;   // the number of bands
        private Metadata         metadata;    // the metadata assoc w/ raster
        
        /// <summary>
        /// No arg constructor
        /// </summary>
        /// <remark>
        /// Used before calling Read()
        /// </remark>
        public ImageHeader()
        {
            this.dimensions = new Dimensions(0,0);
            this.bandType = System.TypeCode.Byte;
            this.bandSize = 0;
            this.bandCount = 0;
            this.metadata = new Metadata();
        }
        
        /// <summary>
        /// Fully specified constructor - used before calling Write()
        /// </summary>
        /// <remark>
        /// Usually used before calling Write() but could call NoArg constructor,
        /// Read(), and then Write() if copying images
        /// </remark>
        public ImageHeader(Dimensions dimensions, System.TypeCode bandType,
                                int bandCount, IMetadata metadata)
        {
            this.dimensions = dimensions;
            this.bandType   = bandType;
            this.bandCount  = bandCount;
            this.metadata   = metadata as Metadata;
            if (bandType == System.TypeCode.Byte)
                this.bandSize   = 1;
            else if (bandType == System.TypeCode.UInt16)
                this.bandSize   = 2;
            else
                throw new ApplicationException("ImageHeader: bandType must either be Byte or UInt16");
        }
        
        /// <summary>
        /// Get the row x col dimensions of the header
        /// </summary>
        public Dimensions Dimensions
        {
            get { return this.dimensions; }
        }
        
        /// <summary>
        /// Get the band type of the header
        /// </summary>
        /// <remark>
        /// Should always be either System.TypeCode.Byte or System.TypeCode.UInt16
        /// <remark>
        public System.TypeCode BandType
        {
            get { return this.bandType; }
        }
        
        /// <summary>
        /// Returns the number of bytes per subpixel band defined in the header
        /// </summary>
        public int BandSize
        {
            get { return this.bandSize; }
        }
        
        /// <summary>
        /// Returns the number of bands defined by the header
        /// </summary>
        public int BandCount
        {
            get { return this.bandCount; }
        }
        
        /// <summary>
        /// Returns the metadata associated with the header
        /// </summary>
        public IMetadata Metadata
        {
            get { return this.metadata; }
        }
        
        /// <summary>
        /// Read the Erdas Image Header from a file given filename
        /// </summary>
        /// <remark>
        /// Uses variable definitions straight from Erdas spec - ipack, nbands,
        /// irows, icols, xstart, ystart, maptyp, nclass, iautyp, acre, xmap,
        /// ymap, xcell, ycell
        /// </remark>
        public void Read(string filename)
        {
            // init members as needed
            this.metadata = new Metadata();
            
            // prepare to read header
            FileStream file = null;
            BinaryReader fileReader = null;
            System.UInt16 ipack = 0;
            System.UInt16 nbands = 0;
            System.UInt32 irows = 0;
            System.UInt32 icols = 0;

            try
            {
                // open file
                file = new FileStream(filename,FileMode.Open);
                fileReader = new BinaryReader(file);

                // Read Header

                // if not start with "HEAD74" throw exception
                byte[] sentinel = fileReader.ReadBytes(6);
                if ((sentinel[0] != (byte)'H') ||
                    (sentinel[1] != (byte)'E') ||
                    (sentinel[2] != (byte)'A') ||
                    (sentinel[3] != (byte)'D') ||
                    (sentinel[4] != (byte)'7') ||
                    (sentinel[5] != (byte)'4'))
                    throw new ApplicationException(filename+" is not an ERDAS 7.4 compatible image file");

                // packing
                ipack = fileReader.ReadUInt16();

                // nbands
                nbands = fileReader.ReadUInt16();

                // unused
                byte[] unused = fileReader.ReadBytes(6);

                // icols
                icols = fileReader.ReadUInt32();

                // irows
                irows = fileReader.ReadUInt32();

                // xstart
                System.Int32 xstart = fileReader.ReadInt32();
                this.metadata[MetadataIds.RASTER_ULX] = xstart;

                // ystart
                System.Int32 ystart = fileReader.ReadInt32();
                this.metadata[MetadataIds.RASTER_ULY] = ystart;

                // unused
                unused = fileReader.ReadBytes(56);

                // maptyp
                System.UInt16 maptyp = fileReader.ReadUInt16();
                string projection = Projections.find(maptyp);
                if (projection != null)
                    this.metadata[MetadataIds.PROJECTION] = projection;
                if (maptyp == 0)
                    this.metadata[MetadataIds.SCALE_UNITS] = "degrees";
                else if (maptyp == 2)
                    this.metadata[MetadataIds.SCALE_UNITS] = "feet";
                else
                    this.metadata[MetadataIds.SCALE_UNITS] = "meters";

                // nclass : calc if needed but never has been in past
                System.UInt16 nclass = fileReader.ReadUInt16();

                // unused
                unused = fileReader.ReadBytes(14);

                // iautyp
                System.UInt16 iautyp = fileReader.ReadUInt16();

                // acre
                System.Single acre = fileReader.ReadSingle();

                // xmap
                System.Single xmap = fileReader.ReadSingle();
                this.metadata[MetadataIds.WORLD_ULX] = xmap;

                // ymap
                System.Single ymap = fileReader.ReadSingle();
                this.metadata[MetadataIds.WORLD_ULY] = ymap;

                // xcell
                System.Single xcell = fileReader.ReadSingle();
                this.metadata[MetadataIds.X_SCALE] = xcell;

                // ycell
                System.Single ycell = fileReader.ReadSingle();
                this.metadata[MetadataIds.Y_SCALE] = ycell;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (fileReader != null)
                    fileReader.Close();
                    // the prev line automatically closes file also
                else if (file != null)
                    file.Close();
            }
            
            // now set instance vars
            // metadata already set by above code
            this.dimensions = new Dimensions((int)irows,(int)icols);
            this.bandCount = nbands;
            if (ipack == 0)
            {
                this.bandType = System.TypeCode.Byte;
                this.bandSize = 1;
            }
            else if (ipack == 2)
            {
                this.bandType = System.TypeCode.UInt16;
                this.bandSize = 2;
            }
            else
                throw new ApplicationException("ImageHeader: Only 8 and 16 bit bands are supported");
        }
        
        /// <summary>
        /// Write an Erdas Image Header to a file given filename
        /// </summary>
        /// <remark>
        /// Uses variable definitions straight from Erdas spec - ipack, nbands,
        /// irows, icols, xstart, ystart, maptyp, nclass, iautyp, acre, xmap,
        /// ymap, xcell, ycell
        /// </remark>
        public void Write(string filename)
        {
            // Internal conversion factors
            //   1 acre = 4840 sq. yards = 43560 sq. feet
            const double AcresPerSqFoot = 1.0 / 43560.0;
            //   1 hectare = 10000 sq. meters
            const double HectaresPerSqMeter = 1.0 / 10000.0;
        
            // prepare to write header (using metadata whenever possible)
            FileStream file = null;
            BinaryWriter fileWriter = null;
            
            try
            {
                file = new FileStream(filename,FileMode.OpenOrCreate);
                fileWriter = new BinaryWriter(file);
                
                // sentinel
                byte[] sentinel = new byte[6];
                sentinel[0] = (byte) 'H';
                sentinel[1] = (byte) 'E';
                sentinel[2] = (byte) 'A';
                sentinel[3] = (byte) 'D';
                sentinel[4] = (byte) '7';
                sentinel[5] = (byte) '4';
                fileWriter.Write(sentinel);

                // packing
                System.UInt16 ipack;
                if (bandType == System.TypeCode.Byte)
                    ipack = 0;
                else // bandType == System.TypeCode.UInt16
                    ipack = 2;
                fileWriter.Write(ipack);

                // nbands
                System.UInt16 nbands = (System.UInt16)bandCount;
                fileWriter.Write(nbands);

                // unused
                for (int i = 0; i < 6; i++)
                    fileWriter.Write((byte)0);

                // icols
                System.UInt32 icols = (System.UInt32)dimensions.Columns;
                fileWriter.Write(icols);

                // irows
                System.UInt32 irows = (System.UInt32)dimensions.Rows;
                fileWriter.Write(irows);

                // xstart
                System.Int32 xstart = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<Int32>(
                        MetadataIds.RASTER_ULX,ref xstart)))
                {
                }
                fileWriter.Write(xstart);

                // ystart
                System.Int32 ystart = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<Int32>(
                        MetadataIds.RASTER_ULY,ref ystart)))
                {
                }
                fileWriter.Write(ystart);

                // unused
                for (int i = 0; i < 56; i++)
                    fileWriter.Write((byte)0);

                // maptyp
                System.UInt16 maptyp = 99;  // 99 means NONE
                string projection = null;
                if ((metadata != null) &&
                    (metadata.TryGetValue<string>(
                        MetadataIds.PROJECTION,ref projection)))
                {
                    int projNum = Projections.find(projection);
                    if (projNum != -1)
                        maptyp = (System.UInt16)projNum;
                }
                fileWriter.Write(maptyp);

                // nclass : calc if needed but never has been in past
                System.UInt16 nclass = 0;
                fileWriter.Write(nclass);

                // unused
                for (int i = 0; i < 14; i++)
                    fileWriter.Write((byte)0);

                // iautyp : first need xcell and ycell and then acre
                
                System.Single xcell = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<Single>(
                        MetadataIds.X_SCALE,ref xcell)))
                {
                }
                
                System.Single ycell = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<Single>(
                        MetadataIds.Y_SCALE,ref ycell)))
                {
                }
                
                // 99 -> map type of NONE : by def xcell and ycell == 0
                if (maptyp == 99)
                {
                    xcell = 0;
                    ycell = 0;
                }
                
                System.UInt16 iautyp = 0;
                
                System.Single acre = 0;
                
                switch (maptyp)  // iautyp depends upon maptyp indirectly
                {
                    case 0: // Lat/Long -> dist unit == degrees
                        iautyp = 0;  // default to no area unit
                        acre = 0;
                        break;
                    case 2: // State Plane ->  dist unit == feet
                        iautyp = 1;  // default to acres
                        acre = xcell * ycell;
                        // acre = sq.feet at this pt
                        //   so now convert to acres
                        acre = (float) ((double)acre * AcresPerSqFoot);
                        break;
                    default: //  dist unit == meters
                        iautyp = 2;  // default to hectares
                        acre = xcell * ycell;
                        // acre = sq.meters at this pt
                        //   so now convert to hectares
                        acre = (float) ((double)acre * HectaresPerSqMeter);
                        break;
                }
                fileWriter.Write(iautyp);

                // acre
                fileWriter.Write(acre);

                // xmap
                System.Single xmap = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<Single>(
                        MetadataIds.WORLD_ULX,ref xmap)))
                {
                }
                fileWriter.Write(xmap);

                // ymap
                System.Single ymap = 0;
                if ((metadata != null) &&
                    (metadata.TryGetValue<Single>(
                        MetadataIds.WORLD_ULY,ref ymap)))
                {
                }
                fileWriter.Write(ymap);

                // xcell
                fileWriter.Write(xcell);

                // ycell
                fileWriter.Write(ycell);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (fileWriter != null)
                    fileWriter.Close();
                    // file automatically closed by prev line
                else if (file != null)
                    file.Close();
            }
            
        }
    }
}
