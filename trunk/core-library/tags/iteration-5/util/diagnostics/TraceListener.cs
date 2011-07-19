using SysDiag = System.Diagnostics;

namespace Landis.Util.Diagnostics
{
	/// <summary>
	/// A TraceListener that throws a AssertException; intended to be used
	/// in NUnit test frameworks to test Debug.Assert and Trace.Assert.
	/// </summary>
	/// <remarks>
	/// <example>
	/// The following example shows how to a NUnit test framework would
	/// setup the list of TraceListeners for Debug to be a single instance
	/// of this custom TraceListener that throws an AssertException.
	/// <code>
	///	namespace My.Testing.Namespace
	///	{
	///		[TestFixture]
	///		public class MyTestsForSomeClassX
	///		{
	/// 		//  other instance members
	/// 		private TraceListener[] listeners;
	///	
	///			//---------------------------------------------------------------------
	///	
	///			[SetUp]
	///			public void Init()
	///			{
	///				//  initialize other members
	///	
	///				listeners = Landis.Util.Diagnostics.TraceListener.Copy(Debug.Listeners);
	///				Debug.Listeners.Clear();
	///				Debug.Listeners.Add(new Landis.Util.Diagnostics.TraceListener());
	///			}
	///	
	///			//---------------------------------------------------------------------
	///	
	///			[Test]
	/// 		[System.Diagnostics.Conditional("Debug")]
	///			[ExpectedException(typeof(Landis.Util.Diagnostics.AssertException))]
	///			public void SomeTest()
	///			{
	///				//  Call some method that call Debug.Assert
	///			}
	///	
	///			//---------------------------------------------------------------------
	///	
	///			[TearDown]
	///			public void Cleanup()
	///			{
	///				Debug.Listeners.Clear();
	///				Debug.Listeners.AddRange(listeners);
	/// 
	/// 			//  other cleanup code ...
	///			}
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// </remarks>
	public class TraceListener
		: SysDiag.TraceListener
	{
		public TraceListener()
			: base()
		{
		}

		//---------------------------------------------------------------------

		public override void Fail(string message)
		{
			throw new AssertException(message);
		}


		//---------------------------------------------------------------------

		public override void Fail(string message,
		                          string detailMessage)
		{
			Fail(message + "\n  " + detailMessage);
		}

		//---------------------------------------------------------------------

		public override void Write(string str)
		{
		}

		//---------------------------------------------------------------------

		public override void WriteLine(string str)
		{
		}

		//---------------------------------------------------------------------

		public static SysDiag.TraceListener[] Copy(SysDiag.TraceListenerCollection listeners)
		{
			SysDiag.TraceListener[] array = new SysDiag.TraceListener[listeners.Count];
			listeners.CopyTo(array, 0);
			return array;
		}
	}
}
