using Edu.Wisc.Forest.Flel.Util.PlugIns;
using System.Collections.Generic;

namespace Landis.PlugIns
{
	/// <summary>
	/// An individual entry in a plug-ins dataset.
	/// </summary>
	public interface IDatasetEntry
		: IInfo
	{
		/// <summary>
		/// A short description of the plug-in.
		/// </summary>
		/// <remarks>
		/// Used in lists in user documentation, so it is a brief sentence.
		/// </remarks>
		string Description
		{
			get;
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
		string ClassName
		{
			get;
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
		string AssemblyName
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// A list of names of libraries referenced by the plug-in's assembly.
		/// </summary>
		/// <remarks>
		/// This read-only list does not include the assemblies listed in the
		/// DatasetEntry.CommonlyExcludedAssemblies property.
		/// </remarks>
		IList<string> ReferencedAssemblies
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The plug-in's version.
		/// </summary>
		/// <remarks>
		/// This is the version of the plug-in's assembly.
		/// </remarks>
		System.Version Version
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The minimum version of the core framework that the plug-in
		/// requires.
		/// </summary>
		System.Version CoreVersion
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The path to the plug-in's user guide.
		/// </summary>
		/// <remarks>
		/// null if there is no user guide.
		/// </remarks>
		string UserGuidePath
		{
			get;
		}
	}
}
