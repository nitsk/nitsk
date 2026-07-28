using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;

namespace RevitMcpAddin.Commands
{
    public class GetSelectedElementsCommand : IRevitCommand
    {
        public object Execute(UIApplication uiApplication, Dictionary<string, object> parameters)
        {
            var uiDoc = uiApplication.ActiveUIDocument;
            if (uiDoc == null)
                return new { count = 0, elements = new object[0] };

            var doc = uiDoc.Document;
            var elements = uiDoc.Selection.GetElementIds()
                .Select(id => doc.GetElement(id))
                .Where(e => e != null)
                .Select(e => new
                {
                    id = e.Id.IntegerValue,
                    name = e.Name,
                    category = e.Category?.Name,
                })
                .ToList();

            return new { count = elements.Count, elements };
        }
    }
}
