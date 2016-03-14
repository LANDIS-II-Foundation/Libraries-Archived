using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Edu.Wisc.Forest.Flel.Util
{
	/// <summary>
	/// A binary file to which objects are serialized.
	/// </summary>
	public class OutputBinaryFile
		: BinaryFile
	{
		private FileStream stream;
		private bool disposed;

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
		public OutputBinaryFile(string path)
			: base(path)
		{
			this.stream = new FileStream(path, FileMode.Create);
			this.disposed = false;
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Serializes an instance of the specified type to the file.
		/// </summary>
		/// <exception cref="System.Runtime.Serialization.SerializationException">
		/// An error occurred while serializing the instance.
		/// </exception>
		public void Serialize<T>(T instance)
		{
			CheckNotDisposed();
			this.BinaryFormatter.Serialize(stream, instance);
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
				if (stream != null) {
					stream.Close();
					stream = null;
				}
				base.Dispose(disposeManaged);
			}
		}
	}
}
