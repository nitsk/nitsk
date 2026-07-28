# nitsk

## AutoCAD MCP server

This repo wires up [thepiruthvirajan/autocad-mcp-server](https://github.com/thepiruthvirajan/autocad-mcp-server) as a Claude Code MCP server via `.mcp.json`. The server itself is **not vendored here** — it must be installed on a Windows machine with AutoCAD, since it drives AutoCAD through COM automation (`pywin32`) and only runs on Windows.

### Prerequisites

- Windows with AutoCAD (2000+) installed and running
- Python 3.8+

### Install the server

```bash
git clone https://github.com/thepiruthvirajan/autocad-mcp-server.git
cd autocad-mcp-server
pip install -e .
```

This registers the `autocad-com-mcp` console command on your `PATH` (via the `[project.scripts]` entry in the server's `pyproject.toml`).

### Use it with Claude Code

`.mcp.json` in this repo already points at that command:

```json
{
  "mcpServers": {
    "autocad-mcp": {
      "command": "autocad-com-mcp",
      "args": []
    }
  }
}
```

Open this project in Claude Code on the same Windows machine (with AutoCAD running) and approve the `autocad-mcp` server when prompted — no extra configuration needed as long as `autocad-com-mcp` is on `PATH`.
