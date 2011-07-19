namespace Landis.Util
{
	/// <summary>
	/// Methods for testing assertions.
	/// </summary>
	public static class Require
	{
		/// <summary>
		/// Checks if an argument is not null.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		/// Argument is null.
		/// </exception>
		public static void ArgumentNotNull(object arg)
		{
			if (arg == null)
				throw new System.ArgumentNullException();
		}
	}
}
