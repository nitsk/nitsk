"""Thin HTTP client for the Revit add-in's local command bridge."""

from __future__ import annotations

from typing import Any

import httpx

DEFAULT_PORT = 8787
DEFAULT_TIMEOUT = 35.0


class RevitCommandError(RuntimeError):
    """Raised when the Revit add-in reports a command failure."""


class RevitClient:
    def __init__(self, port: int = DEFAULT_PORT, timeout: float = DEFAULT_TIMEOUT) -> None:
        self._url = f"http://127.0.0.1:{port}/"
        self._timeout = timeout

    def call(self, command: str, **params: Any) -> Any:
        try:
            response = httpx.post(self._url, json={"command": command, "params": params}, timeout=self._timeout)
        except httpx.ConnectError as exc:
            raise RevitCommandError(
                "Could not reach the Revit add-in. Is Revit running with the "
                "RevitMcpAddin add-in loaded?"
            ) from exc

        body = response.json()
        if not body.get("success"):
            raise RevitCommandError(body.get("error") or "Revit add-in reported an unknown error.")
        return body.get("result")
