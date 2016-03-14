using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Edu.Wisc.Forest.Flel.Util
{
	/// <summary>
	/// A binary file from which objects are deserialized.
	/// </summary>
	public class InputBinaryFile
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
		public InputBinaryFile(string path)
			: base(path)
		{
			this.stream = new FileStream(path, FileMode.Open);
			this.disposed = false;
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Deserializes an object of the specified type from the file.
		/// </summary>
		public T Deserialize<T>()
		{
			CheckNotDisposed();
			return (T) this.BinaryFormatter.Deserialize(stream);
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
