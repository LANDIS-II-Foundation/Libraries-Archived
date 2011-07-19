using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

using Landis.Landscape;
using Landis.Util;

namespace Landis.Test
{
	[TestFixture]
	public class Landscape_Test
	{
		private DataGrid<bool> grid;
		private Landscape.Landscape landscape;
		private List<Location> activeSites;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			string path = Path.Combine(Data.UtilBoolTestDir, "mixed.txt");
			bool[,] array = Bool.Read2DimArray(path);
			grid = new DataGrid<bool>(array);
			landscape = new Landscape.Landscape(grid);

			path = Path.Combine(Data.UtilBoolTestDir,
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
			uint inactiveDataIndex = landscape.ActiveSiteCount;

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

		[Test]
		public void GetSite_RowColumn()
		{
			uint inactiveDataIndex = landscape.ActiveSiteCount;

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
		public void IntSiteVar()
		{
			SiteVariable<int> var = new SiteVariable<int>("foo");
			Assert.AreEqual("foo", var.Name);
			Assert.IsNull(var.Landscape);
			Assert.AreEqual(typeof(int), var.DataType);

			landscape.Add(var);
		}
	}
}
