using Landis.RasterIO;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;

namespace Landis.Test.RasterIO
{
	[TestFixture]
	public class PixelBandShort_Test
	{
		[Test]
		public void TypeCode()
		{
			PixelBandShort pixelBand = new PixelBandShort();
			Assert.AreEqual(System.TypeCode.Int16, pixelBand.TypeCode);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DefaultCtor()
		{
			PixelBandShort pixelBand = new PixelBandShort();
			Assert.AreEqual(0, pixelBand.Value);
		}

		//---------------------------------------------------------------------

		[Test]
		public void CtorWithValue()
		{
		    short initialValue = -678;
			PixelBandShort pixelBand = new PixelBandShort(initialValue);
			Assert.AreEqual(initialValue, pixelBand.Value);
		}

		//---------------------------------------------------------------------

		[Test]
		public void SetValue()
		{
			PixelBandShort pixelBand = new PixelBandShort();
		    short newValue = 12345;
			pixelBand.Value = newValue;
			Assert.AreEqual(newValue, pixelBand.Value);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetBytes()
		{
			byte[] bytes = new byte[]{ 1, 2 };
		    short newValue = System.BitConverter.ToInt16(bytes, 0);

			PixelBandShort pixelBand = new PixelBandShort();
			pixelBand.Value = newValue;
			Assert.AreEqual(newValue, pixelBand.Value);

			byte[] result = pixelBand.GetBytes();
			Assert.AreEqual(bytes.Length, result.Length);
			for (int i = 0; i < bytes.Length; ++i)
				Assert.AreEqual(bytes[i], result[i]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void SetBytes()
		{
			byte[] bytes = new byte[]{ 1, 2 };
		    short expectedValue = System.BitConverter.ToInt16(bytes, 0);

			PixelBandShort pixelBand = new PixelBandShort();
			pixelBand.SetBytes(bytes, 0);
			Assert.AreEqual(expectedValue, pixelBand.Value);
		}

		//---------------------------------------------------------------------

		[Test]
		public void SetThenGet()
		{
			byte[] bytes = new byte[]{ 255, 1 };
		    short expectedValue = System.BitConverter.ToInt16(bytes, 0);

			PixelBandShort pixelBand = new PixelBandShort();
			pixelBand.SetBytes(bytes, 0);
			Assert.AreEqual(expectedValue, pixelBand.Value);

			byte[] getResult = pixelBand.GetBytes();
			Assert.AreEqual(bytes.Length, getResult.Length);
			for (int i = 0; i < bytes.Length; ++i)
				Assert.AreEqual(bytes[i], getResult[i]);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void SetBytes_Null()
		{
			PixelBandShort pixelBand = new PixelBandShort();
			pixelBand.SetBytes(null, 0);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void SetBytes_OffsetNegative()
		{
			PixelBandShort pixelBand = new PixelBandShort();
			pixelBand.SetBytes(new byte[]{}, -1);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void SetBytes_OffsetTooBig()
		{
			PixelBandShort pixelBand = new PixelBandShort();
			pixelBand.SetBytes(new byte[]{1, 2}, 1);
		}
	}
}
