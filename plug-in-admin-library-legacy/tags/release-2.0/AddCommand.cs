using Edu.Wisc.Forest.Flel.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace Landis.PlugIns.Admin
{
    /// <summary>
    /// A command that adds information about an extension to the plug-in
    /// database.
    /// </summary>
    public class AddCommand
        : ICommand
    {
        private string extensionInfoPath;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AddCommand(string extensionInfoPath)
        {
            this.extensionInfoPath = extensionInfoPath;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute()
        {
	        Dataset dataset = Util.OpenDatasetForChange(Dataset.DefaultPath);
	        EditableExtensionInfo.Dataset = dataset;

	        ExtensionParser parser = new ExtensionParser();
            ExtensionInfo extension = Data.Load<ExtensionInfo>(extensionInfoPath, parser);

            dataset.Add(extension);
            dataset.Save();
            Console.WriteLine("Added the extension \"{0}\"", extension.Name);
        }
    }
}
