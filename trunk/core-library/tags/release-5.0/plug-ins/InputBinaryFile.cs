namespace Landis
{
	/// <summary>
	/// A binary file from which objects are deserialized.
	/// </summary>
	public class InputBinaryFile
		: Edu.Wisc.Forest.Flel.Util.InputBinaryFile
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
				throw new System.ObjectDisposedException(typeof(InputBinaryFile).FullName);
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Initializes a new instance.
		/// </summary>
		/// <exception cref="System.ApplicationException">
		/// The file is not a Landis-II binary file.
		/// </exception>
		public InputBinaryFile(string path)
			: base(path)
		{
			this.disposed = false;
			try {
				BinaryFileIdentifier fileId = Deserialize<BinaryFileIdentifier>();
				identifier = fileId.ToString();
			}
			catch (System.InvalidCastException) {
				throw new System.ApplicationException("The file is not a Landis-II binary file");
			}
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="path">
		/// The file's path.
		/// </param>
		/// <param name="identifier">
		/// The identifier that is expected to be in the file.
		/// </param>
		/// <exception cref="System.ApplicationException">
		/// The file is not a Landis-II binary file, or the file's identifier
		/// does not match the identifier parameter.
		/// </exception>
		public InputBinaryFile(string path,
		                       string identifier)
			: this(path)
		{
			if (Identifier != identifier)
				throw new System.ApplicationException(string.Format("The file's identifier is \"{0}\", but expected it to be \"{1}\"",
				                                                    Identifier, identifier));
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
