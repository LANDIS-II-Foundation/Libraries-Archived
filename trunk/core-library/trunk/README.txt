The Premake build configuration tool is needed to generate the C# solution
and project files.  Premake is available from:

  http://industriousone.com/premake

For convenience, it's recommended that you install Premake in a folder that
is in your PATH (for example, "My Documents\bin\" on Windows XP).  So, you
can run the tool in the command prompt by simply entering its name.

To allow multiple versions of Premake to be installed side-by-side, it's
recommended that you rename each executable to include its version number.
For example, on Windows,

  premake4.3.exe  -- Premake 4.3, current stable version
  premake4.4.exe  -- Premake 4.4-beta 4, current development version

Premake 4.3 has been used successfully to generate VS2008 project files, by
running this command in the folder with this README.txt file.

  premake4.3 vs2008

In order to generate VS2010 project files, Premake 4.4 (currently, beta 4)
is needed because Premake 4.3 doesn't support VS2010 C# projects.

  premake4.4 vs2010

After running Premake, open the LANDIS-II_core.sln solution file in an IDE
(Visual Studio, MonoDevelop) and build the solution.  If you're using
MonoDevelop, then an extra step is required.  The C# project files must be
modified so that MonoDevelop can properly locate the third-party assemblies.
To modify those project files, run Premake a second time as follows:

  premake4.x add-hintpaths

Replace "premake4.x" with the version of Premake used the first time (i.e.,
either "premake4.3" or "premake4.4").
