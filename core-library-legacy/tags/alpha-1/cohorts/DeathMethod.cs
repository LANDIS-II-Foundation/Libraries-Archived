using Landis.Landscape;

namespace Landis
{
	/// <summary>
	/// A method that invoked when a cohort dies at a site.
	/// </summary>
	public delegate void DeathMethod<T>(T          cohort,
	                                    ActiveSite site);
}
