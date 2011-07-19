namespace Landis.Util
{
	/// <summary>
	/// Interface to an editable version of a class.
	/// </summary>
	public interface IEditable<T>
	{
		/// <summary>
		/// Has all the necessary data been specified so that a complete
		/// instance of T can be generated?
		/// </summary>
		bool IsComplete
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Converts an instance to a complete instance of T.
		/// </summary>
		/// <returns>
		/// null if IsComplete is false.
		/// </returns>
		T GetComplete();
	}
}
