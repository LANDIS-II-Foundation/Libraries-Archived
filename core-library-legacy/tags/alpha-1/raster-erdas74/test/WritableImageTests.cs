using Edu.Wisc.Forest.Flel.Util;
using Landis.Raster;
using Landis.Raster.Erdas74;
using NUnit.Framework;
using System;


namespace Landis.Test.Raster.Erdas74
{
    [TestFixture]
    public class WritableImageTests
    {
        private string outputGisImagePath;

        [SetUp]
        public void Init()
        {
            outputGisImagePath = Data.MakeOutputPath("junk.gis");
        }
        
        [Test]
        public void WritePixels()
        {
            Erdas74Pixel8 pixel = new Erdas74Pixel8();
            
            WritableImage image = new WritableImage(outputGisImagePath,
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
            WritableImage image = null;
            try {
                Erdas74Pixel8 pixel = new Erdas74Pixel8();
                
                image = new WritableImage(outputGisImagePath,
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
    }
}