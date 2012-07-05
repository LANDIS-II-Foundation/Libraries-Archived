thirdPartyDir = "third-party"
thirdPartyLibs = {
  FLEL       = thirdPartyDir .. "/FLEL/util/bin/Edu.Wisc.Forest.Flel.Util.dll",
  LSML       = thirdPartyDir .. "/LSML/Landis.SpatialModeling.dll",
  log4net    = thirdPartyDir .. "/log4net/bin/log4net.dll",
  Troschuetz = thirdPartyDir .. "/Troschuetz/Troschuetz.Random.dll"
}

buildDir = "build"

solution "LANDIS-II_core"

  language "C#"    -- by default, Premake uses "Any CPU" for platform
  framework "3.5"

  configurations { "Debug", "Release" }
 
  configuration "Debug"
    defines { "DEBUG" }
    flags { "Symbols" }
    targetdir ( buildDir .. "/Debug" )
 
  configuration "Release"
    flags { "OptimizeSize" }
    targetdir ( buildDir .. "/Release" )
 
  -- The core's API (referenced by LANDIS-II extensions)
  project "Landis_Core"
    location "src"
    kind "SharedLib"
    targetname "Landis.Core"
    files { "src/*.cs" }
    links {
      "System",
      "System.Core",
      thirdPartyLibs["FLEL"],
      thirdPartyLibs["LSML"],
      thirdPartyLibs["Troschuetz"]
    }

  -- The core's implementation
  project "Landis_Core_Implementation"
    location "src/Implementation"
    kind "SharedLib"
    targetname "Landis.Core.Implementation"
    files { "src/Implementation/**.cs" }
    links {
      "Landis_Core",
      "System",
      "System.Core",
      thirdPartyLibs["FLEL"],
      thirdPartyLibs["LSML"],
      thirdPartyLibs["log4net"],
      thirdPartyLibs["Troschuetz"]
    }
