using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMcpAddin.Commands
{
    /// <summary>
    /// Params: name (string), elevation (double, feet).
    /// </summary>
    public class CreateLevelCommand : IRevitCommand
    {
        public object Execute(UIApplication uiApplication, Dictionary<string, object> parameters)
        {
            var doc = uiApplication.ActiveUIDocument?.Document;
            if (doc == null)
                throw new InvalidOperationException("No active Revit document.");

            if (!parameters.Require("elevation"))
                throw new ArgumentException("Missing required parameter 'elevation'.");

            var elevation = parameters.GetDouble("elevation");
            var name = parameters.GetString("name");

            Level level;
            using (var tx = new Transaction(doc, "MCP: Create Level"))
            {
                tx.Start();
                level = Level.Create(doc, elevation);
                if (!string.IsNullOrEmpty(name))
                    level.Name = name;
                tx.Commit();
            }

            return new { id = level.Id.IntegerValue, name = level.Name, elevation = level.Elevation };
        }
    }
}
