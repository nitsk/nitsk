using System.Collections.Generic;
using Autodesk.Revit.UI;

namespace RevitMcpAddin.Commands
{
    /// <summary>
    /// A single Revit-side operation exposed over the HTTP bridge. Implementations run on
    /// Revit's UI thread (invoked via ExternalEvent), so they may call the Revit API directly.
    /// </summary>
    public interface IRevitCommand
    {
        object Execute(UIApplication uiApplication, Dictionary<string, object> parameters);
    }
}
