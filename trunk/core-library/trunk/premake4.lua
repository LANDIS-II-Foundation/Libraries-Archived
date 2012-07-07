thirdPartyDir = "third-party"
thirdPartyLibs = {
  FLEL       = thirdPartyDir .. "/FLEL/util/bin/Edu.Wisc.Forest.Flel.Util.dll",
  LSML       = thirdPartyDir .. "/LSML/Landis.SpatialModeling.dll",
  log4net    = thirdPartyDir .. "/log4net/bin/log4net.dll",
  Troschuetz = thirdPartyDir .. "/Troschuetz/Troschuetz.Random.dll"
}

-- Are we running on Windows?
if string.match(_PREMAKE_VERSION, "^4.[123]") then
  -- Premake 4.3 or earlier.  Since os.getversion() added in Premake 4.4, use
  -- a simple test (does PATH env var have ";"?) to determine if on Windows.
  onWindows = string.find(os.getenv("PATH"), ";")
else
  -- Premake 4.4 or later
  local osVersion = os.getversion()
  onWindows = string.find(osVersion.description, "Windows")
end

-- Fetch LSML if it's not present and we're generating project files
if _ACTION and _ACTION ~= "clean" then
  if not os.isfile(thirdPartyLibs["LSML"]) then
    print("Fetching LSML ...")
    local scriptExt = iif(onWindows, "cmd", "sh")
    local LSMLscript = thirdPartyDir .. "/LSML/get-LSML." .. scriptExt
    if onWindows then
      LSMLscript = path.translate(LSMLscript, "\\")
    end
    os.execute(LSMLscript)
  end
end

buildDir = "build"

-- ==========================================================================

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

-- ==========================================================================

-- MonoDevelop cannot find referenced assemblies which have paths in their
-- Include attribute (see https://bugzilla.xamarin.com/show_bug.cgi?id=6008).

-- This function reads in a *.csproj file, and looks for Reference elements
-- with a path in their Include attribute.  For each such Reference, the
-- path is removed from its Include attribute, and inserted into a new
-- HintPath element.  For example, this line:
--
--    <Reference Include="some\path\to\Example.Assembly.dll" />
--
-- is replaced with 3 lines:
--
--    <Reference Include="Example.Assembly">
--      <HintPath>some\path\to\Example.Assembly.dll</HintPath>
--    </Reference>
--
-- If the function does no replacements, then it returns nil.  If it does
-- at least one replacement, it returns a table with all the file's lines
-- (including all the replacements).

function adjustReferencePaths(csprojPath)
  local referenceAdjusted = false
  local lines = { }
  for line in io.lines(csprojPath) do
    -- Look for <Reference Include="some\path\to\Example.Assembly.dll" />
    local pattern = "(%s*)<Reference%s+Include=\"(.*)\\(.+)\""
    local indent, path, fileName = string.match(line, pattern)
    if path then
      fileNoExt = string.gsub(fileName, "\.[^.]+$", "")
      local newLines = {
        string.format("<Reference Include=\"%s\">", fileNoExt) ,
        string.format("  <HintPath>%s\\%s</HintPath>", path, fileName) ,
	              "</Reference>"
      }
      for _, newLine in ipairs(newLines) do
        table.insert(lines, indent .. newLine)
      end
      referenceAdjusted = true
    else
      table.insert(lines, line)
    end
  end -- for each line in file
  if referenceAdjusted then
    return lines
  else
    return nil
  end
end


-- The function below modifies the all the projects' *.csproj files, by
-- changing each Reference that has a path in its Include attribute to use
-- a HintPath element instead.

function addHintPaths()
  for i, prj in ipairs(solution().projects) do
    local csprojFile = prj.name .. ".csproj"
    locationRelPath = path.getrelative(os.getcwd(), prj.location)
    local csprojRelPath = locationRelPath .. "/" .. csprojFile
    local csprojAbsPath = prj.location .. "/" .. csprojFile
    if not os.isfile(csprojAbsPath) then
      print(csprojRelPath .. " does not exist; Generate project files first")
    else
      print("Reading " .. csprojRelPath .. " ...")
      local adjustedLines = adjustReferencePaths(csprojAbsPath)
      if adjustedLines then
	local outFile, errMessage = io.open(csprojAbsPath, "w")
        if not outFile then
          error(string.format("Cannot open \"%s\" for writing: %s",
                              csprojRelPath, errMessage))
        end
	for _, line in ipairs(adjustedLines) do
          outFile:write(line .. "\n")
        end
        outFile:close()
        print("  <HintPath> elements added to the file")
      else
        print("  No <HintPath> elements added; file not modified")
      end
    end
  end -- for each project
end


newaction {
  trigger = "add-hintpaths",
  description = "Add <HintPath> to references with paths (for MonoDevelop)",
  execute = addHintPaths
}
