using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMcpAddin.Commands
{
    /// <summary>
    /// Lists elements in the active document, optionally filtered by built-in category name
    /// (e.g. "OST_Walls"). Results are capped by "limit" (default 100) to keep responses small.
    /// </summary>
    public class ListElementsCommand : IRevitCommand
    {
        public object Execute(UIApplication uiApplication, Dictionary<string, object> parameters)
        {
            var doc = uiApplication.ActiveUIDocument?.Document;
            if (doc == null)
                throw new InvalidOperationException("No active Revit document.");

            var categoryName = parameters.GetString("category");
            var limit = parameters.GetInt("limit", 100);

            var collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();

            if (!string.IsNullOrEmpty(categoryName))
            {
                if (!Enum.TryParse(categoryName, out BuiltInCategory builtInCategory))
                    throw new ArgumentException($"Unknown category '{categoryName}'. Expected a BuiltInCategory name such as OST_Walls.");
                collector = collector.OfCategory(builtInCategory);
            }

            var elements = collector
                .Take(limit)
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
