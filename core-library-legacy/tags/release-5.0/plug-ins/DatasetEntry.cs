using Edu.Wisc.Forest.Flel.Util.PlugIns;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Landis.PlugIns
{
	/// <summary>
	/// An individual entry in a plug-ins dataset.
	/// </summary>
	[Serializable]
	public class DatasetEntry
		: IDatasetEntry, ISerializable
	{
		private static List<string> commonlyExcludedAssemblies;

		//---------------------------------------------------------------------

		static DatasetEntry()
		{
			string[] names = new string[]{
				"mscorlib",
				"Edu.Wisc.Forest.Flel.Util",
				"Landis.Cohorts",
				"Landis.Ecoregions",
				"Landis.Landscape",
				"Landis.Main",
				"Landis.PlugIns",
				"Landis.Species",
				"Landis.RasterIO",
				"Landis.Util"
			};
			commonlyExcludedAssemblies = new List<string>(names);
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// The list of simple names of the assemblies that are commonly
		/// excluded from an entry's ReferencedAssemblies property.
		/// </summary>
		public static IList<string> CommonlyExcludedAssemblies
		{
			get {
				return commonlyExcludedAssemblies;
			}
		}

		//---------------------------------------------------------------------

		private string name;
		private System.Type interfaceType;
		private string implementationName;
		private string description;
		private string className;
		private string assemblyName;
		private Assembly assembly;
		private System.Version version;
		private System.Version coreVersion;
		private string userGuidePath;
		private IList<string> referencedAssemblies;

		//---------------------------------------------------------------------

		/// <summary>
		/// The name that users refer to the plug-in by.
		/// </summary>
		public string Name
		{
			get {
				return name;
			}

			set {
				name = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The type of the plug-in's interface.
		/// </summary>
		public System.Type InterfaceType
		{
			get {
				return interfaceType;
			}

			set {
				interfaceType = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The AssemblyQualifiedName of the class that implements the plug-in.
		/// </summary>
		public string ImplementationName
		{
			get {
				return implementationName;
			}

			set {
				implementationName = value;
				string[] implNameParts = implementationName.Split(',');
				className = implNameParts[0];
				string newAssemblyName = implNameParts[1];
				if (newAssemblyName != assemblyName)
					assembly = null;
				assemblyName = newAssemblyName;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// A short description of the plug-in.
		/// </summary>
		/// <remarks>
		/// Used in lists in user documentation, so it is a brief sentence.
		/// </remarks>
		public string Description
		{
			get {
				return description;
			}

			set {
				description = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The name of the class that implements the plug-in.
		/// </summary>
		/// <remarks>
		/// The class name includes the full namespace but does not include
		/// the assembly name.  This name is the leading portion of the
		/// ImplementationName that preceeds the comma.
		/// </remarks>
		public string ClassName
		{
			get {
				return className;
			}

			set {
				className = value;
				implementationName = className + "," + assemblyName;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The name of the assembly that contains the class that implements
		/// the plug-in.
		/// </summary>
		/// <remarks>
		/// This name is the trailing portion of the ImplementationName that
		/// follows the comma.
		/// </remarks>
		public string AssemblyName
		{
			get {
				return assemblyName;
			}

			set {
				if (value != assemblyName)
					assembly = null;
				assemblyName = value;
				implementationName = className + "," + assemblyName;
			}
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// The loaded assembly that contains the plug-in.
		/// </summary>
		/// <remarks>
		/// Used to set the AssemblyName, Version and ReferencedAssemblies
		/// properties.
		/// </remarks>
		public Assembly Assembly
		{
			set {
				Edu.Wisc.Forest.Flel.Util.Require.ArgumentNotNull(value);
				assembly = value;
				assemblyName = assembly.GetName().Name;
				version = assembly.GetName().Version;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// A list of names of libraries referenced by the plug-in's assembly.
		/// </summary>
		/// <remarks>
		/// This read-only list does not include the assemblies listed in the
		/// DatasetEntry.CommonlyExcludedAssemblies property.
		/// </remarks>
		public IList<string> ReferencedAssemblies
		{
			get {
				if (referencedAssemblies == null) {
					if (assemblyName == null)
						throw new System.InvalidOperationException("Assembly name has not been set");

					if (assembly == null)
						assembly = Assembly.ReflectionOnlyLoad(assemblyName);
					List<string> referencedLibs = new List<string>();
					foreach (AssemblyName referencedLib in assembly.GetReferencedAssemblies()) {
						if (! DatasetEntry.CommonlyExcludedAssemblies.Contains(referencedLib.Name))
							referencedLibs.Add(referencedLib.Name);
					}
					referencedAssemblies = referencedLibs.AsReadOnly();
				}
				return referencedAssemblies;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The plug-in's version.
		/// </summary>
		/// <remarks>
		/// This is the version of the plug-in's assembly.
		/// </remarks>
		public System.Version Version
		{
			get {
				return version;
			}

			set {
				if (assembly != null)
					throw new System.InvalidOperationException("Version property already fetched from assembly");
				version = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The minimum version of the core framework that the plug-in
		/// requires.
		/// </summary>
		public System.Version CoreVersion
		{
			get {
				return coreVersion;
			}

			set {
				coreVersion = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The path to the plug-in's user guide.
		/// </summary>
		/// <remarks>
		/// null if there is no user guide.
		/// </remarks>
		public string UserGuidePath
		{
			get {
				return userGuidePath;
			}

			set {
				userGuidePath = value;
			}
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Initializes a new instance with all properties set to their default
		/// values based on their types.
		/// </summary>
		public DatasetEntry()
		{
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Initializes a new instance by setting its properties equal to those
		/// of another entry.
		/// </summary>
		public DatasetEntry(IDatasetEntry entry)
		{
			this.name                 = entry.Name;
			this.interfaceType        = entry.InterfaceType;
			this.implementationName   = entry.ImplementationName;
			this.description          = entry.Description;
			this.className            = entry.ClassName;
			this.assemblyName         = entry.AssemblyName;
			this.version              = entry.Version;
			this.coreVersion          = entry.CoreVersion;
			this.userGuidePath        = entry.UserGuidePath;
			this.referencedAssemblies = entry.ReferencedAssemblies;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance from serialization information (i.e.,
		/// when an instance is being deserialized).
		/// </summary>
		internal DatasetEntry(SerializationInfo info,
		                      StreamingContext  context)
		{
			this.name = info.GetString("name");
			this.interfaceType = Type.GetType(info.GetString("interfaceType.AssemblyQualifiedName"));

			//	Use the property's set accessor so that the ClassName and
			//	AssemblyName properties are also set.
			this.ImplementationName = info.GetString("implementationName");

			this.description = info.GetString("description");
			this.version = (Version) info.GetValue("version", typeof(Version));
			this.coreVersion = (Version) info.GetValue("coreVersion", typeof(Version));
			this.userGuidePath= info.GetString("userGuidePath");
			this.referencedAssemblies = (IList<string>) info.GetValue("referencedAssemblies", typeof(IList<string>));
		}

		//---------------------------------------------------------------------

		public void GetObjectData(SerializationInfo info,
		                          StreamingContext  context)
		{
			info.AddValue("name", name);
			info.AddValue("interfaceType.AssemblyQualifiedName", interfaceType.AssemblyQualifiedName);
			info.AddValue("implementationName", implementationName);
			info.AddValue("description", description);
			info.AddValue("version", version);
			info.AddValue("coreVersion", coreVersion);
			info.AddValue("userGuidePath", userGuidePath);
			info.AddValue("referencedAssemblies", ReferencedAssemblies);
		}
	}
}
