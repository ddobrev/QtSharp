project "QtSharp.CLI"

  kind  "ConsoleApp"
  language "C#"

  SetupManagedProject()

  --debugargs { "C:\Qt\Qt5.3.2\5.3\mingw482_32\bin\qmake.exe C:\Qt\Qt5.3.2\Tools\mingw482_32\bin\mingw32-make.exe" }

  files { "*.cs" }
  links
  {
    "CppSharp",
    "CppSharp.AST",
    "CppSharp.Generator",  
    "System",
    "System.Core",
    "System.IO.Compression",
    "System.IO.Compression.FileSystem",
    "QtSharp"
  }

  SetupParser()
  
