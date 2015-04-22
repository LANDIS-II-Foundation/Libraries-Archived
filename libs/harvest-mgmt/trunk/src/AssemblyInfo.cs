using System.Reflection;

[assembly: AssemblyTitle("Harvest Library")]
[assembly: AssemblyVersion("1.1.*")]
[assembly: AssemblyDescription("Harvest Library for LANDIS-II")]
[assembly: AssemblyCopyright("2015 University of Notre Dame")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif