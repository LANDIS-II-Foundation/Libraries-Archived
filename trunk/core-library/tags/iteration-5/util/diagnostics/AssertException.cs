namespace Landis.Util.Diagnostics
{
	/// <summary>
	/// An exception that is thrown by TraceListener.
	/// </summary>
	public class AssertException
		: System.Exception
	{
		public AssertException(string message)
			: base(message)
		{
		}
	}
}
