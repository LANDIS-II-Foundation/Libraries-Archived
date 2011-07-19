using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Landis;

namespace Landis.PlugIn
{
	/// <summary>
	/// Manages pluggable components.
	/// </summary>
	public static class Manager
	{
		/// <exclude/>
		/// <summary>
		/// Loads a plug-in, given its name.
		/// name is the AssemblyQualifiedName of the plug-in's type.
		/// </summary>
		public static T Load<T>(Info info)
		{
			Type plugInType;
			try {
				plugInType = Type.GetType(info.TypeName);
			}
			catch (System.Exception e) {
				throw new Exception(info.TypeName, "GetType error", e);
			}
			if (plugInType == null)
				throw new NotInstalledException(info.TypeName);
			try {
				Assembly assembly = plugInType.Assembly;
				T plugIn = (T) assembly.CreateInstance(plugInType.FullName);
				Debug.Assert( (object) plugIn != null );
				return plugIn;
			}
			catch (System.InvalidCastException) {
				throw new CategoryException(info.TypeName, plugInType, typeof(T));
			}
			catch (System.Exception e) {
				throw new Exception(info.TypeName, "CreateInstance error", e);
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Unloads all the plug-in components that have been loaded.
		/// </summary>
		public static void UnloadAll()
		{
			// foreach of the plug-ins that have been loaded,
			// call their Unload method (which should call their Dispose
			// method)
		}

		//---------------------------------------------------------------------
/*
		public void LoadPlugIns(string assemblyName)
		{
			Assembly assembly = Assembly.Load(assemblyName);
			successionPlugIns.AddRange(
								LoadPlugIns<Succession.PlugIn>(assembly,
			                                                   "Succession"));
		}

		//---------------------------------------------------------------------

		public List<T> LoadPlugIns<T>(Assembly assembly,
		                              string   plugInTypeDesc)
		{
			List<T> plugIns = new List<T>();
			Type plugInType = typeof(T);
			Type[] types = assembly.GetExportedTypes();
			foreach (Type type in types) {
				if (type.IsSubclassOf(plugInType)) {
					Console.WriteLine("  {0} plug-in: {1}", plugInTypeDesc,
					                  type.FullName);
					T plugIn = (T) assembly.CreateInstance(type.FullName);
					Debug.Assert( (object) plugIn != null );
					plugIns.Add(plugIn);
				}
			}
			return plugIns;
		}
*/
	}
}
