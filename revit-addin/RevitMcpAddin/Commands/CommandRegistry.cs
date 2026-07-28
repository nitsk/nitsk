using System;
using System.Collections.Generic;

namespace RevitMcpAddin.Commands
{
    public static class CommandRegistry
    {
        private static readonly Dictionary<string, IRevitCommand> Commands = new Dictionary<string, IRevitCommand>(StringComparer.OrdinalIgnoreCase)
        {
            ["get_document_info"] = new GetDocumentInfoCommand(),
            ["list_elements"] = new ListElementsCommand(),
            ["get_selected_elements"] = new GetSelectedElementsCommand(),
            ["create_level"] = new CreateLevelCommand(),
            ["create_wall"] = new CreateWallCommand(),
            ["delete_element"] = new DeleteElementCommand(),
        };

        public static bool TryGet(string commandName, out IRevitCommand command)
        {
            return Commands.TryGetValue(commandName ?? string.Empty, out command);
        }

        public static IEnumerable<string> Names => Commands.Keys;
    }
}
