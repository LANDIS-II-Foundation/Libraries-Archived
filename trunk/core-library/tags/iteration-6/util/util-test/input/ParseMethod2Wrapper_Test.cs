using Landis.Util;
using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;

namespace Landis.Test.Util
{
	[TestFixture]
	public class ParseMethod2Wrapper_Test
	{
		private ParseMethod2Wrapper<int, NumberStyles> intStyleWrapper;
		private ParseMethod2Wrapper<float, NumberStyles> floatStyleWrapper;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			NumberStyles intStyle = NumberStyles.Integer |
									NumberStyles.AllowThousands;
			intStyleWrapper = new ParseMethod2Wrapper<int, NumberStyles>(int.Parse,
			                                                             intStyle);
			NumberStyles floatStyle = NumberStyles.Float |
									  NumberStyles.AllowThousands;
			floatStyleWrapper = new ParseMethod2Wrapper<float, NumberStyles>(float.Parse,
			                                                                 floatStyle);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void IntStyleWrapper_EmptyString()
		{
			int i = intStyleWrapper.Parse("");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void IntStyleWrapper_Whitespace()
		{
			int i = intStyleWrapper.Parse("  \t  \n\r ");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void IntStyleWrapper_PlusOnly()
		{
			int i = intStyleWrapper.Parse("+");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void IntStyleWrapper_NoDigits()
		{
			int i = intStyleWrapper.Parse("+A");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void IntStyleWrapper_ExtraChars()
		{
			int i = intStyleWrapper.Parse("-3@");
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntStyleWrapper_JustDigits()
		{
			Assert.AreEqual(1234, intStyleWrapper.Parse("1234"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntStyleWrapper_PlusDigits()
		{
			Assert.AreEqual(1234, intStyleWrapper.Parse("+1234"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntStyleWrapper_MinusDigits()
		{
			Assert.AreEqual(-1234, intStyleWrapper.Parse("-1234"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntStyleWrapper_LeadingWhiteSpace()
		{
			Assert.AreEqual(-1234, intStyleWrapper.Parse(" \t -1234"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntStyleWrapper_TrailingWhiteSpace()
		{
			Assert.AreEqual(-1234, intStyleWrapper.Parse("-1234 \n "));
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntStyleWrapper_NumWithWhiteSpace()
		{
			Assert.AreEqual(-1234, intStyleWrapper.Parse(" \t -1234 \n "));
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntStyleWrapper_Commas()
		{
			Assert.AreEqual(123456789, intStyleWrapper.Parse("123,456,789"));
		}

		//---------------------------------------------------------------------
		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void FloatStyleWrapper_EmptyString()
		{
			float f = floatStyleWrapper.Parse("");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void FloatStyleWrapper_Whitespace()
		{
			float f = floatStyleWrapper.Parse("  \t  \n\r ");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void FloatStyleWrapper_PlusOnly()
		{
			float f = floatStyleWrapper.Parse("+");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void FloatStyleWrapper_NoDigits()
		{
			float f = floatStyleWrapper.Parse("+A");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.FormatException))]
		public void FloatStyleWrapper_ExtraChars()
		{
			float f = floatStyleWrapper.Parse("-3@");
		}

		//---------------------------------------------------------------------

		[Test]
		public void FloatStyleWrapper_JustDigits()
		{
			Assert.AreEqual(1234, floatStyleWrapper.Parse("1234"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void FloatStyleWrapper_PlusDigits()
		{
			Assert.AreEqual(1234, floatStyleWrapper.Parse("+1234"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void FloatStyleWrapper_MinusDigits()
		{
			Assert.AreEqual(-1234, floatStyleWrapper.Parse("-1234"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void FloatStyleWrapper_LeadingWhiteSpace()
		{
			Assert.AreEqual(-1234, floatStyleWrapper.Parse(" \t -1234"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void FloatStyleWrapper_TrailingWhiteSpace()
		{
			Assert.AreEqual(-1234, floatStyleWrapper.Parse("-1234 \n "));
		}

		//---------------------------------------------------------------------

		[Test]
		public void FloatStyleWrapper_NumWithWhiteSpace()
		{
			Assert.AreEqual(-1234, floatStyleWrapper.Parse(" \t -1234 \n "));
		}

		//---------------------------------------------------------------------

		[Test]
		public void FloatStyleWrapper_Commas()
		{
			Assert.AreEqual(123456.789f, floatStyleWrapper.Parse("123,456.789"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void FloatStyleWrapper_Exponent()
		{
			Assert.AreEqual(1, floatStyleWrapper.Parse("100,000e-5"));
		}

		//---------------------------------------------------------------------

		[Test]
		public void FloatStyleWrapper_DecimalPtDigits()
		{
			Assert.AreEqual(.01234f, floatStyleWrapper.Parse("  .01234 "));
		}

		//---------------------------------------------------------------------

		[Test]
		public void FloatStyleWrapper_DecimalPtExponent()
		{
			Assert.AreEqual(12.345e-11f, floatStyleWrapper.Parse("  12.345e-11 "));
		}
	}
}
