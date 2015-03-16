using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

#if ANDROID
using Android.App;

#if DEBUG || TESTFLIGHT
[assembly: Application(Debuggable=true)]
#elif RELEASE || MARKETPLACE
[assembly: Application(Debuggable=false)]
#endif

#endif

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("B. A. Bellhop")]
[assembly: AssemblyProduct( "B. A. Bellhop" )]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Munomic, LLC.")]
[assembly: AssemblyCopyright("Copyright © Munomic 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type. Only Windows
// assemblies support COM.
[assembly: ComVisible(false)]

// On Windows, the following GUID is for the ID of the typelib if this
// project is exposed to COM. On other platforms, it unique identifies the
// title storage container when deploying this assembly to the device.
[assembly: Guid("a99a8e4b-8b27-485b-830e-a716c0d64e8c")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0")]
[assembly: NeutralResourcesLanguageAttribute("en-US")]