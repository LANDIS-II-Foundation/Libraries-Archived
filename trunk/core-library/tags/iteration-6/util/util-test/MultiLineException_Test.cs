using Landis.Util;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Landis.Test.Util
{
	[TestFixture]
	public class MultiLineException_Test
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
			MultiLineException.Indent = null;
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void NullMessage()
		{
			MultiLineException exception = new MultiLineException(null);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void NullMessageWithInnerMessage()
		{
			MultiLineException exception = new MultiLineException(null, "");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void NullMessageAndInnerException()
		{
			System.Exception nullException = null;
			MultiLineException exception = new MultiLineException(null, nullException);
		}

		//---------------------------------------------------------------------

		[Test]
		public void JustMessage()
		{
			string myMessage = "Four score and seven years ago ...";
			MultiLineException exception = new MultiLineException(myMessage);

			Assert.IsNull(exception.InnerException);
			Assert.AreEqual(myMessage, exception.Message);
			Assert.AreEqual(1, exception.MultiLineMessage.Count);
			Assert.AreEqual(myMessage, exception.MultiLineMessage[0]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void MessageAndInner()
		{
			string myMessage = "Four score and seven years ago ...";
			string innerMessage = "The quick brown fox";
			MultiLineException exception = new MultiLineException(myMessage,
			                                                      innerMessage);

			Assert.IsNull(exception.InnerException);
			Assert.AreEqual(2, exception.MultiLineMessage.Count);
			Assert.AreEqual(myMessage + ":", exception.MultiLineMessage[0]);
			Assert.AreEqual(MultiLineException.Indent + innerMessage,
			                exception.MultiLineMessage[1]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void NullInnerException()
		{
			string myMessage = "Four score and seven years ago ...";
			System.Exception nullException = null;
			MultiLineException exception = new MultiLineException(myMessage, nullException);

			Assert.IsNull(exception.InnerException);
			Assert.AreEqual(myMessage, exception.Message);
			Assert.AreEqual(1, exception.MultiLineMessage.Count);
			Assert.AreEqual(myMessage, exception.MultiLineMessage[0]);
		}

		//---------------------------------------------------------------------

		private void Inner_NoMultiLineMessage()
		{
			string innerMessage = "The quick brown fox";
			System.ApplicationException inner = new System.ApplicationException(innerMessage);

			string myMessage = "Four score and seven years ago ...";
			MultiLineException exception = new MultiLineException(myMessage,
			                                                      inner);

			Assert.AreEqual(inner, exception.InnerException);
			Assert.AreEqual(2, exception.MultiLineMessage.Count);
			Assert.AreEqual(myMessage + ":", exception.MultiLineMessage[0]);
			Assert.AreEqual(MultiLineException.Indent + innerMessage,
			                exception.MultiLineMessage[1]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void InnerHasNoMultiLineMessage()
		{
			//  Use default indent
			Inner_NoMultiLineMessage();
		}

		//---------------------------------------------------------------------

		[Test]
		public void InnerHasNoMultiLineMessage_Indent()
		{
			MultiLineException.Indent = "\t";
			Inner_NoMultiLineMessage();
			Assert.AreEqual("\t", MultiLineException.Indent);
		}

		//---------------------------------------------------------------------

		private void Inner_MultiLineMessage()
		{
			string innerInnerMessage = "Damn it, Jim, I'm a doctor, not a bricklayer!";
			System.ApplicationException innerInner = new System.ApplicationException(innerInnerMessage);

			string innerMessage = "The quick brown fox";
			MultiLineException inner = new MultiLineException(innerMessage,
		                                                      innerInner);

			string myMessage = "Four score and seven years ago ...";
			MultiLineException exception = new MultiLineException(myMessage,
		                                                          inner);

			Assert.AreEqual(inner, exception.InnerException);
			Assert.AreEqual(3, exception.MultiLineMessage.Count);
			Assert.AreEqual(myMessage + ":", exception.MultiLineMessage[0]);
			Assert.AreEqual(MultiLineException.Indent + innerMessage + ":",
			                exception.MultiLineMessage[1]);
			Assert.AreEqual(MultiLineException.Indent +
			                MultiLineException.Indent + innerInnerMessage,
			                exception.MultiLineMessage[2]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void InnerHasMultiLineMessage()
		{
			//  Use default indent
			Inner_MultiLineMessage();
		}

		//---------------------------------------------------------------------

		[Test]
		public void InnerHasMultiLineMessage_Indent()
		{
			MultiLineException.Indent = ". . ";
			Inner_MultiLineMessage();
			Assert.AreEqual(". . ", MultiLineException.Indent);
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
