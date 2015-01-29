project "QtSharp"

  kind  "SharedLib"
  language "C#"
  flags { "Unsafe" }

  SetupCppSharp()
  SetupManagedProject()

  files { "*.cs" }
  excludes { "QEvent*", "Marshal*", "QObject*" }

  libdirs { "../References" }
  links
  {
  	"System",
  	"System.Core",
  	"System.Data",
  	"Mono.Data.Sqlite",
  	"zlib.net",
  }
