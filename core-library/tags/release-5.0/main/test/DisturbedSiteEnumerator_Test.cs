using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using NUnit.Framework;

namespace Landis.Test.Main
{
	[TestFixture]
	public class DisturbedSiteEnumerator_Test
	{
		[Test]
		public void MixedLandscape()
		{
									// columns:    123456789
			ILandscape landscape = MakeLandscape( "---------",	// row 1
			                                      "---XXXX--",	// row 2
			                                      "--XXXXX--",	// row 3
			                                      "--XXXXXX-",	// row 4
			                                      "-XXX--XX-",	// row 5
			                                      "-XX---XXX",	// row 6
			                                      "--XX--X--");	// row 7

			ISiteVar<bool> disturbed = landscape.NewSiteVar<bool>();
			Location[] disturbedLocs = new Location[] {
				new Location(2,6),
				new Location(3,4),
				new Location(3,5),
				new Location(5,7),
				new Location(5,8),
				new Location(6,2),
				new Location(7,7)
			};
			foreach (Location loc in disturbedLocs) {
				ActiveSite site = landscape[loc];
				Assert.IsNotNull(site);
				disturbed[site] = true;
			}

			DisturbedSiteEnumerator disturbedSites;
			disturbedSites = new DisturbedSiteEnumerator(landscape, disturbed);
			int count = 0;
			foreach (ActiveSite site in disturbedSites) {
				count++;
				Assert.IsTrue(count <= disturbedLocs.Length);
				Assert.AreEqual(disturbedLocs[count-1], site.Location);
			}
			Assert.AreEqual(count, disturbedLocs.Length);
		}

		//---------------------------------------------------------------------

		private ILandscape MakeLandscape(params string[] rows)
		{
			bool[,] array = Bool.Make2DimArray(rows, "X");
			DataGrid<bool> grid = new DataGrid<bool>(array);
			return new Landscape.Landscape(grid);
		}
	}
}
