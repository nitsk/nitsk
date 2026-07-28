# Connect Claude Desktop to FreeCAD (Windows)

Wires Claude Desktop up to FreeCAD 1.1 (`C:\Program Files\FreeCAD 1.1\bin\freecad.exe`)
through the [freecad-mcp](https://github.com/neka-nat/freecad-mcp) add-on by neka-nat.
Once connected, Claude can write and run FreeCAD Python commands directly, see a
screenshot of the result, and iterate — so you can build parts by describing them
in plain language.

Source guide: MakeForm's *Connect Claude to FreeCAD* setup guide, cross-checked
against neka-nat/freecad-mcp.

## What you need

- Windows 64-bit, Administrator rights
- Claude Desktop (any plan with Desktop access) — https://claude.com/download
- FreeCAD 1.1 — already installed at `C:\Program Files\FreeCAD 1.1\bin\freecad.exe`

## Quick start

1. Install Claude Desktop and sign in, if you haven't already.
2. Open **PowerShell as Administrator**.
3. Run the setup script from this folder:

   ```powershell
   cd path\to\freecad-claude-integration
   .\setup-freecad-claude.ps1
   ```

   This installs `uv`/`uvx` (Python tool runner) if missing, downloads the
   `freecad-mcp` add-on into `%APPDATA%\FreeCAD\v1-1\Mod`, and adds a `freecad`
   entry to `claude_desktop_config.json` (merging with any MCP servers you
   already have configured; a `.bak` backup is made first).

   - If `uvx` had to be installed for the first time, the script will tell you
     to close PowerShell, reopen it as Administrator, and run the script again
     (PATH changes only apply to a fresh shell).
   - Want the token-saving, text-only mode (no viewport screenshots)?
     Run `.\setup-freecad-claude.ps1 -TextOnly` instead.

4. Finish the manual steps the script can't do for you (GUI actions):
   1. Quit Claude Desktop **from the system tray**, not just the window —
      closing the window leaves it running in the background and the config
      change won't take effect.
   2. Open FreeCAD → workbench dropdown → select the **MCP add-on**.
   3. Click **Start RPC Server** in the new toolbar (allow it through the
      Windows firewall prompt if asked). Tick **Autostart Server** so you
      don't have to do this every session.
   4. Reopen Claude Desktop → **+** → **Connectors** → confirm `freecad` is
      listed and toggled on. Double-check under **Settings (Ctrl+,) →
      Developer → Local MCP Servers** that it shows as added and running.

You're connected once FreeCAD shows in Claude's connector list as active.

## Prompt library (test it)

Paste these into Claude once connected. First use of each tool asks for
permission — click **Always Allow** so you're not reapproving every time.

| Goal | Prompt |
|---|---|
| New document | `Create a new document in FreeCAD called 'Test Part'` |
| Simple box | `Create a box in FreeCAD with length 50mm, width 30mm, height 20mm` |
| Fillet (vague on purpose) | `Fillet all edges` |
| Holes (works out placement itself) | `Make 4 holes on top` |
| Fully specified flange | `Design a flange in FreeCAD with a base diameter of 100mm, thickness 10mm, and a center hole of 20mm diameter, with 4 bolt holes of 8mm diameter equally spaced at 70mm PCD` |
| From a sketch | Upload a photo of a hand drawing (front/top/right views, dimensions marked), then: `Create this part in FreeCAD` |
| Real math (involute gear) | `Design a spur gear in FreeCAD with 20 teeth, module 2, face width 20mm, and a center bore of 10mm diameter` |
| Export | `Export the part as a 3mf file` |

Use **Tools → Measure** in FreeCAD to verify dimensions yourself. Everything
stays parametric — fillet radius, hole diameter, hole position — all still
editable later just by prompting.

## Important: save your work

Claude can close open documents without warning (e.g. if you ask it to
"close all the documents") — no confirmation, no save prompt. It can
recreate parts if asked, but that means redoing (and re-spending tokens on)
the work.

- **Habit 1:** Hit `Ctrl+S` in FreeCAD after each part you're happy with.
- **Habit 2:** Prompt `Save all open documents` before closing anything or
  ending a session.

## Troubleshooting

| Problem | Fix |
|---|---|
| `uvx --version` errors | PowerShell wasn't restarted after install. Close it fully, reopen as Administrator. |
| No MCP add-on in FreeCAD's workbench dropdown | FreeCAD was open during install — close fully and reopen. Confirm the add-on landed at `%APPDATA%\FreeCAD\v1-1\Mod\freecad-mcp`. |
| FreeCAD connector doesn't appear in Claude | Claude wasn't fully closed — quit from the system tray, not just the window, then reopen. |
| Claude won't start / config seems ignored | JSON syntax error in `claude_desktop_config.json`. Restore from the `.bak` file the script created and re-run, or validate the JSON. |
| "Cannot connect to FreeCAD" | The RPC server isn't running — FreeCAD → MCP workbench → **Start RPC Server**. Tick **Autostart Server**. |
| Firewall prompt was denied | Windows Security → Firewall → Allow an app → find FreeCAD → enable it. |
| Burning through tokens too fast | Re-run the script with `-TextOnly`, or edit the `args` in `claude_desktop_config.json` to add `--only-text-feedback`. |
| Fillet fails with a geometry error | Radius too large for the geometry — keep it well under the smallest part dimension. |

## Files here

- `setup-freecad-claude.ps1` — automates the installable/config steps above.
- `README.md` — this file.

## Credits

FreeCAD MCP add-on by neka-nat: https://github.com/neka-nat/freecad-mcp
