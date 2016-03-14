using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Landis.Test
{
	[TestFixture]
	public class SiteVar_Test
	{
		//  Local class for testing site variable whose type is a reference type
		public class Foo
		{
			public readonly int Value;

			public Foo(int value)
			{
				Value = value;
			}
		}

		//---------------------------------------------------------------------

		private DataGrid<bool> grid;
		private Landscape.Landscape landscape;
		private ISiteVar<int> intShare;
		private ISiteVar<int> intDistinct;
		private ISiteVar<Foo> objShare;
		private ISiteVar<Foo> objDistinct;
		private Foo[] fooObjs;

		//---------------------------------------------------------------------

		[SetUp]
		public void Init()
		{
			string path = Path.Combine(Data.Directory, "mixed.txt");
			bool[,] array = Bool.Read2DimArray(path);
			grid = new DataGrid<bool>(array);
			landscape = new Landscape.Landscape(grid);

			intShare = landscape.NewSiteVar<int>(InactiveSiteMode.Share1Value);
			intDistinct = landscape.NewSiteVar<int>(InactiveSiteMode.DistinctValues);

			objShare = landscape.NewSiteVar<Foo>(InactiveSiteMode.Share1Value);
			objDistinct = landscape.NewSiteVar<Foo>(InactiveSiteMode.DistinctValues);

			fooObjs = new Foo[landscape.SiteCount];
			for (int i = 0; i < fooObjs.Length; ++i)
				fooObjs[i] = new Foo(i+1);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Int_NoModeArg()
		{
			ISiteVar<int> var = landscape.NewSiteVar<int>();
			Assert.AreEqual(typeof(int), var.DataType);
			Assert.AreEqual(InactiveSiteMode.Share1Value, var.Mode);
			Assert.AreSame(landscape, var.Landscape);

			foreach (Site site in landscape.AllSites)
				Assert.AreEqual(0, var[site]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntShare()
		{
			Assert.AreEqual(typeof(int), intShare.DataType);
			Assert.AreEqual(InactiveSiteMode.Share1Value, intShare.Mode);
			Assert.AreSame(landscape, intShare.Landscape);
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntShare_SetAndGet()
		{
			int count = 0;
			int lastInactiveSite = -1;
			foreach (Site site in landscape.AllSites) {
				count++;
				intShare[site] = count;
				if (! site.IsActive)
					lastInactiveSite = count;
			}
			count = 0;
			foreach (Site site in landscape.AllSites) {
				count++;
				if (site.IsActive)
					Assert.AreEqual(count, intShare[site]);
				else
					Assert.AreEqual(lastInactiveSite, intShare[site]);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntShare_ActiveSiteValues()
		{
			foreach (Site site in landscape.AllSites) {
				intShare[site] = 9876;
			}
			intShare.ActiveSiteValues = -100;
			foreach (Site site in landscape.AllSites) {
				if (site.IsActive)
					Assert.AreEqual(-100, intShare[site]);
				else
					Assert.AreEqual(9876, intShare[site]);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntShare_InactiveSiteValues()
		{
			foreach (Site site in landscape.AllSites) {
				intShare[site] = 987;
			}
			intShare.InactiveSiteValues = -1;
			foreach (Site site in landscape.AllSites) {
				if (site.IsActive)
					Assert.AreEqual(987, intShare[site]);
				else
					Assert.AreEqual(-1, intShare[site]);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntShare_SiteValues()
		{
			foreach (Site site in landscape.AllSites) {
				intShare[site] = 4444;
			}
			intShare.SiteValues = -22;
			foreach (Site site in landscape.AllSites) {
				Assert.AreEqual(-22, intShare[site]);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntDistinct()
		{
			Assert.AreEqual(typeof(int), intDistinct.DataType);
			Assert.AreEqual(InactiveSiteMode.DistinctValues, intDistinct.Mode);
			Assert.AreSame(landscape, intDistinct.Landscape);
		}

		//---------------------------------------------------------------------

		private void SetEachSiteToCount(ISiteVar<int> intVar)
		{
			int count = 0;
			foreach (Site site in landscape.AllSites) {
				count++;
				intVar[site] = count;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntDistinct_SetAndGet()
		{
			SetEachSiteToCount(intDistinct);
			int count = 0;
			foreach (Site site in landscape.AllSites) {
				count++;
				Assert.AreEqual(count, intDistinct[site]);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntDistinct_ActiveSiteValues()
		{
			SetEachSiteToCount(intDistinct);
			intDistinct.ActiveSiteValues = -100;
			int count = 0;
			foreach (Site site in landscape.AllSites) {
				count++;
				if (site.IsActive)
					Assert.AreEqual(-100, intDistinct[site]);
				else
					Assert.AreEqual(count, intDistinct[site]);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntDistinct_InactiveSiteValues()
		{
			SetEachSiteToCount(intDistinct);
			intDistinct.InactiveSiteValues = -100;
			int count = 0;
			foreach (Site site in landscape.AllSites) {
				count++;
				if (site.IsActive)
					Assert.AreEqual(count, intDistinct[site]);
				else
					Assert.AreEqual(-100, intDistinct[site]);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void IntDistinct_SiteValues()
		{
			SetEachSiteToCount(intDistinct);
			intDistinct.SiteValues = -100;
			foreach (Site site in landscape.AllSites) {
				Assert.AreEqual(-100, intDistinct[site]);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void Obj_NoModeArg()
		{
			ISiteVar<Foo> var = landscape.NewSiteVar<Foo>();
			Assert.AreEqual(typeof(Foo), var.DataType);
			Assert.AreEqual(InactiveSiteMode.Share1Value, var.Mode);
			Assert.AreSame(landscape, var.Landscape);

			foreach (Site site in landscape.AllSites)
				Assert.AreEqual(null, var[site]);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ObjShare()
		{
			Assert.AreEqual(typeof(Foo), objShare.DataType);
			Assert.AreEqual(InactiveSiteMode.Share1Value, objShare.Mode);
			Assert.AreSame(landscape, objShare.Landscape);
		}

		//---------------------------------------------------------------------

		private Foo SetToFooObjs(ISiteVar<Foo> fooVar)
		{
			int i = 0;
			Foo lastInactiveSite = null;
			foreach (Site site in landscape.AllSites) {
				fooVar[site] = fooObjs[i];
				if (! site.IsActive)
					lastInactiveSite = fooObjs[i];
				i++;
			}
			return lastInactiveSite;
		}

		//---------------------------------------------------------------------

		[Test]
		public void ObjShare_SetAndGet()
		{
			Foo lastInactiveSite = SetToFooObjs(objShare);
			int index = 0;
			foreach (Site site in landscape.AllSites) {
				if (site.IsActive)
					Assert.AreSame(fooObjs[index], objShare[site]);
				else
					Assert.AreSame(lastInactiveSite, objShare[site]);
				index++;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void ObjShare_ActiveSiteValues()
		{
			Foo lastInactiveSite = SetToFooObjs(objShare);
			Foo expectedFoo = new Foo(-100);
			objShare.ActiveSiteValues = expectedFoo;

			foreach (Site site in landscape.AllSites) {
				if (site.IsActive)
					Assert.AreSame(expectedFoo, objShare[site]);
				else
					Assert.AreSame(lastInactiveSite, objShare[site]);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void ObjShare_InactiveSiteValues()
		{
			Foo lastInactiveSite = SetToFooObjs(objShare);
			Foo expectedFoo = new Foo(-100);
			objShare.InactiveSiteValues = expectedFoo;

			int index = 0;
			foreach (Site site in landscape.AllSites) {
				if (site.IsActive)
					Assert.AreSame(fooObjs[index], objShare[site]);
				else
					Assert.AreSame(expectedFoo, objShare[site]);
				index++;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void ObjShare_SiteValues()
		{
			Foo lastInactiveSite = SetToFooObjs(objShare);
			Foo expectedFoo = new Foo(-22);
			objShare.SiteValues = expectedFoo;

			foreach (Site site in landscape.AllSites) {
				Assert.AreSame(expectedFoo, objShare[site]);
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void ObjDistinct()
		{
			Assert.AreEqual(typeof(Foo), objDistinct.DataType);
			Assert.AreEqual(InactiveSiteMode.DistinctValues, objDistinct.Mode);
			Assert.AreSame(landscape, objDistinct.Landscape);
		}

		//---------------------------------------------------------------------

		[Test]
		public void ObjDistinct_SetAndGet()
		{
			Foo lastInactiveSite = SetToFooObjs(objDistinct);
			int index = 0;
			foreach (Site site in landscape.AllSites) {
				Assert.AreSame(fooObjs[index], objDistinct[site]);
				index++;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void ObjDistinct_ActiveSiteValues()
		{
			Foo lastInactiveSite = SetToFooObjs(objDistinct);
			Foo expectedFoo = new Foo(-9);
			objDistinct.ActiveSiteValues = expectedFoo;

			int index = 0;
			foreach (Site site in landscape.AllSites) {
				if (site.IsActive)
					Assert.AreSame(expectedFoo, objDistinct[site]);
				else
					Assert.AreSame(fooObjs[index], objDistinct[site]);
				index++;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void ObjDistinct_InactiveSiteValues()
		{
			Foo lastInactiveSite = SetToFooObjs(objDistinct);
			Foo expectedFoo = new Foo(-1234);
			objDistinct.InactiveSiteValues = expectedFoo;

			int index = 0;
			foreach (Site site in landscape.AllSites) {
				if (site.IsActive)
					Assert.AreSame(fooObjs[index], objDistinct[site]);
				else
					Assert.AreSame(expectedFoo, objDistinct[site]);
				index++;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void ObjDistinct_SiteValues()
		{
			Foo lastInactiveSite = SetToFooObjs(objDistinct);
			Foo expectedFoo = new Foo(-333);
			objDistinct.SiteValues = expectedFoo;

			foreach (Site site in landscape.AllSites) {
				Assert.AreSame(expectedFoo, objDistinct[site]);
			}
		}
	}
}
