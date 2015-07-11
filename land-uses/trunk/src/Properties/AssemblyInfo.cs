using System.Reflection;

[assembly: AssemblyProduct("LANDIS-II")]
[assembly: AssemblyTitle("Land-Use Library")]
[assembly: AssemblyCopyright("Copyright 2012 Green Code LLC")]
[assembly: AssemblyVersion("0.1.*")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
