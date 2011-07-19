using Landis.Util;
using NUnit.Framework;
using System;

namespace Landis.Test.Util
{
	[TestFixture]
	public class InputVar_Double_Test
	{
		InputVar<double> doubleVar;
		private double[] values;
		private string valuesAsStr;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			doubleVar = new InputVar<double>("Double Input Var");

			values = new double[] { -4, 78900, 0, 555 };
			string[] valsAsStrs = Array.ConvertAll(values,
			                                       new Converter<double, string>(Convert.ToString));
			valuesAsStr = string.Join(" ", valsAsStrs);
		}

		//---------------------------------------------------------------------

		private void PrintInputVarException(string input)
		{
			try {
				StringReader reader = new StringReader(input);
				doubleVar.ReadValue(reader);
			}
			catch (InputVariableException exc) {
				System.Console.WriteLine(exc.FullMessage);
				throw exc;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputVariableException))]
		public void EmptyString()
		{
			PrintInputVarException("");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputVariableException))]
		public void Whitespace()
		{
			PrintInputVarException(" \t \r ");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputVariableException))]
		public void DecimalPt()
		{
			PrintInputVarException(".");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputVariableException))]
		public void JustExponent()
		{
			PrintInputVarException("e4");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputVariableException))]
		public void NoExponentDigits()
		{
			PrintInputVarException("-1.23e+");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(InputVariableException))]
		public void TooBig()
		{
			PrintInputVarException("9e+999");
		}

		//---------------------------------------------------------------------

		private void CheckReadResults(string 			 input,
		                              InputValue<double> expectedValue,
		                              int                expectedIndex)
		{
			StringReader reader = new StringReader(input);
			doubleVar.ReadValue(reader);
			Assert.AreEqual(expectedValue.Actual, doubleVar.Value.Actual);
			Assert.AreEqual(expectedValue.String, doubleVar.Value.String);
			Assert.AreEqual(expectedIndex, doubleVar.Index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void JustDigits()
		{
			CheckReadResults("1234",
			                 new InputValue<double>(1234, "1234"),
			                 0);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DigitsDecimalPt()
		{
			CheckReadResults("4.",
			                 new InputValue<double>(4, "4."),
			                 0);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DigitsDecimalPtExponent()
		{
			CheckReadResults("4.e3",
			                 new InputValue<double>(4.0e3, "4.e3"),
			                 0);
		}

		//---------------------------------------------------------------------

		[Test]
		public void DecimalPtDigits()
		{
			CheckReadResults(".78",
			                 new InputValue<double>(.78, ".78"),
			                 0);
		}

		//---------------------------------------------------------------------

		[Test]
		public void PlusDigits()
		{
			CheckReadResults("+1,234",
			                 new InputValue<double>(1234, "+1,234"),
			                 0);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MinusDigits()
		{
			CheckReadResults("-1234",
			                 new InputValue<double>(-1234, "-1234"),
			                 0);
		}

		//---------------------------------------------------------------------

		[Test]
		public void LeadingWhiteSpace()
		{
			CheckReadResults(" \t -1234",
			                 new InputValue<double>(-1234, "-1234"),
			                 3);
		}

		//---------------------------------------------------------------------

		[Test]
		public void TrailingWhiteSpace()
		{
			CheckReadResults("-1234 \n ",
			                 new InputValue<double>(-1234, "-1234"),
			                 0);
		}

		//---------------------------------------------------------------------

		[Test]
		public void NumWithWhiteSpace()
		{
			CheckReadResults(" \t -1,234 \n ",
			                 new InputValue<double>(-1234, "-1,234"),
			                 3);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Thousands()
		{
			CheckReadResults("12,345,678",
			                 new InputValue<double>(12345678, "12,345,678"),
			                 0);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ThousandsDecimalPtExponent()
		{
			CheckReadResults("12,345.67e+02",
			                 new InputValue<double>(12345.67e+2, "12,345.67e+02"),
			                 0);
		}

		//---------------------------------------------------------------------

		[Test]
		public void StringOfDoubles()
		{
			StringReader reader = new StringReader(valuesAsStr);
			foreach (double d in values) {
				doubleVar.ReadValue(reader);
				Assert.AreEqual(d, doubleVar.Value.Actual);
			}
		}
	}
}
