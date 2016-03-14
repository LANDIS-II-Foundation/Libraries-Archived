using Landis.Ecoregions;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using Landis.RasterIO;
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
		private ushort[,] ecoregions16Bit;
		private Dimensions dims16Bit;
		private string path16Bit;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			List<IParameters> ecoregionParms = new List<IParameters>();
			ecoregionParms.Add(new Parameters("eco0", "Ecoregion A", 0, true));
			ecoregionParms.Add(new Parameters("eco11", "Ecoregion B", 11, false));
			ecoregionParms.Add(new Parameters("eco222", "Ecoregion C", 222, true));
			ecoregionParms.Add(new Parameters("eco3333", "Ecoregion D", 3333, false));
			ecoregionParms.Add(new Parameters("eco-65535", "Ecoregion E", 65535, true));

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
			dims8Bit = new RasterIO.Dimensions(ecoregions8Bit.GetLength(0),
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

			//	Write 16-bit ecoregion map
			ecoregions16Bit = new ushort[,] {
				{   0,   0,  11, 222,  11,  3333,     0 },
				{   0,  11,  11, 222,  11,  3333, 65535 },
				{   0,  11,  11, 222, 222,  3333, 65535 },
				{  11,  11,  11, 222, 222,  3333, 65535 },
				{  11,  11, 222, 222, 222,  3333, 65535 },
				{  11,  11, 222, 222, 222, 65535, 65535 },
				{   0,   0, 222, 222, 222, 65535, 65535 }
			};
			dims16Bit = new RasterIO.Dimensions(ecoregions16Bit.GetLength(0),
			                                    ecoregions16Bit.GetLength(1));
			path16Bit = Data.MakeOutputPath("map-16-bit.gis");
			IOutputRaster<Pixel16Bit> map16Bit;
			map16Bit = Landis.Util.Raster.Create<Pixel16Bit>(path16Bit,
			                                                 dims16Bit,
			                                                 null  /* Metadata */);
			using (map16Bit) {
				Pixel16Bit pixel = new Pixel16Bit();
				for (int row = 0; row < dims16Bit.Rows; ++row) {
					for (int column = 0; column< dims16Bit.Columns; ++column) {
						pixel.Band0 = ecoregions16Bit[row, column];
						map16Bit.WritePixel(pixel);
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

		//---------------------------------------------------------------------

		[Test]
		public void Map16Bit()
		{
			Map map = new Map(path16Bit, dataset);
			using (IInputGrid<bool> inputGrid = map.OpenAsInputGrid()) {
				Assert.AreEqual(dims16Bit.Rows, inputGrid.Dimensions.Rows);
				Assert.AreEqual(dims16Bit.Columns, inputGrid.Dimensions.Columns);

				for (int row = 0; row < dims16Bit.Rows; ++row) {
					for (int column = 0; column < dims16Bit.Columns; ++column) {
						IEcoregion ecoregion = dataset.Find(ecoregions16Bit[row,column]);
						Assert.AreEqual(ecoregion.Active, inputGrid.ReadValue());
					}
				}
			}
		}
	}
}
