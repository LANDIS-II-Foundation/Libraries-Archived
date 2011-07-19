using Edu.Wisc.Forest.Flel.Util;

namespace Landis
{
	/// <summary>
	/// Methods for plug-in information.
	/// </summary>
	public static class PlugInInfo
	{
		/// <summary>
		/// Reads a plug-in name from a text reader and returns the
		/// information for the plug-in.
		/// </summary>
		public static InputValue<Edu.Wisc.Forest.Flel.Util.PlugIns.Info> Read(StringReader reader,
	                                                                          out int      index)
		{
			ReadMethod<string> strReadMethod = InputValues.GetReadMethod<string>();
			InputValue<string> name = strReadMethod(reader, out index);
			if (name.Actual.Trim(null) == "")
				throw new InputValueException(name.Actual,
				                              name.String + " is not a valid plug-in name.");
			Edu.Wisc.Forest.Flel.Util.PlugIns.Info info = (Edu.Wisc.Forest.Flel.Util.PlugIns.Info) PlugIns.Manager.GetInfo(name.Actual);
			if (info == null)
				throw new InputValueException(name.Actual,
				                              "No plug-in with the name \"{0}\".",
				                              name.Actual);
			return new InputValue<Edu.Wisc.Forest.Flel.Util.PlugIns.Info>(info, name.Actual);
		}

		//---------------------------------------------------------------------

		private static bool registered = false;

		//---------------------------------------------------------------------

		public static void RegisterReadMethod()
		{
			if (! registered) {
				Type.SetDescription<Edu.Wisc.Forest.Flel.Util.PlugIns.Info>("plug-in name");
				InputValues.Register<Edu.Wisc.Forest.Flel.Util.PlugIns.Info>(Read);
				registered = true;
			}
		}
	}
}
