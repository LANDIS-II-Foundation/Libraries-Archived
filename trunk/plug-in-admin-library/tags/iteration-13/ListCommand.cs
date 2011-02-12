using System;
using System.Collections.Generic;

namespace Landis.PlugIns.Admin
{
	/// <summary>
	/// A command that lists the extensions in the plug-in database.
	/// </summary>
	public class ListCommand
		: ICommand
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ListCommand()
		{
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Executes the command.
		/// </summary>
		public void Execute()
		{
			EditableDataset dataset = Dataset.LoadIfExists();
			if (dataset == null || dataset.Count == 0)
				Console.WriteLine("No extensions are installed.");
			else {
				for (int i = 0; i < dataset.Count; i++) {
					IDatasetEntry entry = dataset[i];
					Console.WriteLine("   Extension : {0}", entry.Name);
					Console.WriteLine("     Version : {0}", entry.Version);
					Console.WriteLine("        Type : {0}", Interface.GetName(entry.InterfaceType));
					Console.WriteLine(" Description : {0}", entry.Description);
					Console.WriteLine("  User Guide : {0}", entry.UserGuidePath);
					Console.WriteLine("Core Version : {0}", entry.CoreVersion);
					Console.WriteLine("    Assembly : {0}", entry.AssemblyName);
					Console.WriteLine("       Class : {0}", entry.ClassName);

					IList<string> libs = entry.ReferencedAssemblies;
					if (libs.Count > 0) {
						Console.WriteLine("   Libraries : {0}", libs[0]);
						for (int j = 1; j < libs.Count; j++)
							Console.WriteLine("               {0}", libs[j]);
					}
				}
			}
		}
	}
}
