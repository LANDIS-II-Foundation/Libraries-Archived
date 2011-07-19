namespace Landis.Util
{
	/// <summary>
	/// A parser that reads input from a LineReader and returns an instance
	/// of T.
	/// </summary>
	public interface ITextParser<T>
	{
		/// <summary>
		/// Parses the input from a LineReader and constructs an instance of T.
		/// </summary>
		T Parse(LineReader reader);
	}
}
