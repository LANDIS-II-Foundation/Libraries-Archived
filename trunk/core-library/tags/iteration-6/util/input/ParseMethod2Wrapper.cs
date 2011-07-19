using System.Globalization;

namespace Landis.Util
{
	/// <summary>
	/// Wrapper for a ParseMethod2 so it can used as a ParseMethod.
	/// </summary>
	public class ParseMethod2Wrapper<T, Parameter2Type>
	{
		private ParseMethod2<T, Parameter2Type> parseMethod;
		private Parameter2Type parameter2;

		//---------------------------------------------------------------------

		public ParseMethod2Wrapper(ParseMethod2<T, Parameter2Type> method,
		                           Parameter2Type                  parameter2)
		{
			this.parseMethod = method;
			this.parameter2 = parameter2;
		}

		//---------------------------------------------------------------------

		public T Parse(string s)
		{
			return parseMethod(s, parameter2);
		}
	}
}
