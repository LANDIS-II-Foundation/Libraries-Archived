using Edu.Wisc.Forest.Flel.Util;

namespace Landis
{
	/// <summary>
	/// Editable plug-in and its initialization file.  The plug-in must
	/// support the interface (T).
	/// </summary>
	public interface IEditablePlugIn<T>
		: IEditable<IPlugIn>
		where T : Edu.Wisc.Forest.Flel.Util.PlugIns.IPlugIn
	{
		/// <summary>
		/// Information about the plug-in.
		/// </summary>
		/// <exception cref="System.ArgumentException">
		/// Tried to assign plug-in information with an interface type other
		/// than T.
		/// </exception>
		InputValue<Edu.Wisc.Forest.Flel.Util.PlugIns.Info> Info
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The path to the data file to initialize the plug-in with.
		/// </summary>
		InputValue<string> InitFile
		{
			get;
			set;
		}
	}
}
