using Edu.Wisc.Forest.Flel.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace Landis.PlugIns.Admin
{
	/// <summary>
	/// A command that adds an extension to the plug-in database.
	/// </summary>
	public class AddCommand
		: ICommand
	{
		private static string installDir = Application.Directory;

		private string extensionInfoPath;
		private EditableDataset dataset;

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public AddCommand(string extensionInfoPath)
		{
			this.extensionInfoPath = extensionInfoPath;
			this.dataset = Dataset.LoadOrCreate();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Determines if an extension name is already in use in the plug-ins
		/// datasbase.
		/// </summary>
		public bool IsNameInUse(string name)
		{
			return dataset.Find(name) != null;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Executes the command.
		/// </summary>
		public void Execute()
		{
			ExtensionParser parser = new ExtensionParser(IsNameInUse);
			ExtensionInfo extension = Data.Load<ExtensionInfo>(extensionInfoPath, parser);

			List<string> missingLibs = new List<string>();
			foreach (string library in extension.ReferencedAssemblies) {
				if (! dataset.ReferencedByEntries(library))
					missingLibs.Add(library);
			}

			List<string> libsToBeInstalled = new List<string>();
			foreach (string libPath in extension.LibraryPaths) {
				libsToBeInstalled.Add(Path.GetFileNameWithoutExtension(libPath));
			}

			foreach (string libToBeInstalled in libsToBeInstalled) {
				missingLibs.Remove(libToBeInstalled);
			}
			if (missingLibs.Count > 0) {
				MultiLineText message = new MultiLineText();
				message.Add("Error: The extension requires the following libraries which are not");
				message.Add("       currently installed and are not listed in the extension info file:");
				foreach (string lib in missingLibs)
					message.Add("         " + lib);
				throw new MultiLineException(message);
			}

			Console.WriteLine("Installation directory: {0}", installDir);
			Console.WriteLine("Copying files to installation directory ...");
			CopyFileToInstallDir(extension.AssemblyPath);
			foreach (string libPath in extension.LibraryPaths) {
				CopyFileToInstallDir(libPath);
			}

			dataset.Add(extension);
			dataset.Save();
			Console.WriteLine("Extension {0} installed", extension.Name);
		}

		//---------------------------------------------------------------------

		private void CopyFileToInstallDir(string path)
		{
			string libFileName = Path.GetFileName(path);
			string targetFile = Path.Combine(installDir, libFileName);
			bool overwritten = File.Exists(targetFile);
			File.Copy(path, targetFile, true);
			if (overwritten)
				Console.WriteLine("  {0}  (replaced existing file)", libFileName);
			else
				Console.WriteLine("  {0}", libFileName);
		}
	}
}
