using Edu.Wisc.Forest.Flel.Util;
using Landis.Raster;
using Landis.Raster.Erdas74;
using NUnit.Framework;
using System;


namespace Landis.Test.Raster.Erdas74
{
//Library
//        public IInputRaster<T> Open<T>(string path)
//        public IOutputRaster<T> Create<T>(string path,Dimensions dimensions,IMetadata metadata)

    [TestFixture]
    public class LibraryTests
    {
    	private Library lib;

    	[SetUp]
    	public void Init()
    	{
    		lib = new Library();
    	}

    	[Test]
        public void Open1()
        {
            IInputRaster<Erdas74Pixel8> raster =
            	lib.Open<Erdas74Pixel8>(Data.MakeInputPath("fred.gis"));
            raster.Close();
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void Open2()
        {
            // not made up of 16bit pixels
            IInputRaster<Erdas74Pixel16> raster;
        	try {
            	raster = lib.Open<Erdas74Pixel16>(Data.MakeInputPath("fred.gis"));
	            raster.Close();
        	}
            catch (Exception exc) {
            	Data.Output.WriteLine(exc.Message);
            	throw;
            }
        }
        
        [Test]
        public void Create1()
        {
            IOutputRaster<Erdas74Pixel8> raster =
            	lib.Create<Erdas74Pixel8>(Data.MakeOutputPath("temp.gis"),
                                            new Dimensions(20,10),
                                            null);
            raster.Close();
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void Create2()
        {
            IOutputRaster<Erdas74Pixel16> raster;
            try {
            	raster = lib.Create<Erdas74Pixel16>(Data.MakeOutputPath("temp.gis"),
                                                    new Dimensions(20,10),
                                                    null);
	            raster.Close();
        	}
            catch (Exception exc) {
            	Data.Output.WriteLine(exc.Message);
            	throw;
            }
        }
    }
}
