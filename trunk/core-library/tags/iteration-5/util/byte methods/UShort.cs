using System;

namespace Landis.Util.ByteMethods
{
	public class UShort
		: IByteMethods<ushort>
	{
		public UShort()
		{
		}

		//---------------------------------------------------------------------

		public ToBytesMethod<ushort> ToBytes
		{
			get {
				return new ToBytesMethod<ushort>(BitConverter.GetBytes);
			}
		}

		//---------------------------------------------------------------------

		public FromBytesMethod<ushort> FromBytes
		{
			get {
				return new FromBytesMethod<ushort>(BitConverter.ToUInt16);
			}
		}
	}
}
