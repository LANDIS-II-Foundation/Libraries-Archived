using Edu.Wisc.Forest.Flel.Util;
using Landis.Raster;
using Landis.Raster.Erdas74;
using NUnit.Framework;
using System;


namespace Landis.Test.Raster.Erdas74
{

//ErdasImageFile
//  - constructor 1
//  - constructor 2
//  - writePixel
//  - readPixel
//  - pixelBandLocation
//  - dimensions
//  - bandCount
//  - bandType
//  - metadata
//  - close
//  - destructor

    [TestFixture]
    public class ErdasImageFileTests
    {
        private string inputGisImagePath;
        private string inputLanImagePath;

        private string outputGisImagePath;
        private string outputLanImagePath;

        [SetUp]
        public void Init()
        {
            inputGisImagePath = Data.MakeInputPath("fred.gis");
            inputLanImagePath = Data.MakeInputPath("fred.lan");

            outputGisImagePath = Data.MakeOutputPath("ErdasImageFileTests.gis");
            outputLanImagePath = Data.MakeOutputPath("ErdasImageFileTests.lan");
        }

        [Test]
        public void NewPix8()
        {
            Erdas74Pixel8 pix8 = new Erdas74Pixel8();
        }
        
        [Test]
        public void NewPix16()
        {
            Erdas74Pixel16 pix16 = new Erdas74Pixel16();
        }
        
        [Test]
        public void ConstructNewLan()
        {
            ErdasImageFile
            image =
                new ErdasImageFile(outputLanImagePath,
                                    new Dimensions(100,50),
                                    2,
                                    System.TypeCode.UInt16,
                                    null);
            Assert.AreEqual(100, image.Dimensions.Rows);
            Assert.AreEqual(50, image.Dimensions.Columns);
            Assert.AreEqual(2, image.BandCount);
            Assert.AreEqual(System.TypeCode.UInt16, image.BandType);
            Assert.AreEqual(null, image.Metadata);
            
            image.Close();  // needed or file stays locked after run of tests. why?
        }

        [Test]
        public void ConstructNewGis()
        {
            ErdasImageFile
            image = new ErdasImageFile(outputGisImagePath,
                                         new Dimensions(60,40),
                                         1,
                                         System.TypeCode.Byte,
                                         null);
            Assert.AreEqual(60, image.Dimensions.Rows);
            Assert.AreEqual(40, image.Dimensions.Columns);
            Assert.AreEqual(1, image.BandCount);
            Assert.AreEqual(System.TypeCode.Byte, image.BandType);
            Assert.AreEqual(null, image.Metadata);
            
            image.Close();  // needed or file stays locked after run of tests. why?
        }

        private void TryCtor(string filename,
                             Dimensions dimensions,
                             int bandCount,
                             System.TypeCode bandType,
                             IMetadata metadata)
        {
            try {
                ErdasImageFile image = new ErdasImageFile(filename, dimensions,
                                                          bandCount, bandType,
                                                          metadata);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine(exc.Message);
                throw;
            }
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void create_exception1()
        {
            // extension not GIS/LAN
            TryCtor(Data.MakeOutputPath("fail.blp"),
                    new Dimensions(60,40),
                    1,
                    System.TypeCode.Byte,
                    null);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void create_exception2()
        {
            // 0 rows
            TryCtor(Data.MakeOutputPath("fail.gis"),
                    new Dimensions(0,40),
                    1,
                    System.TypeCode.Byte,
                    null);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void create_exception3()
        {
            // -1 cols
            TryCtor(Data.MakeOutputPath("fail.gis"),
                    new Dimensions(50,-1),
                    1,
                    System.TypeCode.Byte,
                    null);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void create_exception4()
        {
            // gis w/ >1 band
            TryCtor(Data.MakeOutputPath("fail.gis"),
                    new Dimensions(50,50),
                    2,
                    System.TypeCode.Byte,
                    null);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void create_exception5()
        {
            // 0 bands
            TryCtor(Data.MakeOutputPath("fail.gis"),
                    new Dimensions(50,50),
                    0,
                    System.TypeCode.UInt16,
                    null);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void create_exception6()
        {
            // > 65535 bands
            TryCtor(Data.MakeOutputPath("fail.gis"),
                    new Dimensions(50,50),
                    70000,
                    System.TypeCode.UInt16,
                    null);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void create_exception7()
        {
            // unsupported band type
            TryCtor(Data.MakeOutputPath("fail.gis"),
                    new Dimensions(50,50),
                    1,
                    System.TypeCode.UInt32,
                    null);
        }
        
        [Test]
        public void OpenExistingReadOnly()
        {
            ErdasImageFile
            image = new ErdasImageFile(inputGisImagePath, RWFlag.Read);
            image.Close();
        }
        
        [Test]
        public void OpenExistingWritable()
        {
            ErdasImageFile
            image = new ErdasImageFile(outputGisImagePath, RWFlag.Write);
            image.Close();
        }
        
        private void TryOpen(string filename,
                             RWFlag rwFlag)
        {
            try {
                ErdasImageFile image = new ErdasImageFile(filename, rwFlag);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine(exc.Message);
                throw;
            }
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void open_exception1()
        {
            // bad extension
            TryOpen(Data.MakeInputPath("fred.beep"), RWFlag.Read);
        }
        
        [Test]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void open_exception2()
        {
            // file does not exist
            TryOpen(Data.MakeInputPath("beeple.gis"), RWFlag.Read);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void open_exception3()
        {
            // file does not start with HEAD74 - not a true GIS/LAN file
            TryOpen(Data.MakeInputPath("bad.gis"), RWFlag.Read);
        }
        
        [Test]
        public void ReadPixels()
        {
            ErdasImageFile
            image = new ErdasImageFile(inputLanImagePath, RWFlag.Read);
            
            FredLanPixel pixel = new FredLanPixel();
            
            int pixCount = image.Dimensions.Rows * image.Dimensions.Columns;
            
            for (int i = 0; i < pixCount; i++)
            {
                image.ReadPixel(pixel);
            }
            
            image.Close();
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void ReadTooManyPixels()
        {
            ErdasImageFile image = null;
            try {
                image = new ErdasImageFile(inputLanImagePath, RWFlag.Read);
                
                FredLanPixel pixel = new FredLanPixel();
    
                int pixCount = image.Dimensions.Rows * image.Dimensions.Columns;
                
                for (int i = 0; i < pixCount; i++)
                {
                    image.ReadPixel(pixel);
                }
            
                // one too many
                image.ReadPixel(pixel);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine(exc.Message);
                throw;
            }
            finally {
                if (image != null)
                    image.Close();
            }
        }
        
        [Test]
        public void WritePixels()
        {
            Erdas74Pixel8 pixel = new Erdas74Pixel8();
            
            ErdasImageFile
            image = new ErdasImageFile(outputGisImagePath,
                                       new Dimensions(10,10),
                                       1,
                                       System.TypeCode.Byte,
                                       null);

            byte[] bytes = new byte[1];
            bytes[0] = 1;
            
            pixel[0].SetBytes(bytes,0);
            
            int pixCount = image.Dimensions.Rows * image.Dimensions.Columns;
            
            for (int i = 0; i < pixCount; i++)
            {
                image.WritePixel(pixel);
            }
            
            image.Close();
            
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void WriteTooManyPixels()
        {
            ErdasImageFile image = null;
            try {
                Erdas74Pixel8 pixel = new Erdas74Pixel8();
                
                image = new ErdasImageFile(outputGisImagePath,
                                           new Dimensions(10,10),
                                           1,
                                           System.TypeCode.Byte,
                                           null);
    
                byte[] bytes = new byte[1];
                bytes[0] = 1;
                
                pixel[0].SetBytes(bytes,0);
                
                int pixCount = image.Dimensions.Rows * image.Dimensions.Columns;
    
                for (int i = 0; i < pixCount; i++)
                {
                    image.WritePixel(pixel);
                }
                
                // one too many
                image.WritePixel(pixel);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine(exc.Message);
                throw;
            }
            finally {
                if (image != null)
                    image.Close();
            }
            
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void ReadFromWritable1()
        {
            ErdasImageFile image = null;
            try {
                image = new ErdasImageFile(Data.MakeOutputPath("junk.gis"),
                                             new Dimensions(10,10),
                                             1,
                                             System.TypeCode.Byte,
                                             null);
    
                Erdas74Pixel8 pixel = new Erdas74Pixel8();
    
                // read after opening for Write
                image.ReadPixel(pixel);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine(exc.Message);
                throw;
            }
            finally {
                if (image != null)
                    image.Close();
            }
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void ReadFromWritable2()
        {
            ErdasImageFile image = null;
            try {
                image = new ErdasImageFile(Data.MakeOutputPath("junk.gis"), RWFlag.Write);
                
                Erdas74Pixel8 pixel = new Erdas74Pixel8();
                
                // read after opening for Write
                image.ReadPixel(pixel);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine(exc.Message);
                throw;
            }
            finally {
                if (image != null)
                    image.Close();
            }
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void WriteToReadable()
        {
            ErdasImageFile image = null;
            try {
                image = new ErdasImageFile(Data.MakeOutputPath("junk.gis"),
                                           RWFlag.Read);
                
                Erdas74Pixel8 pixel = new Erdas74Pixel8();
                
                // write after opening for Read
                image.WritePixel(pixel);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine(exc.Message);
                throw;
            }
            finally {
                if (image != null)
                    image.Close();
            }
        }
        
        /*
        // if you run this test everything works. But if you then try to delete the
        // associated data files you get that files are in use by another process.
        // its like NUnit doesn't run the garbage collector.
        [Test]
        public void NoClose()
        {
            image =
                new ErdasImageFile("c:\\temp\\fred.lan",
                                    new Dimensions(100,50),
                                    2,
                                    System.TypeCode.UInt16,
                                    null);
            Assert.AreEqual(100, image.Dimensions.Rows);
            Assert.AreEqual(50, image.Dimensions.Columns);
            Assert.AreEqual(2, image.BandCount);
            Assert.AreEqual(System.TypeCode.UInt16, image.BandType);
            
            image = new ErdasImageFile("c:\\temp\\fred.gis",
                                         new Dimensions(60,40),
                                         1,
                                         System.TypeCode.Byte,
                                         null);
            Assert.AreEqual(60, image.Dimensions.Rows);
            Assert.AreEqual(40, image.Dimensions.Columns);
            Assert.AreEqual(1, image.BandCount);
            Assert.AreEqual(System.TypeCode.Byte, image.BandType);
        }
        */
    }
}
