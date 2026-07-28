using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMcpAddin.Commands
{
    public class GetDocumentInfoCommand : IRevitCommand
    {
        public object Execute(UIApplication uiApplication, Dictionary<string, object> parameters)
        {
            var doc = uiApplication.ActiveUIDocument?.Document;
            if (doc == null)
                return new { open = false };

            return new
            {
                open = true,
                title = doc.Title,
                pathName = doc.PathName,
                isFamilyDocument = doc.IsFamilyDocument,
                activeViewName = uiApplication.ActiveUIDocument.ActiveView?.Name,
                revitVersion = uiApplication.Application.VersionNumber,
            };
        }
    }
}
