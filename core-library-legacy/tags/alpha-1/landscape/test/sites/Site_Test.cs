using NUnit.Framework;

using Landis.Landscape;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Test
{
	[TestFixture]
	public class Site_Test
	{
		private bool[,]    activeSites;
		private ILandscape landscape;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			                            // 12345678
			string[] rows = new string[]{ "...XX...",   // 1
				                          "..XXX..X",   // 2
				                          ".XXXX.XX",   // 3
				                          "...XXXX.",   // 4
				                          "....XX..",   // 5
			                              "........" }; // 6
			activeSites = Bool.Make2DimArray(rows, "X");
			DataGrid<bool> grid = new DataGrid<bool>(activeSites);
			landscape = new Landscape.Landscape(grid);
		}

		//---------------------------------------------------------------------

		private void CheckNeighbor(Site     neighbor,
		                           Location expectedLocation)
		{
			Assert.IsNotNull(neighbor);
			Assert.AreEqual(expectedLocation, neighbor.Location);
			Assert.AreEqual(activeSites[expectedLocation.Row - 1,
			                            expectedLocation.Column - 1],
			                neighbor.IsActive);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ActiveSite_NeighborEast()
		{
			Site site = landscape[3, 7];
			Assert.IsTrue(site.IsActive);
			Site neighbor = site.GetNeighbor(new RelativeLocation(0, 1));
			CheckNeighbor(neighbor, new Location(3, 8));
		}

		//---------------------------------------------------------------------

		[Test]
		public void ActiveSite_NeighborWest()
		{
			Site site = landscape[3, 7];
			Assert.IsTrue(site.IsActive);
			Site neighbor = site.GetNeighbor(new RelativeLocation(0, -1));
			CheckNeighbor(neighbor, new Location(3, 6));
		}

		//---------------------------------------------------------------------

		[Test]
		public void ActiveSite_NeighborNorth()
		{
			Site site = landscape[3, 7];
			Assert.IsTrue(site.IsActive);
			Site neighbor = site.GetNeighbor(new RelativeLocation(-1, 0));
			CheckNeighbor(neighbor, new Location(2, 7));
		}

		//---------------------------------------------------------------------

		[Test]
		public void ActiveSite_NeighborSouth()
		{
			Site site = landscape[3, 7];
			Assert.IsTrue(site.IsActive);
			Site neighbor = site.GetNeighbor(new RelativeLocation(1, 0));
			CheckNeighbor(neighbor, new Location(4, 7));
		}

		//---------------------------------------------------------------------

		[Test]
		public void UpperLeft_8Neighbors()
		{
			Site site = landscape.GetSite(1, 1);
			Assert.IsNull(site.GetNeighbor(new RelativeLocation(-1, -1)));
			Assert.IsNull(site.GetNeighbor(new RelativeLocation(-1, 0)));
			Assert.IsNull(site.GetNeighbor(new RelativeLocation(-1, 1)));
			Assert.IsNull(site.GetNeighbor(new RelativeLocation(0, -1)));
			Assert.IsNull(site.GetNeighbor(new RelativeLocation(1, -1)));

			Site neighbor;
			neighbor = site.GetNeighbor(new RelativeLocation(0, 1));
			CheckNeighbor(neighbor, new Location(1, 2));
			neighbor = site.GetNeighbor(new RelativeLocation(1, 0));
			CheckNeighbor(neighbor, new Location(2, 1));
			neighbor = site.GetNeighbor(new RelativeLocation(1, 1));
			CheckNeighbor(neighbor, new Location(2, 2));
		}
	}
}
