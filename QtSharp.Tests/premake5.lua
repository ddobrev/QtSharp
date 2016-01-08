project "QtSharp.Tests"

  kind  "SharedLib"
  language "C#"
  flags { "Unsafe" }
  
  SetupManagedProject()

  files { "*.cs" }
  links
  {
    "CppSharp",
    "CppSharp.AST",
    "CppSharp.Parser.CLI",
    "CppSharp.Generator",  
    "System",
    "System.Core"
  }
