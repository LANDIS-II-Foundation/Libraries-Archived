using Landis.Landscape;
using Landis.Species;

namespace Landis
{
	/// <summary>
	/// A method for dispersing seeds: Does a particular species disperse seeds
	/// into a site?
	/// </summary>
	public delegate bool DisperseSeedMethod(ISpecies   species,
	                                        ActiveSite site);
}
