using System;

namespace Landis.PlugIns.Admin
{
	/// <summary>
	/// A command that remove an extension to the plug-in database.
	/// </summary>
	public class RemoveCommand
		: ICommand
	{
		private string extensionName;

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public RemoveCommand(string extensionName)
		{
			this.extensionName = extensionName;
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
				IDatasetEntry entry = dataset.Remove(extensionName);
				if (entry == null)
					Console.WriteLine("There is no extension named \"{0}\".", extensionName);
				else {
					Console.WriteLine("Extension {0} removed.", extensionName);
					dataset.Save();
				}
			}
		}
	}
}
