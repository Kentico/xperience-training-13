using System.Reflection;

using CMS.Base;
using CMS.DataEngine;

/// <summary>
/// Application methods.
/// </summary>
public class Global : CMSHttpApplication
{
    static Global()
    {
#if DEBUG
        // Set debug mode based on current web project build configuration
        SystemContext.IsWebProjectDebug = true;
#endif
        // Initialize CMS application. This method should not be called from custom code.
        InitApplication(Assembly.GetExecutingAssembly());
    }
}