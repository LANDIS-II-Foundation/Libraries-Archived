using Edu.Wisc.Forest.Flel.Grids;
using Landis.Landscape;
using Landis.RasterIO;
using Landis.Util;

namespace Landis.Ecoregions
{
	public class Map
	{
		private string path;
		private IDataset ecoregions;
		private IRasterFactory rasterFactory;
		private IMetadata metadata;

		//---------------------------------------------------------------------

		/// <summary>
		/// The metadata in the raster file that represents the map.
		/// </summary>
		public IMetadata Metadata
		{
			get {
				return metadata;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="path">
		/// Path to the raster file that represents the map.
		/// </param>
		/// <param name="ecoregions">
		/// The dataset of ecoregions that are in the map.
		/// </param>
		/// <param name="rasterFactory">
		/// The raster factory to use to read the map.
		/// </param>
		public Map(string         path,
		           IDataset       ecoregions,
		           IRasterFactory rasterFactory)
		{
			this.path = path;
			this.ecoregions = ecoregions;
			this.rasterFactory = rasterFactory;
			IInputRaster<Pixel> map = rasterFactory.OpenRaster<Pixel>(path);
			using (map) {
				this.metadata = map.Metadata;
			}
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
			IInputRaster<Pixel> map = rasterFactory.OpenRaster<Pixel>(path);
			return new InputGrid(map, ecoregions);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Creates a site variable with ecoregions.
		/// </summary>
		public ISiteVar<IEcoregion> CreateSiteVar(ILandscape landscape)
		{
			ISiteVar<IEcoregion> siteVar = landscape.NewSiteVar<IEcoregion>();
			IInputRaster<Pixel> map = rasterFactory.OpenRaster<Pixel>(path);
			using (map) {
    			foreach (Site site in landscape.AllSites) {
    				Pixel pixel = map.ReadPixel();
    				if (site.IsActive) {
    					ushort mapCode = pixel.Band0;
    					siteVar[site] = ecoregions.Find(mapCode);
    				}
    			}
			}
			return siteVar;
		}
	}
}
