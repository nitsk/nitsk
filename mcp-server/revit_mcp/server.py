"""MCP server exposing Revit operations to Claude, via the RevitMcpAddin HTTP bridge."""

from __future__ import annotations

from mcp.server.fastmcp import FastMCP

from .client import DEFAULT_PORT, RevitClient

mcp = FastMCP("revit-mcp")
_client = RevitClient(port=DEFAULT_PORT)


@mcp.tool()
def get_document_info() -> dict:
    """Get info about the currently open Revit document (title, path, active view, version)."""
    return _client.call("get_document_info")


@mcp.tool()
def list_elements(category: str | None = None, limit: int = 100) -> dict:
    """List elements in the active Revit document.

    Args:
        category: Optional BuiltInCategory name to filter by, e.g. "OST_Walls", "OST_Doors".
        limit: Maximum number of elements to return (default 100).
    """
    return _client.call("list_elements", category=category, limit=limit)


@mcp.tool()
def get_selected_elements() -> dict:
    """Get the elements currently selected in the Revit UI."""
    return _client.call("get_selected_elements")


@mcp.tool()
def create_level(elevation: float, name: str | None = None) -> dict:
    """Create a new level in the active Revit document.

    Args:
        elevation: Elevation of the level, in feet.
        name: Optional name for the level.
    """
    return _client.call("create_level", elevation=elevation, name=name)


@mcp.tool()
def create_wall(
    x1: float,
    y1: float,
    x2: float,
    y2: float,
    level_name: str,
    height: float = 10.0,
    wall_type_name: str | None = None,
) -> dict:
    """Create a wall between two points in the active Revit document.

    Args:
        x1: Start point X coordinate, in feet.
        y1: Start point Y coordinate, in feet.
        x2: End point X coordinate, in feet.
        y2: End point Y coordinate, in feet.
        level_name: Name of the level to host the wall on.
        height: Wall height, in feet (default 10.0).
        wall_type_name: Optional wall type name; defaults to the document's default wall type.
    """
    return _client.call(
        "create_wall",
        x1=x1,
        y1=y1,
        x2=x2,
        y2=y2,
        levelName=level_name,
        height=height,
        wallTypeName=wall_type_name,
    )


@mcp.tool()
def delete_element(element_id: int) -> dict:
    """Delete an element from the active Revit document by its element ID."""
    return _client.call("delete_element", elementId=element_id)


def main() -> None:
    mcp.run()


if __name__ == "__main__":
    main()
