using Landis.Util;
using NUnit.Framework;

namespace Landis.Test.Util
{
	[TestFixture]
	public class ByteMethods_Test
	{
		[Test]
		public void Byte()
		{
			IByteMethods<byte> methods =
										new Landis.Util.ByteMethods.Byte();
			byte origValue = (byte) 123;
			byte[] bytes = methods.ToBytes(origValue);
			Assert.AreEqual(bytes.Length, sizeof(byte));
			Assert.AreEqual(origValue, bytes[0]);

			byte fromMthdResult = methods.FromBytes(bytes, 0);
			Assert.AreEqual(origValue, fromMthdResult);
		}

		//---------------------------------------------------------------------

		[Test]
		public void SByte()
		{
			IByteMethods<sbyte> methods =
										new Landis.Util.ByteMethods.SByte();
			sbyte origValue = (sbyte) -123;
			byte[] bytes = methods.ToBytes(origValue);
			Assert.AreEqual(bytes.Length, sizeof(sbyte));

			sbyte fromMthdResult = methods.FromBytes(bytes, 0);
			Assert.AreEqual(origValue, fromMthdResult);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Short()
		{
			IByteMethods<short> methods =
										new Landis.Util.ByteMethods.Short();
			short origValue = (short) -12345;
			byte[] bytes = methods.ToBytes(origValue);
			Assert.AreEqual(bytes.Length, sizeof(short));

			short fromMthdResult = methods.FromBytes(bytes, 0);
			Assert.AreEqual(origValue, fromMthdResult);
		}

		//---------------------------------------------------------------------

		[Test]
		public void UShort()
		{
			IByteMethods<ushort> methods =
										new Landis.Util.ByteMethods.UShort();
			ushort origValue = (ushort) 12345;
			byte[] bytes = methods.ToBytes(origValue);
			Assert.AreEqual(bytes.Length, sizeof(ushort));

			ushort fromMthdResult = methods.FromBytes(bytes, 0);
			Assert.AreEqual(origValue, fromMthdResult);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Float()
		{
			IByteMethods<float> methods =
										new Landis.Util.ByteMethods.Float();
			float origValue = (float) -9.876e5;
			byte[] bytes = methods.ToBytes(origValue);
			Assert.AreEqual(bytes.Length, sizeof(float));

			float fromMthdResult = methods.FromBytes(bytes, 0);
			Assert.AreEqual(origValue, fromMthdResult);
		}
	}
}
