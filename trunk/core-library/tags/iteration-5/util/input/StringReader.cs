namespace Landis.Util
{
	public class StringReader
		: System.IO.StringReader
	{
		private int index;

		//---------------------------------------------------------------------

		/// <summary>
		/// Index (position) of the next character that will be read.
		/// </summary>
		public int Index
		{
			get {
				return index;
			}
		}

		//---------------------------------------------------------------------

		public StringReader(string str)
			: base(str)
		{
			index = 0;
		}

		//---------------------------------------------------------------------

		public override int Read()
		{
			int result = base.Read();
			if (result != -1)
				index++;
			return result;
		}

		//---------------------------------------------------------------------

		public override int Read(char[] buffer,
		                         int    index,
		                         int    count)
		{
			int countRead = base.Read(buffer, index, count);
			this.index += countRead;
			return countRead;
		}
	}
}
