namespace Landis.PlugIns
{
	/// <summary>
	/// Methods for plug-in interfaces
	/// </summary>
	public static class Interface
	{
		/// <summary>
		/// Gets the name of an plug-in interface.
		/// </summary>
		public static string GetName(System.Type plugInInterface)
		{
			if (plugInInterface == typeof(ISuccession))
				return "succession";
			if (plugInInterface == typeof(IDisturbance))
				return "disturbance";
			if (plugInInterface == typeof(IOutput))
				return "output";
			return "unknown";
		}
	}
}
