using Edu.Wisc.Forest.Flel.Util;
using Landis.Raster;
using Landis.Raster.Erdas74;
using NUnit.Framework;
using System;

namespace Landis.Test.Raster.Erdas74
{
    [TestFixture]
    public class ReadableImageTests
    {
        private string inputLanImagePath;

        [SetUp]
        public void Init()
        {
            inputLanImagePath = Data.MakeInputPath("fred.lan");
        }
        
        [Test]
        public void ReadPixels()
        {
            ReadableImage image = new ReadableImage(inputLanImagePath);
            
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
            ReadableImage image = null;
            try {
                image = new ReadableImage(inputLanImagePath);
                
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
        
    }
}