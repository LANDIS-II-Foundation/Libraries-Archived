using System.Reflection;

[assembly: AssemblyTitle("Biomass-Library")]
[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyDescription("Biomass Library for LANDIS-II")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif