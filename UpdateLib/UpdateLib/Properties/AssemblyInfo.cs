using System.Reflection;
using System.Runtime.CompilerServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("UpdateLib")]
[assembly: AssemblyDescription("Auto Update Library")]

#if DEBUG
// Make sure the internal methods are available for unit testing
[assembly: InternalsVisibleTo("UpdateLib.Tests")]

[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif
