using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using NUnit.Framework;

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

		//---------------------------------------------------------------------

		[Test]
		public void GetNeighbor_Mutable()
		{
			Site site = landscape[3, 7];

			MutableSite neighbor = null;
			Assert.IsTrue(site.GetNeighbor(new RelativeLocation(0, 1), ref neighbor));
			CheckNeighbor(neighbor, new Location(3, 8));

			Assert.IsTrue(site.GetNeighbor(new RelativeLocation(2, 0), ref neighbor));
			CheckNeighbor(neighbor, new Location(5, 7));

			Assert.IsTrue(site.GetNeighbor(new RelativeLocation(0, -4), ref neighbor));
			CheckNeighbor(neighbor, new Location(3, 3));

			Assert.IsTrue(site.GetNeighbor(new RelativeLocation(-1, 0), ref neighbor));
			CheckNeighbor(neighbor, new Location(2, 7));

			Assert.IsTrue(site.GetNeighbor(new RelativeLocation(3, -3), ref neighbor));
			CheckNeighbor(neighbor, new Location(6, 4));
		}

		//---------------------------------------------------------------------

		[Test]
		public void UpperLeft_8Neighbors_Mutable()
		{
			Site site = landscape.GetSite(1, 1);

			//	Check all 8 immediate neighbors starting with the one due
			//	north and working clockwise.
			MutableSite neighbor = null;

			// 	North
			Assert.IsFalse(site.GetNeighbor(new RelativeLocation(-1, 0), ref neighbor));
			Assert.IsNull(neighbor);

			// 	North-east
			Assert.IsFalse(site.GetNeighbor(new RelativeLocation(-1, 1), ref neighbor));
			Assert.IsNull(neighbor);

			//	East
			Assert.IsTrue(site.GetNeighbor(new RelativeLocation(0, 1), ref neighbor));
			CheckNeighbor(neighbor, new Location(1, 2));

			//	South-east
			Assert.IsTrue(site.GetNeighbor(new RelativeLocation(1, 1), ref neighbor));
			CheckNeighbor(neighbor, new Location(2, 2));

			//	South
			Assert.IsTrue(site.GetNeighbor(new RelativeLocation(1, 0), ref neighbor));
			CheckNeighbor(neighbor, new Location(2, 1));

			//	South-west
			Assert.IsFalse(site.GetNeighbor(new RelativeLocation(1, -1), ref neighbor));
			//	Neighbor unchanged, so it stills refers to south neighbor
			CheckNeighbor(neighbor, new Location(2, 1));

			//	West
			Assert.IsFalse(site.GetNeighbor(new RelativeLocation(0, -1), ref neighbor));
			//	Neighbor unchanged, so it stills refers to south neighbor
			CheckNeighbor(neighbor, new Location(2, 1));

			//	North-west
			Assert.IsFalse(site.GetNeighbor(new RelativeLocation(-1, -1), ref neighbor));
			//	Neighbor unchanged, so it stills refers to south neighbor
			CheckNeighbor(neighbor, new Location(2, 1));
		}
	}
}
