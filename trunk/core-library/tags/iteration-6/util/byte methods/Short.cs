using System;

namespace Landis.Util.ByteMethods
{
	public class Short
		: IByteMethods<short>
	{
		public Short()
		{
		}

		//---------------------------------------------------------------------

		public ToBytesMethod<short> ToBytes
		{
			get {
				return new ToBytesMethod<short>(BitConverter.GetBytes);
			}
		}

		//---------------------------------------------------------------------

		public FromBytesMethod<short> FromBytes
		{
			get {
				return new FromBytesMethod<short>(BitConverter.ToInt16);
			}
		}
	}
}
