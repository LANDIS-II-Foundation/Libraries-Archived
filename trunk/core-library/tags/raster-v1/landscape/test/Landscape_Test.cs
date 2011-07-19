using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

using Landis.Landscape;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Test
{
	[TestFixture]
	public class Landscape_Test
	{
		private DataGrid<bool> grid;
		private Landscape.Landscape landscape;
		private List<Location> activeSites;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			string path = Path.Combine(Data.Directory, "mixed.txt");
			bool[,] array = Bool.Read2DimArray(path);
			grid = new DataGrid<bool>(array);
			landscape = new Landscape.Landscape(grid);

			path = Path.Combine(Data.Directory,
			                    "true-locs-in-mixed.txt");
			activeSites = Data.ReadLocations(path);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ActiveSiteEnumerator()
		{
			Assert.AreEqual(activeSites.Count, landscape.ActiveSiteCount);

			int index = 0;
			foreach (ActiveSite site in landscape) {
				Assert.IsTrue(index < landscape.ActiveSiteCount);
				Assert.AreEqual(index, site.DataIndex);
				Assert.AreEqual(activeSites[index], site.Location);
				Assert.AreEqual(landscape, site.Landscape);
				Assert.AreEqual(true, site.IsActive);
				index++;
			}
			Assert.AreEqual(landscape.ActiveSiteCount, index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ActiveSiteIndexer_Location()
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
					ActiveSite site = landscape[location];
					if (nextActiveSite != null && nextActiveSite == location) {
						Assert.AreEqual(location, site.Location);
						Assert.AreEqual(index, site.DataIndex);
						Assert.AreEqual(landscape, site.Landscape);
						Assert.AreEqual(true, site.IsActive);
						index++;
						if (index < activeSites.Count)
							nextActiveSite = activeSites[index];
						else
							nextActiveSite = null;
					}
					else
						Assert.IsNull(site);
				}
		}

		//---------------------------------------------------------------------

		[Test]
		public void ActiveSiteIndexer_RowColumn()
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
					ActiveSite site = landscape[row, col];
					if (nextActiveSite != null && nextActiveSite == location) {
						Assert.AreEqual(location, site.Location);
						Assert.AreEqual(index, site.DataIndex);
						Assert.AreEqual(landscape, site.Landscape);
						Assert.AreEqual(true, site.IsActive);
						index++;
						if (index < activeSites.Count)
							nextActiveSite = activeSites[index];
						else
							nextActiveSite = null;
					}
					else
						Assert.IsNull(site);
				}
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetSite_Location()
		{
			uint inactiveDataIndex = (uint) landscape.ActiveSiteCount;

			int index = 0;
			Location? nextActiveSite;
			if (index < activeSites.Count)
				nextActiveSite = activeSites[index];
			else
				nextActiveSite = null;
			for (uint row = 1; row <= grid.Rows; ++row)
				for (uint col = 1; col <= grid.Columns; ++col) {
					Location location = new Location(row, col);
					Site site = landscape.GetSite(location);
					if (nextActiveSite != null && nextActiveSite == location) {
						Assert.AreEqual(location, site.Location);
						Assert.AreEqual(index, site.DataIndex);
						Assert.AreEqual(landscape, site.Landscape);
						Assert.AreEqual(true, site.IsActive);
						index++;
						if (index < activeSites.Count)
							nextActiveSite = activeSites[index];
						else
							nextActiveSite = null;
					}
					else {
						Assert.AreEqual(location, site.Location);
						Assert.AreEqual(inactiveDataIndex, site.DataIndex);
						Assert.AreEqual(landscape, site.Landscape);
						Assert.AreEqual(false, site.IsActive);
					}
				}
		}

		//---------------------------------------------------------------------

		private void CheckGetSite_RowColumn(ILandscape landscape)
		{
			uint inactiveDataIndex = (uint) landscape.ActiveSiteCount;

			int index = 0;
			Location? nextActiveSite;
			if (index < activeSites.Count)
				nextActiveSite = activeSites[index];
			else
				nextActiveSite = null;
			for (uint row = 1; row <= grid.Rows; ++row)
				for (uint col = 1; col <= grid.Columns; ++col) {
					Location location = new Location(row, col);
					Site site = landscape.GetSite(row, col);
					if (nextActiveSite != null && nextActiveSite == location) {
						Assert.AreEqual(location, site.Location);
						Assert.AreEqual(index, site.DataIndex);
						Assert.AreEqual(landscape, site.Landscape);
						Assert.AreEqual(true, site.IsActive);
						index++;
						if (index < activeSites.Count)
							nextActiveSite = activeSites[index];
						else
							nextActiveSite = null;
					}
					else {
						Assert.AreEqual(location, site.Location);
						Assert.AreEqual(inactiveDataIndex, site.DataIndex);
						Assert.AreEqual(landscape, site.Landscape);
						Assert.AreEqual(false, site.IsActive);
					}
				}
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetSite_RowColumn()
		{
			CheckGetSite_RowColumn(landscape);
		}

		//---------------------------------------------------------------------

		[Test]
		public void InputGridCtor_GetSite_RowColumn()
		{
			InputGrid<bool> inputGrid = new InputGrid<bool>(grid);
			ILandscape myLandscape = new Landscape.Landscape(inputGrid);
			CheckGetSite_RowColumn(myLandscape);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetSite_0_0()
		{
			Assert.IsNull(landscape.GetSite(0, 0));
			Assert.IsNull(landscape.GetSite(new Location(0, 0)));
		}

		//---------------------------------------------------------------------

		private ILandscape MakeHomogeneousLandscape(string activeSiteChars)
		{
			string[] rows = new string[]{ "XXXXXXXX",
				                          "XXXXXXXX",
				                          "XXXXXXXX",
				                          "XXXXXXXX",
				                          "XXXXXXXX" };
			bool[,] array = Bool.Make2DimArray(rows, activeSiteChars);
			DataGrid<bool> grid = new DataGrid<bool>(array);
			return new Landscape.Landscape(grid);
		}

		//---------------------------------------------------------------------

		[Test]
		public void AllActiveSites()
		{
			ILandscape landscape = MakeHomogeneousLandscape("X");
			Assert.AreEqual(landscape.SiteCount, landscape.ActiveSiteCount);
			Assert.AreEqual(0, landscape.InactiveSiteCount);

			foreach (Site site in landscape.AllSites) {
				Assert.IsTrue(site.IsActive);
			}

			int index = 0;
			foreach (ActiveSite site in landscape.ActiveSites) {
				Assert.AreEqual(index, site.DataIndex);
				index++;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void AllInactiveSites()
		{
			ILandscape landscape = MakeHomogeneousLandscape("");
			Assert.AreEqual(landscape.SiteCount, landscape.InactiveSiteCount);
			Assert.AreEqual(0, landscape.ActiveSiteCount);

			foreach (Site site in landscape.AllSites) {
				Assert.IsFalse(site.IsActive);
			}

			foreach (ActiveSite site in landscape.ActiveSites) {
				Assert.Fail("Expected no active sites");
			}
		}
	}
}
