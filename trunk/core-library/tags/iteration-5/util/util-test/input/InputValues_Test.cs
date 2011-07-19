using Landis.Util;
using NUnit.Framework;
using System;
using System.Globalization;

namespace Landis.Test.Util
{
	[TestFixture]
	public class InputValues_Test
	{
		//  Inner classes for testing GetReadMethod
		public class UnregisteredClass
		{
		}

		public class RegisteredClass
		{
			public readonly string Str;

			private RegisteredClass(string s)
			{
				Str = s;
			}

			public static RegisteredClass Parse(string s)
			{
				return new RegisteredClass(s);
			}
		}

		private ReadMethod<byte> byteReadMethod;
		private byte[] values;
		private string valuesAsStr;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			InputValues.Register<RegisteredClass>(RegisteredClass.Parse,
			                                      "Registered Class");

			byteReadMethod = InputValues.GetReadMethod<byte>();

			values = new byte[] { 78, 0, 255 };
			string[] valsAsStrs = Array.ConvertAll(values,
			                                       new Converter<byte, string>(Convert.ToString));
			valuesAsStr = string.Join(" ", valsAsStrs);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void GetReadMethod_UnregisteredType()
		{
			try {
				ReadMethod<UnregisteredClass> readMethod =
					InputValues.GetReadMethod<UnregisteredClass>();
			}
			catch (Exception exc) {
				System.Console.WriteLine(exc.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetReadMethod_RegisteredType()
		{
			ReadMethod<RegisteredClass> readMethod =
				InputValues.GetReadMethod<RegisteredClass>();
			string[] words = new string[] { "aardvark", "LKR555", "<-o->" };
			string separator = " \t ";
			StringReader reader = new StringReader(string.Join(separator, words));
			foreach (string word in words) {
				int index;
				InputValue<RegisteredClass> result = readMethod(reader, out index);
				Assert.AreEqual(word, result.Actual.Str);
				Assert.AreEqual(word, result.String);
				Assert.AreEqual(index + word.Length , reader.Index);
			}
		}

		//---------------------------------------------------------------------

		private void CheckForException(StringReader reader,
		                               string       expectedValue,
		                               int          indexAfterExpectedValue)
		{
			try {
				int index;
				InputValue<byte> b = byteReadMethod(reader, out index);
			}
			catch (InputValueException exc) {
				System.Console.WriteLine(exc.FullMessage);
				Assert.AreEqual(expectedValue, exc.Value);
				Assert.AreEqual(indexAfterExpectedValue, reader.Index);
				throw exc;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void GetReadMethod_Byte_EmptyString()
		{
			StringReader reader = new StringReader("");
			CheckForException(reader, null, 0);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void GetReadMethod_Byte_Whitespace()
		{
			string str = "   \t   \n\r ";
			StringReader reader = new StringReader(str);
			CheckForException(reader, null, str.Length);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void GetReadMethod_Byte_PlusOnly()
		{
			StringReader reader = new StringReader("+");
			CheckForException(reader, "+", 1);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void GetReadMethod_Byte_SignOnly()
		{
			StringReader reader = new StringReader("  -");
			CheckForException(reader, "-", 3);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void GetReadMethod_Byte_NoDigits()
		{
			StringReader reader = new StringReader("+A");
			CheckForException(reader, "+A", 2);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void GetReadMethod_Byte_ExtraChars()
		{
			StringReader reader = new StringReader("-3@");
			CheckForException(reader, "-3@", 3);
		}

		//---------------------------------------------------------------------

		private void CheckInputValue(StringReader     reader,
		                             InputValue<byte> expectedValue,
		                             int              expectedIndex)
		{
			int index;
			InputValue<byte> result = byteReadMethod(reader, out index);
			Assert.AreEqual(expectedValue.Actual, result.Actual);
			Assert.AreEqual(expectedValue.String, result.String);
			Assert.AreEqual(expectedIndex, reader.Index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetReadMethod_Byte_JustDigits()
		{
			StringReader reader = new StringReader("234");
			CheckInputValue(reader, new InputValue<byte>(234, "234"), 3);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetReadMethod_Byte_PlusDigits()
		{
			StringReader reader = new StringReader("+001");
			CheckInputValue(reader, new InputValue<byte>(1, "+001"), 4);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void GetReadMethod_Byte_MoreThanMax()
		{
			StringReader reader = new StringReader("256");
			CheckForException(reader, "256", 3);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void GetReadMethod_Byte_LessThanMin()
		{
			StringReader reader = new StringReader("-1");
			CheckForException(reader, "-1", 2);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetReadMethod_Byte_StringOfBytes()
		{
			StringReader reader = new StringReader(valuesAsStr);
			int prevIndex = -1;
			foreach (byte b in values) {
				int index;
				InputValue<byte> result = byteReadMethod(reader, out index);
				Assert.AreEqual(b, result.Actual);
				Assert.IsTrue(index > prevIndex);
				prevIndex = index;
			}
			Assert.AreEqual(-1, reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void GetReadMethod_Int_MoreThanMax()
		{
			try {
				StringReader reader = new StringReader(long.MaxValue.ToString());
				ReadMethod<int> readMethod = InputValues.GetReadMethod<int>();
				int index;
				InputValue<int> result = readMethod(reader, out index);
			}
			catch (InputValueException exc) {
				System.Console.WriteLine(exc.FullMessage);
				throw exc;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void GetReadMethod_Float_LessThanMin()
		{
			try {
				StringReader reader = new StringReader(double.MinValue.ToString());
				ReadMethod<float> readMethod = InputValues.GetReadMethod<float>();
				int index;
				InputValue<float> result = readMethod(reader, out index);
			}
			catch (InputValueException exc) {
				System.Console.WriteLine(exc.FullMessage);
				throw exc;
			}
		}
	}
}
