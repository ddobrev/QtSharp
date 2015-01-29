project "QtSharp.Tests"

  kind  "SharedLib"
  language "C#"
  flags { "Unsafe" }
  
  SetupManagedProject()

  files { "*.cs" }
  links { "System", "System.Core" }
