# Revit + Claude integration

Lets Claude (Desktop or Code) read and edit an open Autodesk Revit model, via the
Model Context Protocol (MCP).

## Architecture

```
Claude  <--MCP (stdio)-->  revit_mcp server (Python)  <--HTTP, loopback-->  RevitMcpAddin (C#, inside Revit)
```

- **`revit-addin/`** — a Revit add-in (`IExternalApplication`) that starts a
  loopback-only HTTP server (`127.0.0.1:8787`) when Revit launches. Requests are
  handed to Revit's UI thread via `ExternalEvent`, since the Revit API can only be
  called there.
- **`mcp-server/`** — a Python MCP server that exposes Revit operations as MCP
  tools (`get_document_info`, `list_elements`, `get_selected_elements`,
  `create_level`, `create_wall`, `delete_element`). Each tool call is forwarded as
  a JSON POST to the add-in.

## Setup

### 1. Build and install the Revit add-in

Requires Windows, Revit, and the .NET SDK.

```
cd revit-addin/RevitMcpAddin
dotnet build -c Release -p:RevitVersion=2024   # match your installed Revit version
```

Copy the build output (`RevitMcpAddin.dll`) and `RevitMcpAddin.addin` into Revit's
add-ins folder, then start Revit:

```
%APPDATA%\Autodesk\Revit\Addins\2024\
```

If Revit loaded the add-in, `http://127.0.0.1:8787/` will respond to POST requests
while Revit is running.

> The add-in targets `net48` (Revit 2019-2024). For Revit 2025+, retarget
> `RevitMcpAddin.csproj` to `net8.0-windows`.

### 2. Install the MCP server

Requires Python 3.10+.

```
cd mcp-server
pip install -e .
```

### 3. Point Claude at it

Add to your Claude Desktop config (`claude_desktop_config.json`) or Claude Code
MCP config:

```json
{
  "mcpServers": {
    "revit": {
      "command": "revit-mcp"
    }
  }
}
```

Restart Claude. With Revit open and the add-in loaded, Claude can now call the
`revit` tools to inspect and modify the active document.

## Available tools

| Tool | Description |
|---|---|
| `get_document_info` | Title, path, active view, Revit version of the open document |
| `list_elements` | List elements, optionally filtered by `BuiltInCategory` name (e.g. `OST_Walls`) |
| `get_selected_elements` | Elements currently selected in the Revit UI |
| `create_level` | Create a level at a given elevation |
| `create_wall` | Create a wall between two points on a named level |
| `delete_element` | Delete an element by ID |

## Notes

- The HTTP bridge binds to loopback only (`127.0.0.1`) and is not reachable from
  outside the machine.
- Commands run one at a time; the Python server blocks per-request until Revit
  responds (30s timeout by default).
