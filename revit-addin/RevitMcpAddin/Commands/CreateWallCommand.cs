using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMcpAddin.Commands
{
    /// <summary>
    /// Params: x1, y1, x2, y2 (double, feet), levelName (string), height (double, feet, optional),
    /// wallTypeName (string, optional - defaults to the document's default wall type).
    /// </summary>
    public class CreateWallCommand : IRevitCommand
    {
        public object Execute(UIApplication uiApplication, Dictionary<string, object> parameters)
        {
            var doc = uiApplication.ActiveUIDocument?.Document;
            if (doc == null)
                throw new InvalidOperationException("No active Revit document.");

            foreach (var required in new[] { "x1", "y1", "x2", "y2", "levelName" })
            {
                if (!parameters.Require(required))
                    throw new ArgumentException($"Missing required parameter '{required}'.");
            }

            var start = new XYZ(parameters.GetDouble("x1"), parameters.GetDouble("y1"), 0);
            var end = new XYZ(parameters.GetDouble("x2"), parameters.GetDouble("y2"), 0);
            var levelName = parameters.GetString("levelName");
            var height = parameters.GetDouble("height", 10.0);
            var wallTypeName = parameters.GetString("wallTypeName");

            var level = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .FirstOrDefault(l => l.Name.Equals(levelName, StringComparison.OrdinalIgnoreCase));
            if (level == null)
                throw new ArgumentException($"Level '{levelName}' was not found.");

            WallType wallType = null;
            if (!string.IsNullOrEmpty(wallTypeName))
            {
                wallType = new FilteredElementCollector(doc)
                    .OfClass(typeof(WallType))
                    .Cast<WallType>()
                    .FirstOrDefault(w => w.Name.Equals(wallTypeName, StringComparison.OrdinalIgnoreCase));
                if (wallType == null)
                    throw new ArgumentException($"Wall type '{wallTypeName}' was not found.");
            }

            Wall wall;
            using (var tx = new Transaction(doc, "MCP: Create Wall"))
            {
                tx.Start();
                var line = Line.CreateBound(start, end);
                wall = Wall.Create(doc, line, level.Id, structural: false);
                if (wallType != null)
                    wall.WallType = wallType;
                wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM)?.Set(height);
                tx.Commit();
            }

            return new { id = wall.Id.IntegerValue, level = level.Name, wallType = wall.WallType?.Name };
        }
    }
}
