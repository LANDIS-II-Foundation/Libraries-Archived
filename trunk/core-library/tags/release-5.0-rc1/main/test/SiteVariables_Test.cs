using Edu.Wisc.Forest.Flel.Util;
using Landis.Ecoregions;
using Landis.Landscape;
using NUnit.Framework;

namespace Landis.Test.Main
{
	[TestFixture]
	public class SiteVariables_Test
	{
		private ILandscape landscape;
		private Ecoregions.Map ecoregionsMap;
		private SiteVariables siteVars;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			bool[,] array = new bool[0,0];
			DataGrid<bool> grid = new DataGrid<bool>(array);
			landscape = new Landscape.Landscape(grid);

			Ecoregions.IDataset ecoregions = new Ecoregions.Dataset(null);
			ecoregionsMap = new Ecoregions.Map(Data.MakeInputPath("ecoregions-0by0.gis"),
			                                   ecoregions);
			siteVars = new SiteVariables(landscape, ecoregionsMap);
		}

		//---------------------------------------------------------------------

		private void TryRegister(ISiteVariable siteVar,
		                         string        name)
		{
			try {
				siteVars.RegisterVar(siteVar, name);
			}
			catch (System.Exception exc) {
				Data.Output.WriteLine(exc.Message);
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void Register_NullVar()
		{
			TryRegister(null, "");
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void Register_NullName()
		{
			ISiteVar<bool> var = landscape.NewSiteVar<bool>();
			TryRegister(var, null);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void Register_SameName()
		{
			ISiteVar<bool> var = landscape.NewSiteVar<bool>();
			string name = "foo";
			siteVars.RegisterVar(var, name);
			TryRegister(var, name);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetVar()
		{
			ISiteVar<bool> var = landscape.NewSiteVar<bool>();
			string name = "My Site Variable";
			SiteVariables vars = new SiteVariables(landscape, ecoregionsMap);
			vars.RegisterVar(var, name);

			ISiteVar<bool> fetchedVar = vars.GetVar<bool>(name);
			Assert.IsNotNull(fetchedVar);
			Assert.AreEqual(var, fetchedVar);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetVar_NameNotRegistered()
		{
			SiteVariables vars = new SiteVariables(landscape, ecoregionsMap);

			ISiteVar<bool> fetchedVar = vars.GetVar<bool>("Should not exist");
			Assert.IsNull(fetchedVar);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.ApplicationException))]
		public void GetVar_TypeMismatch()
		{
			ISiteVar<bool> var = landscape.NewSiteVar<bool>();
			string name = "My Site Variable";
			SiteVariables vars = new SiteVariables(landscape, ecoregionsMap);
			vars.RegisterVar(var, name);

			try {
				ISiteVar<int> fetchedVar = vars.GetVar<int>(name);
			}
			catch (System.Exception exc) {
				Data.Output.WriteLine(exc.Message);
				throw;
			}
		}
	}
}
