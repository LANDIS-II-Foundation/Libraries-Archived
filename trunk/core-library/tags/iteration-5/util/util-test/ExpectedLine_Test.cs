using Landis.Util;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Landis.Test.Util
{
	[TestFixture]
	public class ExpectedLine_Test
	{
		private List<ExpectedLine> ReadLines(string filename)
		{
			string path = Path.Combine(Data.Directory, filename);
			return ExpectedLine.ReadLines(path);
		}

		//---------------------------------------------------------------------

		[Test]
		public void EmptyFile()
		{
			List<ExpectedLine> lines = ReadLines("ExpectedLine_Empty.txt");
			Assert.AreEqual(0, lines.Count);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void NoLineNum()
		{
			try {
				List<ExpectedLine> lines = ReadLines(
												"ExpectedLine_NoLineNum.txt");
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void NoColon()
		{
			try {
				List<ExpectedLine> lines = ReadLines(
												"ExpectedLine_NoColon.txt");
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void LineNum0()
		{
			try {
				List<ExpectedLine> lines = ReadLines(
												"ExpectedLine_LineNum0.txt");
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void LessThanPrev()
		{
			try {
				List<ExpectedLine> lines = ReadLines(
											"ExpectedLine_LessThanPrev.txt");
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void SameAsPrev()
		{
			try {
				List<ExpectedLine> lines = ReadLines(
												"ExpectedLine_SameAsPrev.txt");
			}
			catch (System.Exception e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void GoodFile()
		{
			List<ExpectedLine> lines = ReadLines("ExpectedLine_GoodFile.txt");
			Assert.AreEqual(6, lines.Count);

			Assert.AreEqual(1, lines[0].Number);
			Assert.AreEqual("Foo", lines[0].Text);

			Assert.AreEqual(22, lines[1].Number);
			Assert.AreEqual("	The line below 3 tabs.", lines[1].Text);

			Assert.AreEqual(33, lines[2].Number);
			Assert.AreEqual("			", lines[2].Text);

			Assert.AreEqual(444, lines[3].Number);
			Assert.AreEqual("The line below has nothing after the colon.",
			                lines[3].Text);

			Assert.AreEqual(555, lines[4].Number);
			Assert.AreEqual("", lines[4].Text);

			Assert.AreEqual(6666, lines[5].Number);
			Assert.AreEqual("The last line in this file.", lines[5].Text);
		}
	}
}
