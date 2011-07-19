using Landis.Util;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Landis.Test.Util
{
	[TestFixture]
	public class FullMessageException_Test
	{
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
		public void SetIndentNull()
		{
			FullMessageException.Indent = null;
		}

		//---------------------------------------------------------------------

		[Test]
		public void NoInnerException()
		{
			string myMessage = "Four score and seven years ago ...";
			FullMessageException exception = new FullMessageException(myMessage);

			Assert.IsNull(exception.InnerException);
			Assert.AreEqual(myMessage, exception.Message);
			Assert.AreEqual(1, exception.FullMessage.Count);
			Assert.AreEqual(myMessage, exception.FullMessage[0]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void NullInnerException()
		{
			string myMessage = "Four score and seven years ago ...";
			FullMessageException exception = new FullMessageException(myMessage, null);

			Assert.IsNull(exception.InnerException);
			Assert.AreEqual(myMessage, exception.Message);
			Assert.AreEqual(1, exception.FullMessage.Count);
			Assert.AreEqual(myMessage, exception.FullMessage[0]);
		}

		//---------------------------------------------------------------------

		private void Inner_NoFullMessage()
		{
			string innerMessage = "The quick brown fox";
			System.ApplicationException inner = new System.ApplicationException(innerMessage);

			string myMessage = "Four score and seven years ago ...";
			FullMessageException exception = new FullMessageException(myMessage,
			                                                          inner);

			Assert.AreEqual(inner, exception.InnerException);
			Assert.AreEqual(myMessage, exception.Message);
			Assert.AreEqual(2, exception.FullMessage.Count);
			Assert.AreEqual(myMessage + ":", exception.FullMessage[0]);
			Assert.AreEqual(FullMessageException.Indent + innerMessage,
			                exception.FullMessage[1]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void InnerHasNoFullMessage()
		{
			//  Use default indent
			Inner_NoFullMessage();
		}

		//---------------------------------------------------------------------

		[Test]
		public void InnerHasNoFullMessage_Indent()
		{
			FullMessageException.Indent = "\t";
			Inner_NoFullMessage();
			Assert.AreEqual("\t", FullMessageException.Indent);
		}

		//---------------------------------------------------------------------

		private void Inner_FullMessage()
		{
			string innerInnerMessage = "Damn it, Jim, I'm a doctor, not a bricklayer!";
			System.ApplicationException innerInner = new System.ApplicationException(innerInnerMessage);

			string innerMessage = "The quick brown fox";
			FullMessageException inner = new FullMessageException(innerMessage,
			                                                      innerInner);

			string myMessage = "Four score and seven years ago ...";
			FullMessageException exception = new FullMessageException(myMessage,
			                                                          inner);

			Assert.AreEqual(inner, exception.InnerException);
			Assert.AreEqual(myMessage, exception.Message);
			Assert.AreEqual(3, exception.FullMessage.Count);
			Assert.AreEqual(myMessage + ":", exception.FullMessage[0]);
			Assert.AreEqual(FullMessageException.Indent + innerMessage + ":",
			                exception.FullMessage[1]);
			Assert.AreEqual(FullMessageException.Indent +
			                FullMessageException.Indent + innerInnerMessage,
			                exception.FullMessage[2]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void InnerHasFullMessage()
		{
			//  Use default indent
			Inner_FullMessage();
		}

		//---------------------------------------------------------------------

		[Test]
		public void InnerHasFullMessage_Indent()
		{
			FullMessageException.Indent = ". . ";
			Inner_FullMessage();
			Assert.AreEqual(". . ", FullMessageException.Indent);
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
