using Edu.Wisc.Forest.Flel.Util;
using Landis.PlugIns;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Landis.PlugIns.Admin
{
	/// <summary>
	/// A parser that reads information about an extension from text input.
	/// </summary>
	public class ExtensionParser
		: Landis.TextParser<ExtensionInfo>
	{
		private IsNameInUseMethod isNameInUse;

		//---------------------------------------------------------------------

		public override string LandisDataValue
		{
			get {
				return "Extension";
			}
		}

		//---------------------------------------------------------------------

		public ExtensionParser(IsNameInUseMethod isNameInUseMethod)
		{
			this.isNameInUse = isNameInUseMethod;
		}

		//---------------------------------------------------------------------

		protected override ExtensionInfo Parse()
		{
			ReadLandisDataVar();

			ExtensionInfo extension = new ExtensionInfo();

			InputVar<string> name = new InputVar<string>("Name");
			ReadVar(name);
			CheckNotEmptyOrBlank(name);
			if (isNameInUse(name.Value.Actual))
				throw new InputValueException(name.Value.String, "The name \"{0}\" is already in the extensions database", name.Value.Actual);
			extension.Name = name.Value;

			InputVar<string> description = new InputVar<string>("Description");
			ReadVar(description);
			CheckNotEmptyOrBlank(description);
			extension.Description = description.Value;

			InputVar<string> userGuide = new InputVar<string>("UserGuide");
			ReadVar(userGuide);
			CheckFileExists(userGuide);
			extension.UserGuidePath = userGuide.Value;

			InputVar<string> assemblyPath = new InputVar<string>("Assembly");
			ReadVar(assemblyPath);
			CheckNotEmptyOrBlank(assemblyPath);
			CheckFileExists(assemblyPath);
			Assembly assembly = LoadAssembly(assemblyPath.Value);
			extension.Assembly = assembly;
			extension.AssemblyPath = assemblyPath.Value;

			InputVar<string> className = new InputVar<string>("Class");
			ReadVar(className);
			CheckNotEmptyOrBlank(className);
			extension.InterfaceType = GetInterfaceType(className.Value, assembly);
			extension.ClassName = className.Value;

			InputVar<string> library = new InputVar<string>("Library");
			while (ReadOptionalVar(library)) {
				CheckFileExists(library);
				extension.LibraryPaths.Add(library.Value.Actual);
			}
			
			InputVar<string> coreVersion = new InputVar<string>("CoreVersion");
			ReadVar(coreVersion);
			extension.CoreVersion = GetVersion(coreVersion.Value);

			CheckNoDataAfter(string.Format("the {0} parameter", coreVersion.Name));

            return extension;
		}

		//---------------------------------------------------------------------

		private void CheckNotEmptyOrBlank(InputVar<string> var)
		{
			if (var.Value.Actual.Length == 0)
				throw new InputValueException(var.Value.String, "Empty string is not permitted");
			if (var.Value.Actual.Trim(null) == "")
				throw new InputValueException(var.Value.String, "String with just whitespace is not permitted");
		}

		//---------------------------------------------------------------------

		private void CheckFileExists(InputVar<string> var)
		{
			string path = var.Value.Actual;
			if (! File.Exists(path)) {
				if (System.IO.Directory.Exists(path))
					throw new InputValueException(var.Value.String, "{0} is not a file; it's a directory", path);
				throw new InputValueException(var.Value.String, "The file {0} does not exist", path);
			}
		}

		//---------------------------------------------------------------------

		private Assembly LoadAssembly(InputValue<string> name)
		{
			try {
				string fullPath = Path.GetFullPath(name.Actual);
				return Assembly.LoadFile(fullPath);
			}
			catch (System.Exception) {
				throw new InputValueException(name.String, "Could not load the assembly {0}", name.Actual);
			}
		}

		//---------------------------------------------------------------------

		private System.Type GetInterfaceType(string   className,
		                                     Assembly assembly)
		{
			System.Type classType;
			try {
				classType = assembly.GetType(className);
			}
			catch (System.Exception) {
				throw new InputValueException(className, "Could not get the class {0} from the assembly", className);
			}
			if (classType == null)
				throw new InputValueException(className, "The assembly has no {0} class", className);
			if (! classType.IsClass)
				throw new InputValueException(className, "{0} is not a class", className);

			string[] interfaceNames = new string[]{
				"Landis.PlugIns.ISuccession",
				"Landis.PlugIns.IDisturbance",
				"Landis.PlugIns.IOutput"
			};
			foreach (string interfaceName in interfaceNames) {
				System.Type interfaceType = classType.GetInterface(interfaceName);
				if (interfaceType != null)
					return interfaceType;
			}
			string[] message = new string[]{
				string.Format("The class {0} does not", className),
				"implement a Landis-II extension interface"
			};
			throw new InputValueException(className, message);
		}

		//---------------------------------------------------------------------

		private System.Version GetVersion(string version)
		{
			Regex pattern = new Regex(@"^\d+(\.\d+){1,3}$");
			if (! pattern.IsMatch(version))
				throw new InputValueException(version, "\"{0}\" is not a proper version number", version);
			try {
				return new System.Version(version);
			}
			catch (System.OverflowException) {
				throw new InputValueException(version, "One or more parts of \"{0}\" is too big", version);
			}
		}
	}
}
