namespace Landis
{
	/// <summary>
	/// A method for determining whether a cohort is selected for a particular
	/// operation (e.g., removal from the site).
	/// </summary>
	public delegate bool SelectMethod<T>(T cohort);
}
