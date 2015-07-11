using Edu.Wisc.Forest.Flel.Util;

namespace Landis
{
	/// <summary>
	/// Editable plug-in and its initialization file.  The plug-in must
	/// support the interface (T).
	/// </summary>
	public class EditablePlugIn<T>
		: IEditablePlugIn<T>
		where T : Edu.Wisc.Forest.Flel.Util.PlugIns.IPlugIn
	{
		static EditablePlugIn()
		{
			PlugInInfo.RegisterReadMethod();
		}

		//---------------------------------------------------------------------

		private InputValue<Edu.Wisc.Forest.Flel.Util.PlugIns.Info> info;
		private InputValue<string> initFile;

		//---------------------------------------------------------------------

		public InputValue<Edu.Wisc.Forest.Flel.Util.PlugIns.Info> Info
		{
			get {
				return info;
			}

			set {
				if (value != null) {
					if (value.Actual.InterfaceType != typeof(T))
						throw new InputValueException(value.Actual.Name,
						                              "\"{0}\" is not {1} plug-in.",
						                              value.Actual.Name,
						                              String.PrependArticle(PlugIns.Interface.GetName(typeof(T))));
				}
				info = value;
			}
		}

		//---------------------------------------------------------------------

		public InputValue<string> InitFile
		{
			get {
				return initFile;
			}

			set {
				initFile = value;
			}
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				return (info != null) && (initFile != null);
			}
		}

		//---------------------------------------------------------------------

		public EditablePlugIn()
		{
			this.info = null;
			this.initFile = null;
		}

		//---------------------------------------------------------------------

		public IPlugIn GetComplete()
		{
			if (IsComplete)
				return new PlugIn(info.Actual, initFile.Actual);
			else
				return null;
		}
	}
}
