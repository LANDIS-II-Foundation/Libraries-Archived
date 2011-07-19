using Landis.PlugIns;
using NUnit.Framework;

namespace Landis.Test.PlugIns
{
    [TestFixture]
    public class Dataset_Test
    {
        private void TryOpenDataset(string filename)
        {
            string path;
            string pathDisplay;
            if (string.IsNullOrEmpty(filename)) {
                path = filename;
                if (filename == null)
                    pathDisplay = "(null)";
                else
                    pathDisplay = "";
            }
            else {
                path = Data.MakeInputPath(filename);
                pathDisplay = path.Replace(Data.Directory,
                                           Data.DirPlaceholder);
            }
            try {
                Data.Output.WriteLine();
                Data.Output.WriteLine("Reading the plug-in database \"{0}\" ...", pathDisplay);
                Dataset dataset = new Dataset(path);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine("Error: {0}",
                                      exc.Message.Replace(Data.Directory,
                                                          Data.DirPlaceholder));
                throw;
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void NullPath()
        {
            TryOpenDataset(null);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentException))]
        public void EmptyPath()
        {
            TryOpenDataset("");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void NonExistentPath()
        {
            TryOpenDataset("NonExistentFile");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void EmptyFile()
        {
            TryOpenDataset("EmptyFile.xml");
        }

        //---------------------------------------------------------------------

        private Dataset LoadDataset(string filename)
        {
            string path = Data.MakeInputPath(filename);
            Data.Output.WriteLine();
            Data.Output.WriteLine("Opening file \"{0}\" ...",
                                  path.Replace(Data.Directory,
                                               Data.DirPlaceholder));
            return new Dataset(path);
        }

        //---------------------------------------------------------------------

        [Test]
        public void NoPlugIns()
        {
            Dataset dataset = LoadDataset("NoPlugIns.xml");
            Assert.AreEqual(0, dataset.Count);
        }

        //---------------------------------------------------------------------

        [Test]
        public void TwoPlugIns()
        {
            Dataset dataset = LoadDataset("TwoPlugIns.xml");
            Assert.AreEqual(2, dataset.Count);

            PlugInInfo info = dataset["Foo"];
            Assert.IsNotNull(info);
            Assert.AreEqual("Foo", info.Name);
            Assert.AreEqual("disturbance:foo", info.PlugInType.Name);
            Assert.AreEqual("Com.Foo.PlugIn,Com.Foo", info.ImplementationName);

            info = dataset["Bar Extension"];
            Assert.IsNotNull(info);
            Assert.AreEqual("Bar Extension", info.Name);
            Assert.AreEqual("output", info.PlugInType.Name);
            Assert.AreEqual("Org.Bar.PlugIn,Org.Bar", info.ImplementationName);
        }

        //---------------------------------------------------------------------

/*        
        private DatasetEntry fooPlugInEntry;
        private DatasetEntry barPlugInEntry;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            fooPlugInEntry = new DatasetEntry();
            fooPlugInEntry.Name = FooPlugIn.PlugInName;
            fooPlugInEntry.InterfaceType = typeof(IDisturbance);
            fooPlugInEntry.ImplementationName = typeof(FooPlugIn).AssemblyQualifiedName;
            fooPlugInEntry.Description = "Foo disturbance plug-in for testing";
            fooPlugInEntry.Version = new System.Version("1.0");
            fooPlugInEntry.CoreVersion = new System.Version("5.0");
            fooPlugInEntry.UserGuidePath = "C:/projects/foo/UserGuide.pdf";

            barPlugInEntry = new DatasetEntry();
            barPlugInEntry.Name = BarPlugIn.PlugInName;
            barPlugInEntry.InterfaceType = typeof(IOutput);
            barPlugInEntry.ImplementationName = typeof(BarPlugIn).AssemblyQualifiedName;
            barPlugInEntry.Description = "Bar output plug-in for testing";
            barPlugInEntry.Version = new System.Version("2.3");
            barPlugInEntry.CoreVersion = new System.Version("5.4");
            barPlugInEntry.UserGuidePath = "C:/projects/BAR/UserGuide.pdf";
        }

        //---------------------------------------------------------------------

        private void AssertEntriesAreEqual(IDatasetEntry expected,
                                           IDatasetEntry actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.InterfaceType, actual.InterfaceType);
            Assert.AreEqual(expected.ImplementationName, actual.ImplementationName);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.ClassName, actual.ClassName);
            Assert.AreEqual(expected.AssemblyName, actual.AssemblyName);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.CoreVersion, actual.CoreVersion);
            Assert.AreEqual(expected.UserGuidePath, actual.UserGuidePath);
            AssertListsAreEqual(expected.ReferencedAssemblies,
                                actual.ReferencedAssemblies);
        }

        //---------------------------------------------------------------------

        private void AssertListsAreEqual(IList<string> expected,
                                         IList<string> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            foreach (string str in expected)
                Assert.IsTrue(actual.Contains(str));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Create()
        {
            EditableDataset dataset = EditableDataset.Create();
            Assert.IsNotNull(dataset);
            Assert.AreEqual(0, dataset.Count);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void AddNull()
        {
            EditableDataset dataset = EditableDataset.Create();
            dataset.Add(null);
        }

        //---------------------------------------------------------------------

        private EditableDataset CreateDatasetAndAddEntry()
        {
            EditableDataset dataset = EditableDataset.Create();
            dataset.Add(fooPlugInEntry);
            Assert.AreEqual(1, dataset.Count);

            IList<string> libs = fooPlugInEntry.ReferencedAssemblies;
            AssertListsAreEqual(libs, dataset.ReferencedByEntries(libs));

            IDatasetEntry foundEntry = dataset.Find(fooPlugInEntry.Name);
            Assert.IsNotNull(foundEntry);
            AssertEntriesAreEqual(fooPlugInEntry, foundEntry);

            return dataset;
        }

        //---------------------------------------------------------------------

        [Test]
        public void Create_Add()
        {
            CreateDatasetAndAddEntry();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ApplicationException))]
        public void AddSameNameTwice()
        {
            try {
                CreateDatasetAndAddEntry().Add(fooPlugInEntry);
            }
            catch (System.Exception e) {
                Data.Output.WriteLine(e.Message);
                Data.Output.WriteLine();
                throw;
            }
        }

        //---------------------------------------------------------------------

        [Test]
        public void Create_Add_Remove()
        {
            EditableDataset dataset = CreateDatasetAndAddEntry();
            IDatasetEntry removedEntry = dataset.Remove(fooPlugInEntry.Name);
            Assert.AreEqual(0, dataset.Count);
            Assert.IsNotNull(removedEntry);
            AssertEntriesAreEqual(fooPlugInEntry, removedEntry);

            IList<string> libs = fooPlugInEntry.ReferencedAssemblies;
            AssertListsAreEqual(new List<string>(),
                                dataset.ReferencedByEntries(libs));

        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void RemoveNull()
        {
            EditableDataset dataset = EditableDataset.Create();
            dataset.Remove(null);
        }

        //---------------------------------------------------------------------

        [Test]
        public void RemoveFromEmpty()
        {
            EditableDataset dataset = EditableDataset.Create();
            IDatasetEntry removedEntry = dataset.Remove(fooPlugInEntry.Name);
            Assert.AreEqual(0, dataset.Count);
            Assert.IsNull(removedEntry);
        }

        //---------------------------------------------------------------------

        [Test]
        public void RemoveUnknownName()
        {
            EditableDataset dataset = CreateDatasetAndAddEntry();
            IDatasetEntry removedEntry = dataset.Remove("Unknown name");
            Assert.AreEqual(1, dataset.Count);
            Assert.IsNull(removedEntry);
        }

        //---------------------------------------------------------------------

        [Test]
        public void SaveAs_Load()
        {
            EditableDataset dataset = CreateDatasetAndAddEntry();
            string filename = Data.MakeOutputPath("SaveAs_Load.landis");
            dataset.SaveAs(filename);
            dataset = null;

            EditableDataset loadedDataset = EditableDataset.Load(filename);
            Assert.IsNotNull(loadedDataset);
            Assert.AreEqual(1, loadedDataset.Count);
            AssertEntriesAreEqual(fooPlugInEntry, loadedDataset[0]);

            IList<string> libs = fooPlugInEntry.ReferencedAssemblies;
            AssertListsAreEqual(libs, loadedDataset.ReferencedByEntries(libs));
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void SaveAs_Null()
        {
            EditableDataset dataset = EditableDataset.Create();
            dataset.SaveAs(null);
        }

        //---------------------------------------------------------------------

        [Test]
        public void SaveAs_Load_Save_Load()
        {
            EditableDataset dataset = CreateDatasetAndAddEntry();
            string filename = Data.MakeOutputPath("SaveAs_Load_Save_Load.landis");
            dataset.SaveAs(filename);
            dataset = null;

            EditableDataset loadedDataset = EditableDataset.Load(filename);
            Assert.IsNotNull(loadedDataset);
            Assert.AreEqual(1, loadedDataset.Count);

            loadedDataset.Add(barPlugInEntry);
            Assert.AreEqual(2, loadedDataset.Count);

            loadedDataset.Save();
            loadedDataset = null;

            EditableDataset reloadedDataset = EditableDataset.Load(filename);
            Assert.AreEqual(2, reloadedDataset.Count);
            AssertEntriesAreEqual(fooPlugInEntry,
                                  reloadedDataset.Find(fooPlugInEntry.Name));
            AssertEntriesAreEqual(barPlugInEntry,
                                  reloadedDataset.Find(barPlugInEntry.Name));
        }

        //---------------------------------------------------------------------

        [Test]
        public void ReferencedByAfterRemove()
        {
            EditableDataset dataset = CreateDatasetAndAddEntry();
            dataset.Add(barPlugInEntry);

            //    Both plug-ins reference the same lib (nunit.framework)
            List<string> libs = new List<string>();
            libs.Add("nunit.framework");
            AssertListsAreEqual(libs, dataset.ReferencedByEntries(libs));

            dataset.Remove(fooPlugInEntry.Name);
            AssertListsAreEqual(libs, dataset.ReferencedByEntries(libs));

            dataset.Remove(barPlugInEntry.Name);
            AssertListsAreEqual(new List<string>(),
                                dataset.ReferencedByEntries(libs));
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void Save_NonLoadedDataset()
        {
            EditableDataset dataset = EditableDataset.Create();
            dataset.Save();
        }

        //---------------------------------------------------------------------

        private void TryLoad(string path)
        {
            try {
                path = Data.MakeInputPath(path);
                Data.Output.WriteLine("Loading plug-in dataset from \"{0}\" ...",
                                      path.Replace(Data.Directory, Data.DirPlaceholder));
                EditableDataset dataset = EditableDataset.Load(path);
            }
            catch (System.Exception exc) {
                Data.Output.WriteLine("  {0}", exc.Message);
                Data.Output.WriteLine();
                throw;
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ApplicationException))]
        public void Load_NonLandisIIBinaryFile()
        {
            TryLoad("SerializedDouble.bin");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ApplicationException))]
        public void Load_WrongIdentifier()
        {
            string path = Data.MakeOutputPath("FooIdentifier.bin");
            Edu.Wisc.Forest.Flel.Util.OutputBinaryFile file;
            using (file = new Edu.Wisc.Forest.Flel.Util.OutputBinaryFile(path)) {
                file.Serialize(new BinaryFileIdentifier("Foo"));
            }
            TryLoad(path);
        }
*/
    }
}
