namespace Landis.PlugIn
{
	/// <summary>
	/// An exception related to plug-in components.
	/// </summary>
	public class Exception
		: System.ApplicationException
	{
		/// <summary>
		/// Create a new exception.
		/// </summary>
		/// <param name="name">Name of plug-in.</param>
		/// <param name="message">Message explaining the exception.</param>
		public Exception(string name,
		                 string message)
			: base(message)
		{
			StoreName(name);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Create a new exception.
		/// </summary>
		/// <param name="name">Name of plug-in.</param>
		/// <param name="message">Message explaining the exception.</param>
		/// <param name="exception">Inner exception that triggered the
		/// plug-in exception.</param>
		public Exception(string 		  name,
		                 string			  message,
		                 System.Exception exception)
			: base(message, exception)
		{
			StoreName(name);
		}

		//---------------------------------------------------------------------

		private void StoreName(string name)
		{
			Data["PlugIn.Name"] = name;
		}
	}

	//-------------------------------------------------------------------------

	/// <summary>
	/// An exception that occurs when a plug-in is not installed.
	/// </summary>
	public class NotInstalledException
		: Exception
	{
		/// <summary>
		/// Create a new exception.
		/// </summary>
		/// <param name="name">Name of plug-in.</param>
		public NotInstalledException(string name)
			: base(name, string.Format("Plug-in {0} is not installed", name))
		{
		}
	}

	//-------------------------------------------------------------------------

	/// <summary>
	/// Exception raised when a plug-in is not of an expected category.
	/// </summary>
	public class CategoryException
		: Exception
	{
		/// <summary>
		/// Create a new exception.
		/// </summary>
		/// <param name="name">Name of plug-in.</param>
		/// <param name="plugInType">plug-in's type.</param>
		/// <param name="category">The expected category of plug-in.</param>
		public CategoryException(string      name,
		                         System.Type plugInType,
		                         System.Type category)
			: base(name, string.Format("Plug-in {0}'s type ({1}) is not "
			                           + "derived from {2}", name,
			                           plugInType.FullName, category.FullName))
		{
			Data["PlugIn.Type"]     = plugInType;
			Data["PlugIn.Category"] = category;
		}
	}
}
