using Edu.Wisc.Forest.Flel.Util;
using Landis.Raster;
using Landis.Raster.Erdas74;
using NUnit.Framework;
using System;


namespace Landis.Test.Raster.Erdas74
{
//InputRaster
// - public InputRaster(ErdasImageFile image)
// - public T ReadPixel()
// - public Dimensions Dimensions
// - public IMetadata Metadata
// - public void Close()

    [TestFixture]
    public class InputRasterTests
    {
        private string gisImagePath;
        private string lanImagePath;

        [SetUp]
        public void Init()
        {
            gisImagePath = Data.MakeInputPath("fred.gis");
            lanImagePath = Data.MakeInputPath("fred.lan");
        }

        [Test]
        public void Constructor()
        {
            ReadableImage image = new ReadableImage(gisImagePath);
            InputRaster<Erdas74Pixel8> raster = new InputRaster<Erdas74Pixel8>(image);
            raster.Close();
        }

        private void TryCtor<T>(string imagePath)
            where T : IPixel, new()
        {
            ReadableImage image = null;
            try {
                image = new ReadableImage(imagePath);
                InputRaster<T> raster = new InputRaster<T>(image);
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
        public void WrongDataType1()
        {
            // fred.gis has 8-bit pixels
            TryCtor<Erdas74Pixel16>(gisImagePath);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void WrongDataType2()
        {
            // fred.lan has 32-bit pixels
            TryCtor<Erdas74Pixel8>(lanImagePath);
        }
        
        [Test]
        public void Dimensions()
        {
            ReadableImage image = new ReadableImage(gisImagePath);
            InputRaster<Erdas74Pixel8> raster = new InputRaster<Erdas74Pixel8>(image);
            Assert.AreEqual(60,raster.Dimensions.Rows);
            Assert.AreEqual(40,raster.Dimensions.Columns);
            raster.Close();
        }
        
        [Test]
        public void ReadPixels()
        {
            ReadableImage image = new ReadableImage(gisImagePath);
            InputRaster<Erdas74Pixel8> raster = new InputRaster<Erdas74Pixel8>(image);
            Erdas74Pixel8 pixel8 = new Erdas74Pixel8();
            int totPixels = raster.Dimensions.Rows * raster.Dimensions.Columns;
            for (int i = 0; i < totPixels; i++)
              pixel8 = raster.ReadPixel();
            raster.Close();
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void ReadTooManyPixels()
        {
            try {
                ReadableImage image = new ReadableImage(gisImagePath);
                InputRaster<Erdas74Pixel8> raster = new InputRaster<Erdas74Pixel8>(image);
                using (raster) {
                    Erdas74Pixel8 pixel8 = new Erdas74Pixel8();
                    int totPixels = raster.Dimensions.Rows * raster.Dimensions.Columns;
                    for (int i = 0; i < totPixels; i++)
                        pixel8 = raster.ReadPixel();
                    //one too many
                    pixel8 = raster.ReadPixel();
                }
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine(exc.Message);
                throw;
            }
        }
        
        [Test]
        public void Metadata()
        {
            ReadableImage image = new ReadableImage(gisImagePath);
            InputRaster<Erdas74Pixel8> raster = new InputRaster<Erdas74Pixel8>(image);
            
            double data = 0.0;
            
            Assert.AreEqual(false,raster.Metadata.TryGetValue<double>("Anything",ref data));
            
            raster.Close();
        }
    }
}
