using Edu.Wisc.Forest.Flel.Util;
using Landis.Raster;
using Landis.Raster.Erdas74;
using NUnit.Framework;
using System;

namespace Landis.Test.Raster.Erdas74
{
    class FakeMetadata : IMetadata
    {
        public FakeMetadata()
        {
        }
        
        public object this[string name]
        {
            get { return null; }
        }
        
        public bool TryGetValue<T>(string s, ref T val)
        {
            return false;
        }
    }
    
    [TestFixture]
    public class ImageHeaderTests
    {
        [Test]
        public void NoArgConstruct()
        {
            ImageHeader h = new ImageHeader();
            
            Assert.AreEqual(0, h.Dimensions.Rows);                               
            Assert.AreEqual(0, h.Dimensions.Columns);                               
            Assert.AreEqual(System.TypeCode.Byte, h.BandType);
            Assert.AreEqual(0,h.BandSize);
            Assert.AreEqual(0, h.BandCount);
        }
        
        [Test]
        public void TypicalConstruct()
        {
            ImageHeader h = new ImageHeader(new Dimensions(10,20),
                                            System.TypeCode.Byte,
                                            0,
                                            null
                                           );
            Assert.AreEqual(10, h.Dimensions.Rows);                               
            Assert.AreEqual(20, h.Dimensions.Columns);                               
            Assert.AreEqual(System.TypeCode.Byte, h.BandType);
            Assert.AreEqual(1,h.BandSize);
            Assert.AreEqual(0, h.BandCount);
            Assert.AreEqual(null, h.Metadata);
        }
        
        [Test]
        public void GoodMetadata()
        {
            Metadata m = new Metadata();
            
            ImageHeader h = new ImageHeader(new Dimensions(10,20),
                                            System.TypeCode.Byte,
                                            0,
                                            m
                                           );
                               
            Assert.AreEqual(10, h.Dimensions.Rows);                               
            Assert.AreEqual(20, h.Dimensions.Columns);                               
            Assert.AreEqual(System.TypeCode.Byte, h.BandType);
            Assert.AreEqual(1,h.BandSize);
            Assert.AreEqual(0, h.BandCount);
            Assert.AreEqual(m, h.Metadata);
        }
        
        [Test]
        public void WrongMetadata()
        {
            ImageHeader h = new ImageHeader(new Dimensions(10,20),
                                            System.TypeCode.Byte,
                                            7,
                                            new FakeMetadata()
                                           );

            Assert.AreEqual(10, h.Dimensions.Rows);                               
            Assert.AreEqual(20, h.Dimensions.Columns);                               
            Assert.AreEqual(System.TypeCode.Byte, h.BandType);
            Assert.AreEqual(1,h.BandSize);
            Assert.AreEqual(7, h.BandCount);
            Assert.AreEqual(null, h.Metadata);
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void WrongBandType()
        {
            ImageHeader h = new ImageHeader(new Dimensions(10,20),
                                            System.TypeCode.UInt32,
                                            0,
                                            null
                                           );
        }
        
        [Test]
        public void WriteAndRead()
        {
            string filename = Data.MakeOutputPath("blark.out");
            
            ImageHeader w = new ImageHeader(new Dimensions(10,20),
                                            System.TypeCode.Byte,
                                            0,
                                            null
                                           );
                                           
            w.Write(filename);
            
            ImageHeader r = new ImageHeader();
            
            r.Read(filename);
            
            Assert.AreEqual(r.Dimensions.Rows, w.Dimensions.Rows);
            Assert.AreEqual(r.Dimensions.Columns, w.Dimensions.Columns);
            Assert.AreEqual(r.BandType, w.BandType);
            Assert.AreEqual(r.BandCount, w.BandCount);
            Assert.AreEqual(r.BandSize, w.BandSize);
        }
    }
}