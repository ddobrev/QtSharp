-- This is the starting point of the build scripts for the project.
-- It defines the common build settings that all the projects share
-- and calls the build scripts of all the sub-projects.

dofile "Helpers.lua"

solution "QtSharp"

  configurations { "Debug", "Release" }
  platforms { "x32", "x64" }
  flags { common_flags }
  
  location (builddir)
  objdir (path.join(builddir, "obj"))
  targetdir (libdir)
  debugdir (bindir)

  -- startproject "Generator"
  configuration "vs2013"
    framework "4.0"

  configuration "vs2012"
    framework "4.0"

  configuration "windows"
    defines { "WINDOWS" }
	
  include (srcdir .. "/QtSharp")
  include (srcdir .. "/QtSharp.CLI")
  include (srcdir .. "/QtSharp.Tests")
