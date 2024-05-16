using MediaPortal.Common.Utils;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "MP-TVSeries" )]
[assembly: AssemblyDescription( "MP-TVSeries plugin for MediaPortal" )]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration( "Release" )]
#endif

[assembly: AssemblyCompany( "" )]
[assembly: AssemblyProduct( "MP-TVSeries" )]
[assembly: AssemblyCopyright( "Copyright © 2020-2024" )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "3d458ab7-d9b7-4cd2-a3f9-d679eba55abb" )]

// Version Compatibility
// http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/18_Contribute/6_Plugins/Plugin_Related_Changes/1.6.0_to_1.7.0
[assembly: CompatibleVersion( "1.6.100.0" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

// 1st/2nd Digit reprsents the Major Version, this should increment whenever any new enhancements have been made. 3rd Number should be reset.
// 3rd Digit represents the Maintenance Number (Hot Fix), this should increment by one each update release
// 4th Digit represents the build/patch revision
[assembly: AssemblyVersion( "4.5.0.999" )]
[assembly: AssemblyFileVersion( "4.5.0.999" )]
