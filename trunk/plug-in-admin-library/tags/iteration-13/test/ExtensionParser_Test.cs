using Edu.Wisc.Forest.Flel.Util;
using Edu.Wisc.Forest.Flel.Util.PlugIns;
using Landis.PlugIns;
using Landis.PlugIns.Admin;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Landis.Test.PlugIns.Admin
{
	[TestFixture]
	public class ExtensionParser_Test
	{
		private ExtensionParser parser;
		private LineReader reader;
		private bool nameInUse;

		//---------------------------------------------------------------------

		public bool IsNameInUse(string name)
		{
			return nameInUse;
		}

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			parser = new ExtensionParser(IsNameInUse);
		}

		//---------------------------------------------------------------------

		[SetUp]
		public void TestInit()
		{
			nameInUse = false;
		}

		//---------------------------------------------------------------------

		private FileLineReader OpenFile(string filename)
		{
			string path = System.IO.Path.Combine(Data.Directory, filename);
			return Landis.Data.OpenTextFile(path);
		}

		//---------------------------------------------------------------------

		private void TryParse(string filename,
		                      int    errorLineNum)
		{
			try {
				reader = OpenFile(filename);
				IDatasetEntry extension = parser.Parse(reader);
			}
			catch (System.Exception e) {
				Data.Output.WriteLine();
				Data.Output.WriteLine(e.Message.Replace(Data.Directory, Data.DirPlaceholder));
				LineReaderException lrExc = e as LineReaderException;
				if (lrExc != null)
					Assert.AreEqual(errorLineNum, lrExc.LineNumber);
				throw;
			}
			finally {
				reader.Close();
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Empty()
		{
			TryParse("empty.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void LandisData_WrongValue()
		{
			TryParse("LandisData-WrongValue.txt", 3);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Name_Missing()
		{
			TryParse("Name-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Name_Empty()
		{
			TryParse("Name-Empty.txt", 5);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Name_Whitespace()
		{
			TryParse("Name-Whitespace.txt", 5);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Name_InUse()
		{
			nameInUse = true;
			TryParse("Foo-Bar.txt", 5);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Description_Missing()
		{
			TryParse("Description-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Description_Empty()
		{
			TryParse("Description-Empty.txt", 6);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Description_Whitespace()
		{
			TryParse("Description-Whitespace.txt", 6);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void UserGuide_Missing()
		{
			TryParse("UserGuide-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void UserGuide_FileNotExists()
		{
			TryParse("UserGuide-FileNotExists.txt", 7);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Assembly_Missing()
		{
			TryParse("Assembly-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Assembly_Empty()
		{
			TryParse("Assembly-Empty.txt", 8);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Assembly_Whitespace()
		{
			TryParse("Assembly-Whitespace.txt", 8);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Assembly_NotExist()
		{
			TryParse("Assembly-NotExist.txt", 8);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Class_Missing()
		{
			TryParse("Class-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Class_Empty()
		{
			TryParse("Class-Empty.txt", 9);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Class_Whitespace()
		{
			TryParse("Class-Whitespace.txt", 9);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Class_NotExist()
		{
			TryParse("Class-NotExist.txt", 9);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Class_NotClass()
		{
			TryParse("Class-NotClass.txt", 9);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Class_NoLandisInterface()
		{
			TryParse("Class-NoLandisInterface.txt", 9);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void Library_NotExist()
		{
			TryParse("Library-NotExist.txt", 10);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void CoreVersion_Missing()
		{
			TryParse("CoreVersion-Missing.txt", LineReader.EndOfInput);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void CoreVersion_NoMinor()
		{
			TryParse("CoreVersion-NoMinor.txt", 10);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void CoreVersion_BadValue()
		{
			TryParse("CoreVersion-BadValue.txt", 10);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(LineReaderException))]
		public void CoreVersion_TooBig()
		{
			TryParse("CoreVersion-TooBig.txt", 10);
		}

		//---------------------------------------------------------------------

		private IDatasetEntry ParseFile(string filename)
		{
			reader = OpenFile(filename);
			IDatasetEntry extension = parser.Parse(reader);
			reader.Close();
			return extension;
		}

		//---------------------------------------------------------------------

		[Test]
		public void FooBar()
		{
			IDatasetEntry extension = ParseFile("Foo-Bar.txt");
			Assert.IsNotNull(extension);
			Assert.AreEqual("Foo Bar", extension.Name);
			Assert.AreEqual("Compute foo-bar metrics across landscape",
			                extension.Description);
			Assert.AreEqual(NormalizePath(Data.MakeInputPath("Foo-Bar_User-Guide.txt")),
			                NormalizePath(extension.UserGuidePath));
			Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBar",
			                extension.AssemblyName);
			Assert.AreEqual("Landis.Test.PlugIns.Admin.FooBarPlugIn",
			                extension.ClassName);
			Assert.AreEqual(new System.Version("5.0"),
			                extension.CoreVersion);
		}

		//---------------------------------------------------------------------

		private string NormalizePath(string path)
		{
			if (Path.DirectorySeparatorChar != '/')
				path = path.Replace('/', Path.DirectorySeparatorChar);
			return path;
		}
	}
}
