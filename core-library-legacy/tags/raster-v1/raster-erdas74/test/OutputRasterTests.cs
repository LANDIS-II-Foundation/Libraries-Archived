using Edu.Wisc.Forest.Flel.Util;
using Landis.Raster;
using Landis.Raster.Erdas74;
using NUnit.Framework;
using System;


namespace Landis.Test.Raster.Erdas74
{
//OutputRaster
//        public OutputRaster(ErdasImageFile image)
//        public void WritePixel(T pixel)
//        public Dimensions Dimensions
//        public IMetadata Metadata
//        public void Close()

    [TestFixture]
    public class OutputRasterTests
    {
    	private string gisImagePath;
    	private string lanImagePath;

    	[SetUp]
    	public void Init()
    	{
    		gisImagePath = Data.MakeOutputPath("OutputRasterTests.gis");
    		ErdasImageFile image;
    		image = new ErdasImageFile(gisImagePath,
                                       new Dimensions(60,40),
                                       1,
                                       System.TypeCode.Byte,
                                       null);
    		image.Close();

    		lanImagePath = Data.MakeOutputPath("OutputRasterTests.lan");
    		image = new ErdasImageFile(lanImagePath,
                                       new Dimensions(100,50),
                                       2,
                                       System.TypeCode.UInt16,
                                       null);
    		image.Close();
    	}

    	[Test]
        public void Constructor()
        {
            ErdasImageFile image = new ErdasImageFile(gisImagePath,RWFlag.Write);
            OutputRaster<Erdas74Pixel8> raster = new OutputRaster<Erdas74Pixel8>(image);
            raster.Close();
        }

        private void TryCtor<T>(string imagePath,
                                RWFlag rwFlag)
        	where T : IPixel, new()
        {
        	ErdasImageFile image = null;
        	try {
	            image = new ErdasImageFile(imagePath, rwFlag);
	            OutputRaster<T> raster = new OutputRaster<T>(image);
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
        public void OpenRead()
        {
            // open Erdas file for writing
        	TryCtor<Erdas74Pixel8>(gisImagePath, RWFlag.Read);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void WrongDataType1()
        {
            // gis image has 8-bit pixels
        	TryCtor<Erdas74Pixel16>(gisImagePath, RWFlag.Write);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void WrongDataType2()
        {
            // lan image has 32-bit pixels
        	TryCtor<Erdas74Pixel8>(lanImagePath, RWFlag.Write);
        }
        
        [Test]
        public void Dimensions()
        {
            ErdasImageFile image = new ErdasImageFile(gisImagePath, RWFlag.Write);
            OutputRaster<Erdas74Pixel8> raster = new OutputRaster<Erdas74Pixel8>(image);
            Assert.AreEqual(60,raster.Dimensions.Rows);
            Assert.AreEqual(40,raster.Dimensions.Columns);
            raster.Close();
        }
        
        [Test]
        public void WritePixels()
        {
            ErdasImageFile image = new ErdasImageFile(gisImagePath ,RWFlag.Write);
            OutputRaster<Erdas74Pixel8> raster = new OutputRaster<Erdas74Pixel8>(image);
            Erdas74Pixel8 pixel8 = new Erdas74Pixel8();
            int totPixels = raster.Dimensions.Rows * raster.Dimensions.Columns;
            for (int i = 0; i < totPixels; i++)
              raster.WritePixel(pixel8);
            raster.Close();
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void WriteTooManyPixels()
        {
            try {
            	ErdasImageFile image = new ErdasImageFile(gisImagePath, RWFlag.Write);
	            OutputRaster<Erdas74Pixel8> raster = new OutputRaster<Erdas74Pixel8>(image);
	            using (raster) {
		            Erdas74Pixel8 pixel8 = new Erdas74Pixel8();
		            int totPixels = raster.Dimensions.Rows * raster.Dimensions.Columns;
		            for (int i = 0; i < totPixels; i++)
		            	raster.WritePixel(pixel8);
		            // write one too many
		            raster.WritePixel(pixel8);
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
            ErdasImageFile image = new ErdasImageFile(gisImagePath, RWFlag.Write);
            OutputRaster<Erdas74Pixel8> raster = new OutputRaster<Erdas74Pixel8>(image);
            
            double data = 0.0;
            
            Assert.AreEqual(false,raster.Metadata.TryGetValue<double>("Anything",ref data));
            
            raster.Close();
        }
    }
}
