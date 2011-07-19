using Landis.Landscape;
using Landis.Raster;
using Landis.Util;

namespace Landis.Ecoregions
{
	public class Map
	{
		private string path;
		private IDataset ecoregions;

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="path">
		/// Path to the raster file that represents the map.
		/// </param>
		/// <param name="ecoregions">Dataset of ecoregions</param>
		public Map(string   path,
		           IDataset ecoregions)
		{
			this.path = path;
			this.ecoregions = ecoregions;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Opens the map as an input grid of boolean values.
		/// </summary>
		/// <remarks>
		/// For use in constructing a landscape.
		/// </remarks>
		public IInputGrid<bool> OpenAsInputGrid()
		{
			IInputRaster<Pixel> map = Util.Raster.Open<Pixel>(path);
			return new InputGrid(map, ecoregions);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Creates a site variable with ecoregions.
		/// </summary>
		public ISiteVar<IEcoregion> CreateSiteVar(ILandscape landscape)
		{
			ISiteVar<IEcoregion> siteVar = landscape.NewSiteVar<IEcoregion>();
			IInputRaster<Pixel> map = Util.Raster.Open<Pixel>(path);
			foreach (Site site in landscape.AllSites) {
				Pixel pixel = map.ReadPixel();
				if (site.IsActive) {
					byte mapCode = pixel.Band0;
					siteVar[site] = ecoregions.Find(mapCode);
				}
			}
			map.Close();
			return siteVar;
		}
	}
}
