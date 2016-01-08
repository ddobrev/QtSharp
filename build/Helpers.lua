-- This module checks for the all the project dependencies.

action = _ACTION or ""

depsdir = path.getabsolute("../deps");
srcdir = path.getabsolute("..");
incdir = path.getabsolute("../include");
bindir = path.getabsolute("../bin");
examplesdir = path.getabsolute("../examples");
testsdir = path.getabsolute("../tests");

builddir = path.getabsolute("./" .. action);
if _ARGS[1] then
    builddir = path.getabsolute("./" .. _ARGS[1]);
end

libdir = path.join(builddir, "lib", "%{cfg.buildcfg}_%{cfg.platform}");
gendir = path.join(builddir, "gen");

common_flags = { "Unicode", "Symbols" }

function os.is_osx()
  return os.is("macosx")
end

function os.is_windows()
  return os.is("windows")
end

function os.is_linux()
  return os.is("linux")
end

function string.starts(str, start)
   return string.sub(str, 1, string.len(start)) == start
end

function SafePath(path)
  return "\"" .. path .. "\""
end

function SetupManagedProject()
  language "C#"
  location (path.join(builddir, "projects"))

  if not os.is_osx() then
    local c = configuration { "vs*" }
      location "."
    configuration(c)
  end
end

