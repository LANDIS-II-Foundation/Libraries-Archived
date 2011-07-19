using System.IO;

namespace Landis.Util
{
	public class FileLineReader
		: LineReader
	{
		private string path;
		private StreamReader reader;

		//---------------------------------------------------------------------

		public string Path
		{
			get {
				return path;
			}
		}

		//---------------------------------------------------------------------

		public override string SourceName
		{
			get {
				return "file \"" + path + "\"";
			}
		}

		//---------------------------------------------------------------------

		public FileLineReader(string path)
			: base()
		{
			this.path = path;
			this.reader = new StreamReader(path);
		}

		//---------------------------------------------------------------------

		protected override string GetNextLine()
		{
			return reader.ReadLine();
		}

		//---------------------------------------------------------------------

		public override void Close()
		{
			base.Close();
			if (reader != null) {
				reader.Close();
				reader = null;
			}
		}
	}
}
