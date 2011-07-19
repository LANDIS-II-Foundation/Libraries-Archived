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
	public class Manager
	{
		/// <summary>
		/// Create a new plug-in manager.
		/// </summary>
		public Manager()
		{
		}

		//---------------------------------------------------------------------

		/// <exclude/>
		/// <summary>
		/// Load a plug-in, given its name.
		/// name is the AssemblyQualifiedName of the plug-in's type.
		/// </summary>
		public T LoadPlugIn<T>(string name)
		{
			Type plugInType;
			try {
				plugInType = Type.GetType(name);
			}
			catch (System.Exception e) {
				throw new Exception(name, "GetType error", e);
			}
			if (plugInType == null)
				throw new NotInstalledException(name);
			try {
				Assembly assembly = plugInType.Assembly;
				T plugIn = (T) assembly.CreateInstance(plugInType.FullName);
				Debug.Assert( (object) plugIn != null );
				return plugIn;
			}
			catch (System.InvalidCastException) {
				throw new CategoryException(name, plugInType, typeof(T));
			}
			catch (System.Exception e) {
				throw new Exception(name, "CreateInstance error", e);
			}
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
