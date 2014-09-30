using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace QtSharp.Tests.Auto
{
    public class Generator
    {
        internal string[] AssembliesPath = new string[]
		{
			@".\QtCoreSharp.dll"
		};

        internal List<Assembly> Assemblies = new List<Assembly>();

        public void GenerateCode()
        {
            foreach (var a in Assemblies)
            {
                foreach (var c in a.GetExportedTypes())
                {
                    if (c.IsClass &&
                       c.IsPublic &&
                       !c.FullName.Contains("Private") &&
                       !c.IsNested &&
                       !c.ContainsGenericParameters &&
                       c.GetConstructor(BindingFlags.Instance |
                                        BindingFlags.Public |
                                        BindingFlags.NonPublic,
                                        null,
                                        Type.EmptyTypes,
                                        null) != null)
                    {
                        var instance = Activator.CreateInstance(c,
                                                                BindingFlags.CreateInstance |
                                                                BindingFlags.Public |
                                                                BindingFlags.Instance |
                                                                BindingFlags.OptionalParamBinding,
                                                                null,
                                                                new object[] { },
                                                                CultureInfo.CurrentCulture);

                        foreach (var prop in c.GetProperties())
                        {
                            try
                            {
                                //var val = prop.GetValue(instance);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                }
            }
        }

        public Generator()
        {
            foreach (var s in AssembliesPath)
            {
                var a = Assembly.LoadFrom(s);
                Assemblies.Add(a);
            }
        }
    }
}