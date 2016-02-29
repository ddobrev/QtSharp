project "QtSharp"

  kind  "SharedLib"
  language "C#"
  flags { "Unsafe" }

  SetupManagedProject()

  files { "*.cs" }
  excludes { "QEvent*", "Marshal*", "QObject*", "QChar*", "IQ*" }

  libdirs
  { 
    "../References",
    "../deps/HtmlAgilityPack.1.4.9/lib/Net45/",
    "../deps/zlib.net.1.0.4/lib/"
  }

  links
  {
    "CppSharp",
    "CppSharp.AST",
    "CppSharp.Generator",
    "System",
    "System.Core",
    "System.Data",
    "System.Xml",
    "System.Xml.Linq",
    "Mono.Data.Sqlite",
    "zlib.net",
    "HtmlAgilityPack"
  }

  SetupParser()
