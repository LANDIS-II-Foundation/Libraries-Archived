using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

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
				index++;
				Assert.IsTrue(index <= landscape.ActiveSiteCount);
				Assert.AreEqual(index, site.DataIndex);
				Assert.AreEqual(activeSites[index-1], site.Location);
				Assert.AreEqual(landscape, site.Landscape);
				Assert.AreEqual(true, site.IsActive);
				Assert.AreEqual(true, site.IsMutable);
			}
			Assert.AreEqual(landscape.ActiveSiteCount, index);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ActiveSiteEnumerator_Reuse()
		{
			Assert.AreEqual(activeSites.Count, landscape.ActiveSiteCount);

			ActiveSiteEnumerator activeSiteEtor = landscape.GetActiveSiteEnumerator();

			for (int passes = 1; passes <= 3; passes++) {
				int index = 0;
				foreach (ActiveSite site in activeSiteEtor) {
					index++;
					Assert.IsTrue(index <= landscape.ActiveSiteCount);
					Assert.AreEqual(index, site.DataIndex);
					Assert.AreEqual(activeSites[index-1], site.Location);
					Assert.AreEqual(landscape, site.Landscape);
					Assert.AreEqual(true, site.IsActive);
					Assert.AreEqual(true, site.IsMutable);
				}
				Assert.AreEqual(landscape.ActiveSiteCount, index);
			}
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
						Assert.AreEqual(index+1, site.DataIndex);
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
						Assert.AreEqual(index+1, site.DataIndex);
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
						Assert.AreEqual(index+1, site.DataIndex);
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
						Assert.AreEqual(landscape.InactiveSiteDataIndex, site.DataIndex);
						Assert.AreEqual(landscape, site.Landscape);
						Assert.AreEqual(false, site.IsActive);
					}
				}
		}

		//---------------------------------------------------------------------

		[Test]
		public void SiteEnumerator()
		{
			int index = 0;
			Location? nextActiveSite;
			if (index < activeSites.Count)
				nextActiveSite = activeSites[index];
			else
				nextActiveSite = null;

			Location expectedLocation = new Location(1, 0);
				// so next in row-major order is (1,1)

			int siteCount = 0;
			foreach (Site site in landscape.AllSites) {
				siteCount++;

				Assert.AreEqual(landscape, site.Landscape);
				Assert.IsTrue(site.IsMutable);

				expectedLocation = RowMajor.Next(expectedLocation, grid.Columns);
				Assert.AreEqual(expectedLocation, site.Location);

				if (nextActiveSite != null && nextActiveSite == expectedLocation) {
					Assert.AreEqual(index+1, site.DataIndex);
					Assert.AreEqual(true, site.IsActive);
					index++;
					if (index < activeSites.Count)
						nextActiveSite = activeSites[index];
					else
						nextActiveSite = null;
				}
				else {
					//	Inactive site
					Assert.AreEqual(landscape.InactiveSiteDataIndex, site.DataIndex);
					Assert.AreEqual(false, site.IsActive);
				}
			}

			Assert.AreEqual(grid.Count, siteCount);
		}

		//---------------------------------------------------------------------

		[Test]
		public void SiteEnumerator_Reuse()
		{
			SiteEnumerator allSites = landscape.GetSiteEnumerator();

			for (int passes = 1; passes <= 3; passes++) {
				int index = 0;
				Location? nextActiveSite;
				if (index < activeSites.Count)
					nextActiveSite = activeSites[index];
				else
					nextActiveSite = null;
	
				Location expectedLocation = new Location(1, 0);
					// so next in row-major order is (1,1)
	
				int siteCount = 0;
				foreach (Site site in allSites) {
					siteCount++;
	
					Assert.AreEqual(landscape, site.Landscape);
					Assert.IsTrue(site.IsMutable);
	
					expectedLocation = RowMajor.Next(expectedLocation, grid.Columns);
					Assert.AreEqual(expectedLocation, site.Location);
	
					if (nextActiveSite != null && nextActiveSite == expectedLocation) {
						Assert.AreEqual(index+1, site.DataIndex);
						Assert.AreEqual(true, site.IsActive);
						index++;
						if (index < activeSites.Count)
							nextActiveSite = activeSites[index];
						else
							nextActiveSite = null;
					}
					else {
						//	Inactive site
						Assert.AreEqual(landscape.InactiveSiteDataIndex, site.DataIndex);
						Assert.AreEqual(false, site.IsActive);
					}
				}
	
				Assert.AreEqual(grid.Count, siteCount);
			}
		}

		//---------------------------------------------------------------------

		private void CheckGetSite_RowColumn(ILandscape landscape)
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
					Site site = landscape.GetSite(row, col);
					if (nextActiveSite != null && nextActiveSite == location) {
						Assert.AreEqual(location, site.Location);
						Assert.AreEqual(index+1, site.DataIndex);
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
						Assert.AreEqual(landscape.InactiveSiteDataIndex, site.DataIndex);
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

		[Test]
		public void GetSite_Mutable_0_0()
		{
			MutableSite site = null;
			Assert.IsFalse(landscape.GetSite(new Location(0, 0), ref site));
			Assert.IsNull(site);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetSite_Mutable()
		{
			MutableSite site = null;
			MutableSite siteFrom1stCall = null;

			int index = 0;
			Location? nextActiveSite;
			if (index < activeSites.Count)
				nextActiveSite = activeSites[index];
			else
				nextActiveSite = null;

			for (uint row = 1; row <= grid.Rows; ++row)
				for (uint col = 1; col <= grid.Columns; ++col) {
					Location location = new Location(row, col);
					Assert.IsTrue(landscape.GetSite(location, ref site));
					if (siteFrom1stCall == null)
						siteFrom1stCall = site;
					else
						Assert.AreSame(siteFrom1stCall, site);
					Assert.IsTrue(site.IsMutable);

					if (nextActiveSite != null && nextActiveSite == location) {
						Assert.AreEqual(location, site.Location);
						Assert.AreEqual(index+1, site.DataIndex);
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
						Assert.AreEqual(landscape.InactiveSiteDataIndex, site.DataIndex);
						Assert.AreEqual(landscape, site.Landscape);
						Assert.AreEqual(false, site.IsActive);
					}
				}
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
				index++;
				Assert.AreEqual(index, site.DataIndex);
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
