using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMcpAddin.Commands
{
    /// <summary>
    /// Params: elementId (int).
    /// </summary>
    public class DeleteElementCommand : IRevitCommand
    {
        public object Execute(UIApplication uiApplication, Dictionary<string, object> parameters)
        {
            var doc = uiApplication.ActiveUIDocument?.Document;
            if (doc == null)
                throw new InvalidOperationException("No active Revit document.");

            if (!parameters.Require("elementId"))
                throw new ArgumentException("Missing required parameter 'elementId'.");

            var elementId = new ElementId(parameters.GetInt("elementId"));
            if (doc.GetElement(elementId) == null)
                throw new ArgumentException($"Element {elementId.IntegerValue} was not found.");

            using (var tx = new Transaction(doc, "MCP: Delete Element"))
            {
                tx.Start();
                doc.Delete(elementId);
                tx.Commit();
            }

            return new { deleted = true, id = elementId.IntegerValue };
        }
    }
}
