using Landis.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Util
{
	//  Simple null wrapper around base class for testing purposes
	internal class NullLineReader
		: LineReader
	{
		public override string SourceName
		{
			get {
				return null;
			}
		}

		public NullLineReader()
			: base()
		{
		}

		protected override string GetNextLine()
		{
			return null;
		}
	}

	//-------------------------------------------------------------------------

	[TestFixture]
	public class LineReader_Test
	{
		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void CommentLineMarker_Empty()
		{
			try {
				LineReader reader = new NullLineReader();
				reader.CommentLineMarker = "";
			}
			catch (System.ApplicationException e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void CommentLineMarker_1stCharWhitespace()
		{
			try {
				LineReader reader = new NullLineReader();
				reader.CommentLineMarker = " #";
			}
			catch (System.ApplicationException e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void EOLCommentMarker_Empty()
		{
			try {
				LineReader reader = new NullLineReader();
				reader.EOLCommentMarker = "";
			}
			catch (System.ApplicationException e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void EOLCommentMarker_1stCharWhitespace()
		{
			try {
				LineReader reader = new NullLineReader();
				reader.EOLCommentMarker = " //";
			}
			catch (System.ApplicationException e) {
				System.Console.WriteLine(e.Message);
				throw;
			}
		}
	}
}
