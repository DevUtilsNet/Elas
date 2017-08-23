using System.Resources;
using System.Reflection;

[assembly: AssemblyCompany("DevUtils.Net")]
[assembly: AssemblyProduct("ELAS")]
[assembly: AssemblyCopyright("Copyright © 2016 DevUtils.Net. All rights reserved.")]
[assembly: AssemblyTrademark("DevUtils.Net©")]
[assembly: AssemblyInformationalVersion("Manual Build")]
[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

#if !DEBUG
[assembly: AssemblyConfiguration("Release")]
#else
[assembly: AssemblyConfiguration("Debug")]
#endif

[assembly: AssemblyVersion("1.0.11.0")]
[assembly: AssemblyFileVersion("1.0.11.0")]