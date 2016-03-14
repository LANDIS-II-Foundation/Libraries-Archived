namespace Landis
{
	/// <summary>
	/// A binary file to which objects are serialized.
	/// </summary>
	public class OutputBinaryFile
		: Edu.Wisc.Forest.Flel.Util.OutputBinaryFile
	{
		private string identifier;
		private bool disposed;

		//---------------------------------------------------------------------

		/// <summary>
		/// The file's identifier.
		/// </summary>
		public string Identifier
		{
			get {
				CheckNotDisposed();
				return identifier;
			}
		}

		//---------------------------------------------------------------------

		private void CheckNotDisposed()
		{
			if (disposed)
				throw new System.ObjectDisposedException(typeof(OutputBinaryFile).FullName);
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="path">
		/// The file's path.
		/// </param>
		/// <param name="identifier">
		/// The file's identifier.
		/// </param>
		public OutputBinaryFile(string path,
		                        string identifier)
			: base(path)
		{
			this.identifier = identifier;
			this.disposed = false;
			Serialize(new BinaryFileIdentifier(identifier));
		}

		//---------------------------------------------------------------------

		/// <summary>Dispose unmanaged resources and possibly managed
		/// resources as well.
		/// </summary>
		/// <param name="disposeManaged">
		/// If true, then managed resources should be disposed along with
		/// unmanaged resources.  If false, then only unmanaged resources
		/// should be disposed.
		/// </param>
		protected override void Dispose(bool disposeManaged)
		{
			if (! disposed) {
				disposed = true;
				base.Dispose(disposeManaged);
			}
		}
	}
}
