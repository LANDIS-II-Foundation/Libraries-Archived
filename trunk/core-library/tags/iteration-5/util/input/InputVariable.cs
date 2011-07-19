namespace Landis.Util
{
	/// <summary>
	/// An input variable whose value is read from a string or text file.
	/// </summary>
	public abstract class InputVariable
	{
		private string name;

		//---------------------------------------------------------------------

		public string Name
		{
			get {
				return name;
			}
		}

		//---------------------------------------------------------------------

		public InputVariable(string name)
		{
			this.name = name;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Read the variable's value from a string reader.
		/// </summary>
		public abstract void ReadValue(StringReader reader);
	}
}
