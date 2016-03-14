using Landis.PlugIns;
using Edu.Wisc.Forest.Flel.Util.PlugIns;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace Landis.Test.PlugIns
{
	//  A null disturbance plug-in.
	public class NullDisturbance
		: IDisturbance
	{
		public const string PlugInName = "Null Disturbance";

		public string Name
		{
			get {
				return PlugInName;
			}
		}

		public int NextTimeToRun
		{
			get {
				return 0;
			}
		}

		public void Initialize(string dataFile)
		{
		}

		public void Run(int currentTime)
		{
		}
	}

	//-------------------------------------------------------------------------

	[TestFixture]
	public class Manager_Test
	{
		private string myAssemblyName;
		private IInfo disturbancePlugInInfo;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			myAssemblyName = Assembly.GetExecutingAssembly().FullName;
			disturbancePlugInInfo = new Info(NullDisturbance.PlugInName,
			                                 typeof(IDisturbance),
			                                 typeof(NullDisturbance).AssemblyQualifiedName);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Initialize()
		{
			Manager.Initialize();
		}

		//---------------------------------------------------------------------

		[Test]
		public void Initialize_ManyPlugIns()
		{
			Manager.Initialize(Data.MakeInputPath("ManyPlugIns.txt"));
		}

		//---------------------------------------------------------------------

		private T LoadPlugIn<T>(IInfo plugInInfo)
		{
			Data.Output.WriteLine("Loading the {0} plug-in \"{1}\"...",
			                      Interface.GetName(plugInInfo.InterfaceType),
			                      plugInInfo.Name);
			return Manager.Load<T>(plugInInfo);
		}

		//---------------------------------------------------------------------

		private void TryLoadPlugIn<T>(IInfo plugInInfo)
		{
			try {
				T plugIn = LoadPlugIn<T>(plugInInfo);
				Assert.IsNotNull(plugIn);
			}
			catch (System.Exception e) {
				Data.Output.WriteLine(e.Message);
				Data.Output.WriteLine();
				throw;
			}
		}

		//---------------------------------------------------------------------

		[Test]
		public void Load_Disturbance()
		{
			Manager.Initialize();
			IDisturbance plugIn = LoadPlugIn<IDisturbance>(disturbancePlugInInfo);
			Assert.IsNotNull(plugIn);
			Assert.AreEqual(NullDisturbance.PlugInName, plugIn.Name);
		}

		//---------------------------------------------------------------------

		private bool disturbancePlugInAdded = false;

		private void AddDisturbancePlugIn()
		{
			if (! disturbancePlugInAdded) {
				Manager.Add(disturbancePlugInInfo);
				disturbancePlugInAdded = true;
			}
		}

		//---------------------------------------------------------------------

		private void AssertInfosAreEqual(IInfo expected,
		                                 IInfo actual)
		{
			Assert.AreEqual(expected.Name, actual.Name);
			Assert.AreEqual(expected.InterfaceType, actual.InterfaceType);
			Assert.AreEqual(expected.ImplementationName, actual.ImplementationName);
		}

		//---------------------------------------------------------------------

		[Test]
		public void GetInfo()
		{
			AddDisturbancePlugIn();
			IInfo info = Manager.GetInfo(disturbancePlugInInfo.Name);
			Assert.IsNotNull(info);
			AssertInfosAreEqual(disturbancePlugInInfo, info);
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(Edu.Wisc.Forest.Flel.Util.PlugIns.Exception))]
		public void GetInfo_WrongType()
		{
			AddDisturbancePlugIn();
			try {
				IInfo info = Manager.GetInfo(disturbancePlugInInfo.Name,
				                             typeof(IOutput));
			}
			catch (System.Exception exc) {
				Data.Output.WriteLine(exc.Message);
				Data.Output.WriteLine();
				throw;
			}
		}
	}
}
