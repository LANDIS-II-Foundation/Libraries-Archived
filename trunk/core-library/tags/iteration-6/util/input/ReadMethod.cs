namespace Landis.Util
{
	public delegate InputValue<T> ReadMethod<T>(StringReader reader,
	                                            out int      index);
}
