using Edu.Wisc.Forest.Flel.Util;
using System;

namespace Landis
{
	/// <summary>
	/// The string that unique identifies the type of data in a binary file.
	/// </summary>
	[Serializable]
	public class BinaryFileIdentifier
	{
		private string id;

		//---------------------------------------------------------------------

		///	<summary>
		/// Initializes a new instance.
		/// </summary>
		public BinaryFileIdentifier(string id)
		{
			Require.ArgumentNotNull(id);
			this.id = id;
		}

		//---------------------------------------------------------------------

		public override string ToString()
		{
			return id;
		}
	}
}
