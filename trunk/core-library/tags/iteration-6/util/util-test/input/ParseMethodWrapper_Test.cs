using Landis.Util;
using NUnit.Framework;
using System;

namespace Landis.Test.Util
{
	[TestFixture]
	public class ParseMethodWrapper_Test
	{
		private ParseMethod<int> intParseMethod;
		private ParseMethodWrapper<int> intParseWrapper;

		private int[] intValues;
		private string intValuesAsStr;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			intParseMethod = new ParseMethod<int>(int.Parse);
			intParseWrapper = new ParseMethodWrapper<int>(intParseMethod);

			intValues = new int[] { -4, 78900, 0, 555 };
			string[] intsAsStrs = Array.ConvertAll(intValues,
			                                       new Converter<int, string>(Convert.ToString));
			intValuesAsStr = string.Join(" ", intsAsStrs);
		}

		//---------------------------------------------------------------------

		private void CheckForParseMethodWord(System.Exception exception)
		{
			try {
				System.Console.WriteLine("Word passed to parse method = \"{0}\"",
				                         exception.Data["ParseMethod.Word"]);
			}
			catch {
				//  Do nothing
			}
		}

		//---------------------------------------------------------------------

		private void CheckForException(StringReader reader,
		                               string       expectedValue,
		                               int          indexAfterExpectedValue)
		{
			try {
				int index;
				InputValue<int> i = intParseWrapper.Read(reader, out index);
			}
			catch (InputValueException exc) {
				System.Console.WriteLine(exc.Message);
				Assert.AreEqual(expectedValue, exc.Value);
				Assert.AreEqual(indexAfterExpectedValue, reader.Index);
				throw exc;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void IntParseWrapper_EmptyString()
		{
			StringReader reader = new StringReader("");
			CheckForException(reader, null, 0);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void IntParseWrapper_WhitespaceString()
		{
			string str = "   \t   \n\r ";
			StringReader reader = new StringReader(str);
			CheckForException(reader, null, str.Length);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void IntParseWrapper_PlusOnly()
		{
			StringReader reader = new StringReader("+");
			CheckForException(reader, "+", 1);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void IntParseWrapper_SignOnly()
		{
			StringReader reader = new StringReader(" \t  - ");
			CheckForException(reader, "-", 5);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void IntParseWrapper_NoDigits()
		{
			StringReader reader = new StringReader("+A");
			CheckForException(reader, "+A", 2);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputValueException))]
		public void IntParseWrapper_ExtraChars()
		{
			StringReader reader = new StringReader("-3@");
			CheckForException(reader, "-3@", 3);
		}

		//---------------------------------------------------------------------

		private void CheckInputValue(StringReader    reader,
		                             InputValue<int> expectedValue,
		                             int             expectedIndex)
		{
			int index;
			InputValue<int> result = intParseWrapper.Read(reader, out index);
			Assert.AreEqual(expectedValue.Actual, result.Actual);
			Assert.AreEqual(expectedValue.String, result.String);
			Assert.AreEqual(expectedIndex, reader.Index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntParseWrapper_JustDigits()
		{
			StringReader reader = new StringReader("1234");
			CheckInputValue(reader, new InputValue<int>(1234, "1234"), 4);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntParseWrapper_PlusDigits()
		{
			StringReader reader = new StringReader("+1234");
			CheckInputValue(reader, new InputValue<int>(1234, "+1234"), 5);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntParseWrapper_MinusDigits()
		{
			StringReader reader = new StringReader("-1234");
			CheckInputValue(reader, new InputValue<int>(-1234, "-1234"), 5);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntParseWrapper_LeadingWhiteSpace()
		{
			StringReader reader = new StringReader(" \t -1234");
			CheckInputValue(reader, new InputValue<int>(-1234, "-1234"), 8);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntParseWrapper_TrailingWhiteSpace()
		{
			StringReader reader = new StringReader("-1234 \n ");
			CheckInputValue(reader, new InputValue<int>(-1234, "-1234"), 5);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntParseWrapper_NumWithWhiteSpace()
		{
			StringReader reader = new StringReader(" \t -1234 \n ");
			CheckInputValue(reader, new InputValue<int>(-1234, "-1234"), 8);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntParseWrapper_StringOfInts()
		{
			StringReader reader = new StringReader(intValuesAsStr);
			int prevIndex = -1;
			foreach (int i in intValues) {
				int index;
				InputValue<int> result = intParseWrapper.Read(reader, out index);
				Assert.AreEqual(i, result.Actual);
				Assert.IsTrue(index > prevIndex);
				prevIndex = index;
			}
			Assert.AreEqual(-1, reader.Peek());
		}
	}
}
