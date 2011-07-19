using Landis.Ecoregions;
using Landis.Landscape;
using Landis.Raster;
using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Ecoregions
{
	[TestFixture]
	public class Map_Test
	{
		private Dataset dataset;
		private byte[,] ecoregions8Bit;
		private Dimensions dims8Bit;
		private string path8Bit;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			List<IParameters> ecoregionParms = new List<IParameters>();
			ecoregionParms.Add(new Parameters("eco0", "Ecoregion A", 0, true));
			ecoregionParms.Add(new Parameters("eco11", "Ecoregion B", 11, false));
			ecoregionParms.Add(new Parameters("eco222", "Ecoregion C", 222, true));
			ecoregionParms.Add(new Parameters("eco3333", "Ecoregion D", 3333, false));
			ecoregionParms.Add(new Parameters("eco-32768", "Ecoregion E", -32768, true));

			dataset = new Dataset(ecoregionParms);

			//	Write 8-bit ecoregion map
			ecoregions8Bit = new byte[,] {
				{   0,   0,  11, 222,  11 },
				{   0,  11,  11, 222,  11 },
				{   0,  11,  11, 222, 222 },
				{  11,  11,  11, 222, 222 },
				{  11,  11, 222, 222, 222 },
				{  11,  11, 222, 222, 222 }
			};
			dims8Bit = new Raster.Dimensions(ecoregions8Bit.GetLength(0),
			                                 ecoregions8Bit.GetLength(1));
			path8Bit = Data.MakeOutputPath("map-8-bit.gis");
			IOutputRaster<Pixel8Bit> map8Bit;
			map8Bit = Landis.Util.Raster.Create<Pixel8Bit>(path8Bit,
			                                               dims8Bit,
			                                               null  /* Metadata */);
			using (map8Bit) {
				Pixel8Bit pixel = new Pixel8Bit();
				for (int row = 0; row < dims8Bit.Rows; ++row) {
					for (int column = 0; column< dims8Bit.Columns; ++column) {
						pixel.Band0 = ecoregions8Bit[row, column];
						map8Bit.WritePixel(pixel);
					}
				}
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void Map8Bit()
		{
			Map map = new Map(path8Bit, dataset);
			using (IInputGrid<bool> inputGrid = map.OpenAsInputGrid()) {
				Assert.AreEqual(dims8Bit.Rows, inputGrid.Dimensions.Rows);
				Assert.AreEqual(dims8Bit.Columns, inputGrid.Dimensions.Columns);

				for (int row = 0; row < dims8Bit.Rows; ++row) {
					for (int column = 0; column < dims8Bit.Columns; ++column) {
						IEcoregion ecoregion = dataset.Find(ecoregions8Bit[row,column]);
						Assert.AreEqual(ecoregion.Active, inputGrid.ReadValue());
					}
				}
			}
		}
	}
}
