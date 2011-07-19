using Edu.Wisc.Forest.Flel.Util;
using Edu.Wisc.Forest.Flel.Util.PlugIns;

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Landis.PlugIns
{
	/// <summary>
	/// Manages plugable components.
	/// </summary>
	public static class Manager
	{
		private static IList<IInfo> plugInDatabase;

		//---------------------------------------------------------------------

		static Manager()
		{
			plugInDatabase = new List<IInfo>();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes the manager by loading plug-in information from a
		/// database.
		/// </summary>
		public static void Initialize(string path)
		{
			DatabaseParser parser = new DatabaseParser();
			plugInDatabase = Data.Load<IList<IInfo>>(path, parser);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes the manager without any plug-in information.
		/// </summary>
		public static void Initialize()
		{
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes the manager with a list of plug-in information.
		/// </summary>
		public static void Initialize(IList<IInfo> plugIns)
		{
			if (plugIns == null)
				plugInDatabase = new List<IInfo>();
			else
				plugInDatabase = new List<IInfo>(plugIns);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Adds a new plug-in to the database of plug-ins.
		/// </summary>
		/// <param name="newPlugIn">
		/// Information about the new plug-in to add.
		/// </param>
		/// <exception cref="Landis.PlugIn.Exception">
		/// There is already a plug-in in the database with the same name.
		/// </exception>
		public static void Add(IInfo newPlugIn)
		{
			foreach (IInfo info in plugInDatabase) {
				if (info.Name == newPlugIn.Name)
					throw new Exception(newPlugIn,
					                    "The plug-in database already has a plug-in with the name \"{0}\".",
					                    newPlugIn.Name);
			}
			plugInDatabase.Add(newPlugIn);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets the info for a specific plug-in.
		/// </summary>
		/// <param name="name">
		/// The plug-in's name.
		/// </param>
		/// <returns>
		/// null if there is no plug-in with the specified name.
		/// </returns>
		public static IInfo GetInfo(string name)
		{
			foreach (IInfo plugIn in plugInDatabase) {
				if (name == plugIn.Name)
					return plugIn;
			}
			return null;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets the info for a specific plug-in.
		/// </summary>
		/// <param name="name">
		/// The plug-in's name.
		/// </param>
		/// <param name="expectedInterface">
		/// The expected interface for the plug-in.
		/// </param>
		/// <returns>
		/// null if there is no plug-in with the specified name.
		/// </returns>
		/// <exception cref="Exception">
		/// The plug-in does not support the expected interface.
		/// </exception>
		public static IInfo GetInfo(string      name,
		                            System.Type expectedInterface)
		{
			IInfo plugIn = GetInfo(name);
			if (plugIn != null && plugIn.InterfaceType != expectedInterface)
				throw new Exception(plugIn,
				                    "{0} is {1} plug-in, not {2} plug-in.",
				                    name,
				                    ArticleAndName(plugIn.InterfaceType),
				                    ArticleAndName(expectedInterface));
			return plugIn;
		}

		//---------------------------------------------------------------------

		private static string ArticleAndName(System.Type interfaceType)
		{
			return String.PrependArticle(Interface.GetName(interfaceType));
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Loads a plug-in.
		/// </summary>
		public static T Load<T>(IInfo plugInInfo)
		{
			return Loader.Load<T>(plugInInfo);
		}
	}
}
