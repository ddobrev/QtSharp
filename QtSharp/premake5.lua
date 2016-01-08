project "QtSharp"

  kind  "SharedLib"
  language "C#"
  flags { "Unsafe" }

  SetupManagedProject()

  files { "*.cs" }
  excludes { "QEvent*", "Marshal*", "QObject*" }

  libdirs { "../References" }
  links
  {
    "CppSharp",
    "CppSharp.AST",
    "CppSharp.Parser.CLI",
    "CppSharp.Generator",
  	"System",
  	"System.Core",
  	"System.Data",
  	"Mono.Data.Sqlite",
  	"zlib.net",
  }
