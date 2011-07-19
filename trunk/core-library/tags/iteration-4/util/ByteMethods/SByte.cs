using System;

namespace Landis.Util.ByteMethods
{
	public class SByte
		: IByteMethods<sbyte>
	{
		public SByte()
		{
		}

		//---------------------------------------------------------------------

		private byte[] MyToBytes(sbyte value)
		{
			byte[] result = new byte[] { (byte) value };
			return result;
		}

		//---------------------------------------------------------------------

		public ToBytesMethod<sbyte> ToBytes
		{
			get {
				return new ToBytesMethod<sbyte>(MyToBytes);
			}
		}

		//---------------------------------------------------------------------

		private sbyte MyFromBytes(byte[] value,
		                          int    startIndex)
		{
			return (sbyte) (value[startIndex]);
		}

		//---------------------------------------------------------------------

		public FromBytesMethod<sbyte> FromBytes
		{
			get {
				return new FromBytesMethod<sbyte>(MyFromBytes);
			}
		}
	}
}
