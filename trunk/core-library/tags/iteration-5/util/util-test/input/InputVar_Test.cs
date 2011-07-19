using Landis.Util;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace Landis.Test.Util
{
	[TestFixture]
	public class InputVar_Test
	{
		//  Inner classes for testing GetReadMethod
		public class UnregisteredClass
		{
		}

		public class RegisteredClass
		{
			public readonly string Str;

			private RegisteredClass(string s)
			{
				Str = s;
			}

			public static RegisteredClass Parse(string s)
			{
				return new RegisteredClass(s);
			}
		}

		private TraceListener[] listeners;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			listeners = Landis.Util.Diagnostics.TraceListener.Copy(Debug.Listeners);
			Debug.Listeners.Clear();
			Debug.Listeners.Add(new Landis.Util.Diagnostics.TraceListener());
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(Landis.Util.Diagnostics.AssertException))]
		public void SetReadMethodNull()
		{
			InputVar<int> var = new InputVar<int>("var");
			var.ReadMethod = null;
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void GetReadMethod_UnregisteredType()
		{
			try {
				ReadMethod<UnregisteredClass> readMethod =
					InputValues.GetReadMethod<UnregisteredClass>();
			}
			catch (Exception exc) {
				System.Console.WriteLine(exc.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[TearDown]
		public void Cleanup()
		{
			Debug.Listeners.Clear();
			Debug.Listeners.AddRange(listeners);
		}
	}
}
