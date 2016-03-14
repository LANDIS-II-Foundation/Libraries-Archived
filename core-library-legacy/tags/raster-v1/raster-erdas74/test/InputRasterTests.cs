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
            ErdasImageFile image = new ErdasImageFile(gisImagePath, RWFlag.Read);
            InputRaster<Erdas74Pixel8> raster = new InputRaster<Erdas74Pixel8>(image);
            raster.Close();
        }

        private void TryCtor<T>(string imagePath,
                                RWFlag rwFlag)
        	where T : IPixel, new()
        {
        	ErdasImageFile image = null;
        	try {
	            image = new ErdasImageFile(imagePath, rwFlag);
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
        public void OpenWrite()
        {
            // open Erdas file for writing
            TryCtor<Erdas74Pixel8>(gisImagePath, RWFlag.Write);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void WrongDataType1()
        {
            // fred.gis has 8-bit pixels
            TryCtor<Erdas74Pixel16>(gisImagePath, RWFlag.Read);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void WrongDataType2()
        {
            // fred.lan has 32-bit pixels
            TryCtor<Erdas74Pixel8>(lanImagePath, RWFlag.Write);
        }
        
        [Test]
        public void Dimensions()
        {
            ErdasImageFile image = new ErdasImageFile(gisImagePath, RWFlag.Read);
            InputRaster<Erdas74Pixel8> raster = new InputRaster<Erdas74Pixel8>(image);
            Assert.AreEqual(60,raster.Dimensions.Rows);
            Assert.AreEqual(40,raster.Dimensions.Columns);
            raster.Close();
        }
        
        [Test]
        public void ReadPixels()
        {
            ErdasImageFile image = new ErdasImageFile(gisImagePath, RWFlag.Read);
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
	            ErdasImageFile image = new ErdasImageFile(gisImagePath, RWFlag.Read);
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
            ErdasImageFile image = new ErdasImageFile(gisImagePath, RWFlag.Read);
            InputRaster<Erdas74Pixel8> raster = new InputRaster<Erdas74Pixel8>(image);
            
            double data = 0.0;
            
            Assert.AreEqual(false,raster.Metadata.TryGetValue<double>("Anything",ref data));
            
            raster.Close();
        }
    }
}
