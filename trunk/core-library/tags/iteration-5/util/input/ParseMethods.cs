namespace Landis.Util
{
	/// <summary>
	/// A method to parse a string for an input value of a specific type.
	/// </summary>
	public delegate T ParseMethod<T>(string str);

	//-------------------------------------------------------------------------

	/// <summary>
	/// A 2-parameter method to parse a string for an input value of a specific
	/// type.
	/// </summary>
	public delegate T ParseMethod2<T, Parameter2Type>(string         str,
	                                                  Parameter2Type parameter2);
}
