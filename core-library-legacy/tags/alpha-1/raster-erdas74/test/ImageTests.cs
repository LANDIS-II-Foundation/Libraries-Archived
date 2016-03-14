using Edu.Wisc.Forest.Flel.Util;
using Landis.Raster;
using Landis.Raster.Erdas74;
using NUnit.Framework;
using System;


namespace Landis.Test.Raster.Erdas74
{
    [TestFixture]
    public class ImageTests
    {
        private string inputGisImagePath;

        private string outputGisImagePath;
        private string outputLanImagePath;

        [SetUp]
        public void Init()
        {
            inputGisImagePath = Data.MakeInputPath("fred.gis");

            outputGisImagePath = Data.MakeOutputPath("ErdasImageFileTests.gis");
            outputLanImagePath = Data.MakeOutputPath("ErdasImageFileTests.lan");
        }

        [Test]
        public void ConstructNewLan()
        {
            Image image =
                new Image(outputLanImagePath,
                          new Dimensions(100,50),
                          2,
                          System.TypeCode.UInt16,
                          null);
            Assert.AreEqual(100, image.Dimensions.Rows);
            Assert.AreEqual(50, image.Dimensions.Columns);
            Assert.AreEqual(2, image.BandCount);
            Assert.AreEqual(System.TypeCode.UInt16, image.BandType);
            Assert.AreEqual(null, image.Metadata);
        }

        [Test]
        public void ConstructNewGis()
        {
            Image image = new Image(outputGisImagePath,
                                     new Dimensions(60,40),
                                     1,
                                     System.TypeCode.Byte,
                                     null);
            Assert.AreEqual(60, image.Dimensions.Rows);
            Assert.AreEqual(40, image.Dimensions.Columns);
            Assert.AreEqual(1, image.BandCount);
            Assert.AreEqual(System.TypeCode.Byte, image.BandType);
            Assert.AreEqual(null, image.Metadata);
        }

        private void TryCtor(string filename,
                             Dimensions dimensions,
                             int bandCount,
                             System.TypeCode bandType,
                             IMetadata metadata)
        {
            try {
                Image image = new Image(filename, dimensions,
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
        public void OpenExistingReadOnly()
        {
            Image image = new Image(inputGisImagePath);
        }
        
        private void TryOpen(string filename)
        {
            try {
                Image image = new Image(filename);
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
            TryOpen(Data.MakeInputPath("fred.beep"));
        }
        
        [Test]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void open_exception2()
        {
            // file does not exist
            TryOpen(Data.MakeInputPath("beeple.gis"));
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void open_exception3()
        {
            // file does not start with HEAD74 - not a true GIS/LAN file
            TryOpen(Data.MakeInputPath("bad.gis"));
        }
        
    }
}
