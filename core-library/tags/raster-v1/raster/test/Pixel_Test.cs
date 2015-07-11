using Edu.Wisc.Forest.Flel.Util;
using Landis.Raster;
using NUnit.Framework;
using System;

namespace Landis.Test.Raster
{
	[TestFixture]
	public class Pixel_Test
	{
		//  Pixel class for testing exceptions in SetBands method
		public class FakePixel
			: Pixel
		{
			public FakePixel(IPixelBand          band0,
			                 params IPixelBand[] otherBands)
			{
				SetBands(band0, otherBands);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Band0Null()
		{
			FakePixel pixel = new FakePixel(null);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Band1Null()
		{
			FakePixel pixel = new FakePixel(null, null);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Band2Null()
		{
			PixelBandByte band = new PixelBandByte();
			FakePixel pixel = new FakePixel(band, band, null, band);
		}

		//---------------------------------------------------------------------

		public class MyPixel
			: Pixel
		{
			private PixelBandByte   band0;
			private PixelBandShort  band1;
			private PixelBandDouble band2;

			public byte Band0
			{
				get {
					return band0;
				}
				set {
					band0.Value = value;
				}
			}

			public short Band1
			{
				get {
					return band1;
				}
				set {
					band1.Value = value;
				}
			}

			public double Band2
			{
				get {
					return band2;
				}
				set {
					band2.Value = value;
				}
			}

			private void InitializeBands()
			{
				band0 = new PixelBandByte();
				band1 = new PixelBandShort();
				band2 = new PixelBandDouble();
				SetBands(band0, band1, band2);
			}

			public MyPixel()
			{
				InitializeBands();
			}

			public MyPixel(byte   band0,
			               short  band1,
			               double band2)
			{
				InitializeBands();
				Band0 = band0;
				Band1 = band1;
				Band2 = band2;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void DefaultCtor()
		{
			MyPixel pixel = new MyPixel();
			Assert.AreEqual(3, pixel.BandCount);

			Assert.AreEqual(System.TypeCode.Byte,   pixel[0].TypeCode);
			Assert.AreEqual(System.TypeCode.Int16,  pixel[1].TypeCode);
			Assert.AreEqual(System.TypeCode.Double, pixel[2].TypeCode);

			Assert.AreEqual(0, pixel.Band0);
			Assert.AreEqual(0, pixel.Band1);
			Assert.AreEqual(0, pixel.Band2);
		}

		//---------------------------------------------------------------------

		[Test]
		public void CtorWithArgs()
		{
			byte band0 = 200;
			short band1 = -4321;
			double band2 = 1e65;
			MyPixel pixel = new MyPixel(band0, band1, band2);
			Assert.AreEqual(3, pixel.BandCount);

			Assert.AreEqual(System.TypeCode.Byte,   pixel[0].TypeCode);
			Assert.AreEqual(System.TypeCode.Int16,  pixel[1].TypeCode);
			Assert.AreEqual(System.TypeCode.Double, pixel[2].TypeCode);

			Assert.AreEqual(band0, pixel.Band0);
			Assert.AreEqual(band1, pixel.Band1);
			Assert.AreEqual(band2, pixel.Band2);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void BandIndex_Negative()
		{
			MyPixel pixel = new MyPixel();
			IPixelBand band = pixel[-1];
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void BandIndex_TooBig()
		{
			MyPixel pixel = new MyPixel();
			IPixelBand band = pixel[pixel.BandCount];
		}
	}
}
