using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Landis.Test
{
	[TestFixture]
	public class ActiveSiteMap_Test
	{
		private DataGrid<bool> mixedGrid;
		private ActiveSiteMap  mixedMap;
		private Location mixed_1stActive;
		private Location mixed_1stInactive;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			string[] rows = new string[] {	"....X..",
											"...XX.X",
											".......",
											"...XXX.",
											"X.X.X.X",
											"XXXXXXX" };
			bool [,] activeSites = Bool.Make2DimArray(rows, "X");
			mixedGrid = new DataGrid<bool>(activeSites);
			mixedMap = new ActiveSiteMap(mixedGrid);

			bool found_1stActive = false;
			bool found_1stInactive = false;
			for (uint row = 1; row <= mixedGrid.Rows; ++row) {
				for (uint column = 1; column <= mixedGrid.Columns; ++column) {
					if (mixedGrid[row, column]) {
						if (! found_1stActive) {
							mixed_1stActive = new Location(row, column);
							found_1stActive = true;
						}		
					}
					else {
						if (! found_1stInactive) {
							mixed_1stInactive = new Location(row, column);
							found_1stInactive = true;
						}		
					}
				}
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void Grid0x0()
		{
			bool[,] array = new bool[0,0];
			DataGrid<bool> grid = new DataGrid<bool>(array);
			ActiveSiteMap map = new ActiveSiteMap(grid);

			Assert.AreEqual(0, map.Count);
			int iterations = 0;
			foreach (LocationAndIndex entry in map)
				iterations++;
			Assert.AreEqual(map.Count, iterations);

			Assert.IsNull(map.FirstActive);
			Assert.IsNull(map.FirstInactive);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Grid1x0()
		{
			bool[,] array = new bool[1,0];
			DataGrid<bool> grid = new DataGrid<bool>(array);
			ActiveSiteMap map = new ActiveSiteMap(grid);

			Assert.AreEqual(0, map.Count);
			int iterations = 0;
			foreach (LocationAndIndex entry in map)
				iterations++;
			Assert.AreEqual(map.Count, iterations);

			Assert.IsNull(map.FirstActive);
			Assert.IsNull(map.FirstInactive);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Grid4x0()
		{
			bool[,] array = new bool[4,0];
			DataGrid<bool> grid = new DataGrid<bool>(array);
			ActiveSiteMap map = new ActiveSiteMap(grid);

			Assert.AreEqual(0, map.Count);
			int iterations = 0;
			foreach (LocationAndIndex entry in map)
				iterations++;
			Assert.AreEqual(map.Count, iterations);

			Assert.IsNull(map.FirstActive);
			Assert.IsNull(map.FirstInactive);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Grid1x1True()
		{
			bool[,] array = new bool[,] { {true} };
			DataGrid<bool> grid = new DataGrid<bool>(array);
			ActiveSiteMap map = new ActiveSiteMap(grid);

			Assert.AreEqual(grid.Count, map.Count);

			int index = 0;
			Location location = new Location(1,1);
			foreach (LocationAndIndex entry in map) {
				index++;
				Assert.IsTrue(index <= map.Count);
				Assert.AreEqual(location, entry.Location);
				Assert.AreEqual((uint) index, entry.Index);
				location = RowMajor.Next(location, grid.Columns);
			}
			Assert.AreEqual(map.Count, index);

			Assert.AreEqual(new Location(1,1), map.FirstActive.Location);
			Assert.AreEqual(1, map.FirstActive.Index);
			Assert.IsNull(map.FirstInactive);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Grid1x1False()
		{
			bool[,] array = new bool[,] { {false} };
			DataGrid<bool> grid = new DataGrid<bool>(array);
			ActiveSiteMap map = new ActiveSiteMap(grid);

			Assert.AreEqual(0, map.Count);

			int index = 0;
			foreach (LocationAndIndex entry in map) {
				index++;
				Assert.IsTrue(index <= map.Count);
			}
			Assert.AreEqual(map.Count, index);

			Assert.IsNull(map.FirstActive);
			Assert.AreEqual(new Location(1,1), map.FirstInactive.Location);
			Assert.AreEqual(ActiveSiteMap.InactiveSiteDataIndex,
			                map.FirstInactive.Index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Grid5x3True()
		{
			bool[,] array = new bool[,] { {true, true, true},
										  {true, true, true},
										  {true, true, true},
										  {true, true, true},
										  {true, true, true} };
			DataGrid<bool> grid = new DataGrid<bool>(array);
			ActiveSiteMap map = new ActiveSiteMap(grid);

			Assert.AreEqual(grid.Count, map.Count);

			int index = 0;
			Location location = new Location(1,1);
			foreach (LocationAndIndex entry in map) {
				index++;
				Assert.IsTrue(index <= map.Count);
				Assert.AreEqual(location, entry.Location);
				Assert.AreEqual((uint) index, entry.Index);
				location = RowMajor.Next(location, grid.Columns);
			}
			Assert.AreEqual(map.Count, index);

			Assert.AreEqual(new Location(1,1), map.FirstActive.Location);
			Assert.AreEqual(1, map.FirstActive.Index);
			Assert.IsNull(map.FirstInactive);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Grid5x3False()
		{
			bool[,] array = new bool[,] { {false, false, false},
										  {false, false, false},
										  {false, false, false},
										  {false, false, false},
										  {false, false, false} };
			DataGrid<bool> grid = new DataGrid<bool>(array);
			ActiveSiteMap map = new ActiveSiteMap(grid);

			Assert.AreEqual(0, map.Count);

			int index = 0;
			foreach (LocationAndIndex entry in map) {
				index++;
				Assert.IsTrue(index <= map.Count);
			}
			Assert.AreEqual(map.Count, index);

			Assert.IsNull(map.FirstActive);
			Assert.AreEqual(new Location(1,1), map.FirstInactive.Location);
			Assert.AreEqual(ActiveSiteMap.InactiveSiteDataIndex,
			                map.FirstInactive.Index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Mixed_FirstActive()
		{
			Assert.IsNotNull(mixedMap.FirstActive);
			Assert.AreEqual(mixed_1stActive, mixedMap.FirstActive.Location);
			Assert.AreEqual(1, mixedMap.FirstActive.Index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Mixed_FirstInactive()
		{
			Assert.IsNotNull(mixedMap.FirstInactive);
			Assert.AreEqual(mixed_1stInactive, mixedMap.FirstInactive.Location);
			Assert.AreEqual(ActiveSiteMap.InactiveSiteDataIndex,
			                mixedMap.FirstInactive.Index);
		}
	}

	//-------------------------------------------------------------------------

	[TestFixture]
	public class ActiveSiteMap_TestMixed
	{
		private DataGrid<bool> grid;
		private ActiveSiteMap map;
		private List<Location> activeSites;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			string path = Path.Combine(Data.Directory, "mixed.txt");
			bool[,] array = Bool.Read2DimArray(path);
			grid = new DataGrid<bool>(array);
			map = new ActiveSiteMap(grid);

			path = Path.Combine(Data.Directory,
			                    "true-locs-in-mixed.txt");
			activeSites = Data.ReadLocations(path);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Enumerator()
		{
			Assert.AreEqual(activeSites.Count, map.Count);

			int index = 0;
			foreach (LocationAndIndex entry in map) {
				index++;
				Assert.IsTrue(index <= map.Count);
				Assert.AreEqual(index, entry.Index);
				Assert.AreEqual(activeSites[index-1], entry.Location);
			}
			Assert.AreEqual(map.Count, index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void EnumeratorWithEntryVar()
		{
			Assert.AreEqual(activeSites.Count, map.Count);

			LocationAndIndex myEntry = new LocationAndIndex();
			ActiveSiteMapEnumerator mapEtor = map.GetEnumerator();

			System.Type type = typeof(ActiveSiteMapEnumerator);
			MethodInfo useForCurrentEntryMethodInfo = type.GetMethod("UseForCurrentEntry", BindingFlags.Instance |
			                                                                               BindingFlags.NonPublic);
			useForCurrentEntryMethodInfo.Invoke(mapEtor, new object[]{myEntry});

			int index = 0;
			while (mapEtor.MoveNext()) {
				index++;
				Assert.IsTrue(index <= map.Count);
				Assert.AreSame(myEntry, mapEtor.Current);
				Assert.AreEqual(index, myEntry.Index);
				Assert.AreEqual(activeSites[index-1], myEntry.Location);
			}
			Assert.AreEqual(map.Count, index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void EnumeratorReuse()
		{
			Assert.AreEqual(activeSites.Count, map.Count);

			ActiveSiteMapEnumerator mapEtor = map.GetEnumerator();

			int index = 0;
			foreach (LocationAndIndex entry in mapEtor) {
				index++;
				Assert.IsTrue(index <= map.Count);
				Assert.AreEqual(index, entry.Index);
				Assert.AreEqual(activeSites[index-1], entry.Location);
			}
			Assert.AreEqual(map.Count, index);

			//	Reuse enumerator
			index = 0;
			foreach (LocationAndIndex entry in mapEtor) {
				index++;
				Assert.IsTrue(index <= map.Count);
				Assert.AreEqual(index, entry.Index);
				Assert.AreEqual(activeSites[index-1], entry.Location);
			}
			Assert.AreEqual(map.Count, index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IndexerLocation()
		{
			int index = 0;
			Location? nextActiveSite;
			if (index < activeSites.Count)
				nextActiveSite = activeSites[index];
			else
				nextActiveSite = null;
			for (uint row = 1; row <= grid.Rows; ++row)
				for (uint col = 1; col <= grid.Columns; ++col) {
					Location location = new Location(row, col);
					if (nextActiveSite != null && nextActiveSite == location) {
						Assert.AreEqual(index+1, map[location]);
						index++;
						if (index < activeSites.Count)
							nextActiveSite = activeSites[index];
						else
							nextActiveSite = null;
					}
					else
						Assert.AreEqual(ActiveSiteMap.InactiveSiteDataIndex,
						                map[location]);
				}
		}
	}
}
