using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

using Landis.Landscape;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Test
{
	[TestFixture]
	public class ActiveSiteMap_Test
	{
#if TEST_INTERNALS
		private DataGrid<bool> mixedGrid;
		private ActiveSiteMap  mixedMap;

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
		}
#endif  //  TEST_INTERNALS

		//---------------------------------------------------------------------

		[Test]
		public void Grid0x0()
		{
			bool[,] array = new bool[0,0];
			DataGrid<bool> grid = new DataGrid<bool>(array);
			ActiveSiteMap map = new ActiveSiteMap(grid);

			Assert.AreEqual(0, map.Count);
			int iterations = 0;
			foreach (ActiveSiteMap.Entry entry in map)
				iterations++;
			Assert.AreEqual(map.Count, iterations);
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
			foreach (ActiveSiteMap.Entry entry in map)
				iterations++;
			Assert.AreEqual(map.Count, iterations);
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
			foreach (ActiveSiteMap.Entry entry in map)
				iterations++;
			Assert.AreEqual(map.Count, iterations);
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
			foreach (ActiveSiteMap.Entry entry in map) {
				Assert.IsTrue(index < map.Count);
				Assert.AreEqual(location, entry.Location);
				Assert.AreEqual((uint) index, entry.Index);
				index++;
				location = RowMajor.Next(location, grid.Columns);
			}
			Assert.AreEqual(map.Count, index);
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
			foreach (ActiveSiteMap.Entry entry in map) {
				Assert.IsTrue(index < map.Count);
				index++;
			}
			Assert.AreEqual(map.Count, index);
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
			foreach (ActiveSiteMap.Entry entry in map) {
				Assert.IsTrue(index < map.Count);
				Assert.AreEqual(location, entry.Location);
				Assert.AreEqual((uint) index, entry.Index);
				index++;
				location = RowMajor.Next(location, grid.Columns);
			}
			Assert.AreEqual(map.Count, index);
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
			foreach (ActiveSiteMap.Entry entry in map) {
				Assert.IsTrue(index < map.Count);
				index++;
			}
			Assert.AreEqual(map.Count, index);
		}

		//---------------------------------------------------------------------

#if TEST_INTERNALS
		[Test]
		public void RowIntervals()
		{
			List<ActiveSiteMap.Interval> rowIntervals = mixedMap.RowIntervals;
			Assert.AreEqual(2, rowIntervals.Count);

			Assert.AreEqual(1, rowIntervals[0].Start);
			Assert.AreEqual(2, rowIntervals[0].End);
			Assert.AreEqual(0, rowIntervals[0].StartOffset);

			Assert.AreEqual(4, rowIntervals[1].Start);
			Assert.AreEqual(6, rowIntervals[1].End);
			Assert.AreEqual(2, rowIntervals[1].StartOffset);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ActiveRows()
		{
			List<ActiveSiteMap.ActiveRow> activeRows = mixedMap.ActiveRows;
			Assert.AreEqual(5, activeRows.Count);

			Assert.AreEqual(1, activeRows[0].IntervalCount);
			Assert.AreEqual(0, activeRows[0].FirstIntervalOffset);

			Assert.AreEqual(2, activeRows[1].IntervalCount);
			Assert.AreEqual(1, activeRows[1].FirstIntervalOffset);

			Assert.AreEqual(1, activeRows[2].IntervalCount);
			Assert.AreEqual(3, activeRows[2].FirstIntervalOffset);

			Assert.AreEqual(4, activeRows[3].IntervalCount);
			Assert.AreEqual(4, activeRows[3].FirstIntervalOffset);

			Assert.AreEqual(1, activeRows[4].IntervalCount);
			Assert.AreEqual(8, activeRows[4].FirstIntervalOffset);
		}
#endif	// TEST_INTERNALS
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
			foreach (ActiveSiteMap.Entry entry in map) {
				Assert.IsTrue(index < map.Count);
				Assert.AreEqual(index, entry.Index);
				Assert.AreEqual(activeSites[index], entry.Location);
				index++;
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
						Assert.AreEqual(index, map[location]);
						index++;
						if (index < activeSites.Count)
							nextActiveSite = activeSites[index];
						else
							nextActiveSite = null;
					}
					else
						Assert.AreEqual(map.Count, map[location]);
				}
		}
	}
}
