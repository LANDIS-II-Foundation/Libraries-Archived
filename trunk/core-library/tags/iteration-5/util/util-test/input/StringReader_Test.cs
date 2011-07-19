using Landis.Util;
using NUnit.Framework;

namespace Landis.Test.Util
{
	[TestFixture]
	public class StringReader_Test
	{
		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void NullArgument()
		{
			StringReader reader = new StringReader(null);
		}

		//---------------------------------------------------------------------

		[Test]
		public void EmptyString()
		{
			StringReader reader = new StringReader("");
			Assert.AreEqual(0, reader.Index);
			Assert.AreEqual(-1, reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void NonEmptyString()
		{
			string str = "Hello World!";
			StringReader reader = new StringReader(str);
			int expectedIndex = 0;
			foreach (char expectedCh in str) {
				Assert.AreEqual(expectedIndex, reader.Index);
				int i = reader.Read();
				Assert.IsTrue(i != -1);
				Assert.AreEqual(expectedCh, (char) i);
				expectedIndex++;
			}
			Assert.AreEqual(expectedIndex, reader.Index);
			Assert.AreEqual(-1, reader.Peek());
		}

		//---------------------------------------------------------------------

		[Test]
		public void ReadBlock()
		{
			string str = "Four score and seven years ago ...";
			StringReader reader = new StringReader(str);
			char[] buffer = new char[str.Length];
			int blockSize = 5;

			for (int bufferIndex = 0; bufferIndex < buffer.Length; bufferIndex += blockSize) {
				Assert.AreEqual(bufferIndex, reader.Index);
				int countToRead;
				if (bufferIndex + blockSize > buffer.Length)
					countToRead = buffer.Length - bufferIndex;
				else
					countToRead = blockSize;
				Assert.AreEqual(countToRead, reader.Read(buffer, bufferIndex,
				                                         countToRead));
			}
			Assert.AreEqual(str.Length, reader.Index);
			Assert.AreEqual(-1, reader.Peek());
			Assert.AreEqual(str, new string(buffer));
		}
	}
}
