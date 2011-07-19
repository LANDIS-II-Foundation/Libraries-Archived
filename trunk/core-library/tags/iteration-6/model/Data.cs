using Landis.Util;
//using System.IO;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

namespace Landis
{
	/// <summary>
	/// Methods for working with Landis data.
	/// </summary>
	public static class Data
	{
		/// <summary>
		/// The name of the InputVariable that appears first in Landis text
		/// input.
		/// </summary>
		public const string InputVarName = "LandisData";

		//---------------------------------------------------------------------

		/// <summary>
		/// The markers for comments in Landis text data.
		/// </summary>
		public static class CommentMarkers
		{
			/// <summary>
			/// The marker for comment lines.
			/// </summary>
			public const string Line = ">";

			/// <summary>
			/// The marker for end-of-line comments.
			/// </summary>
			public const string EndOfLine = "<<";
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Opens a text file for reading.
		/// </summary>
		/// <remarks>
		/// The file is configured so that blank lines and comment lines are
		/// skipped, and end-of-line comments are trimmed.  The marker for
		/// comment lines is ">" while the marker for end-of-line comments is
		/// "&lt;&lt;".
		/// </remarks>
		public static FileLineReader OpenTextFile(string path)
		{
			Require.ArgumentNotNull(path);
			FileLineReader reader = new FileLineReader(path);
			reader.SkipBlankLines = true;

			reader.SkipCommentLines = true;
			reader.CommentLineMarker = CommentMarkers.Line;

			reader.TrimEndComments = true;
			reader.EndCommentMarker = CommentMarkers.EndOfLine;

			return reader;
		}

		//---------------------------------------------------------------------
		
#if LOAD_METHOD_ENABLED
		public static T Load<T>(string path)
			where T : ParameterSet, new()
		{
			T parmSet;
			if (/* path's extension is ".xml"? or ".landis" */ false) {
				//  Deserialize parameter set from the file:
				//  Binary serialization:
				IFormatter formatter = new BinaryFormatter();
				Stream stream = new FileStream(path, FileMode.Open,
 											   FileAccess.Read, FileShare.Read);
				parmSet = (T) formatter.Deserialize(stream);
				stream.Close();
			}
			else {
				parmSet = new T();
				//  Open the file specified by path
				Util.IStreamReader stream = new Util.FileStreamReader(path);
				stream.SkipBlankLines = true;
				stream.SkipCommentLines = true;
				stream.TrimEolComments = true;
				parmSet.ReadValues(stream);
			}
			return parmSet;
		}
#endif
	}
}
