namespace Landis.Util
{
	/// <summary>
	/// A method for converting a value into a sequence of bytes.
	/// </summary>
	public delegate byte[] ToBytesMethod<T>(T value);

	//-------------------------------------------------------------------------

	/// <summary>
	/// A method for converting a sequence of bytes into a value.
	/// </summary>
    public delegate T FromBytesMethod<T>(byte[] bytes,
				                         int    index);

	//-------------------------------------------------------------------------

	/// <summary>
	/// Set of methods for converting the values of a type into byte sequences.
	/// </summary>
	public interface IByteMethods<T>
	{
		ToBytesMethod<T> ToBytes
		{
			get;
		}

		FromBytesMethod<T> FromBytes
		{
			get;
		}
	}
}
