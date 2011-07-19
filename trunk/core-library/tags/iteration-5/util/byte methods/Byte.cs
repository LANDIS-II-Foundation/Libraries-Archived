using System;

namespace Landis.Util.ByteMethods
{
	public class Byte
		: IByteMethods<byte>
	{
		public Byte()
		{
		}

		//---------------------------------------------------------------------

		private byte[] MyToBytes(byte value)
		{
			byte[] result = new byte[] { value };
			return result;
		}

		//---------------------------------------------------------------------

		public ToBytesMethod<byte> ToBytes
		{
			get {
				return new ToBytesMethod<byte>(MyToBytes);
			}
		}

		//---------------------------------------------------------------------

		private byte MyFromBytes(byte[] value,
		                         int    startIndex)
		{
			return value[startIndex];
		}

		//---------------------------------------------------------------------

		public FromBytesMethod<byte> FromBytes
		{
			get {
				return new FromBytesMethod<byte>(MyFromBytes);
			}
		}
	}
}
