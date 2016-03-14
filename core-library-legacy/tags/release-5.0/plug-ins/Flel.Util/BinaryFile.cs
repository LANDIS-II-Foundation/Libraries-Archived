using System.Runtime.Serialization.Formatters.Binary;

namespace Edu.Wisc.Forest.Flel.Util
{
	/// <summary>
	/// A binary file for storing serialized objects.
	/// </summary>
	public abstract class BinaryFile
		: System.IDisposable
	{
		private string path;
		private BinaryFormatter formatter;

		// This class is somewhat based on the design pattern outlined in the
		// topic "Implementing a Dispose Method" in the .NET Framework
		// Developer's Guide.
		private bool disposed;

		//---------------------------------------------------------------------

		private void CheckNotDisposed()
		{
			if (disposed)
				throw new System.ObjectDisposedException(typeof(BinaryFile).FullName);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The path used to open/create the file.
		/// </summary>
		public string Path
		{
			get {
				CheckNotDisposed();
				return path;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The binary formatter that subclasses use to serialize or
		/// deserializes objects.
		/// </summary>
		protected BinaryFormatter BinaryFormatter
		{
			get {
				CheckNotDisposed();
				return formatter;
			}
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Initializes a new instance.
		/// </summary>
		public BinaryFile(string path)
		{
			this.path = path;
			this.formatter = new BinaryFormatter();
			this.disposed = false;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Closes the file, releasing any unmanaged resources.
		/// </summary>
		public void Close()
		{
			Dispose();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Closes the file, releasing any unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			System.GC.SuppressFinalize(this);
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
		protected virtual void Dispose(bool disposeManaged)
		{
			disposed = true;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Finalizes an instance before its destruction.
		/// </summary>
		/// <remarks>
		/// This destructor will run only if the Close or Dispose method wasn't
		/// called.  Derived classes have no need to provide their own
		/// destructors.
		/// </remarks>
		~BinaryFile()
		{
			Dispose(false);
		}
	}
}
