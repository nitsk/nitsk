# Graphify — 100% local via Ollama

Config for running [Graphify](https://github.com/Graphify-Labs/graphify)
(turns a repo/docs/PDFs/images into a queryable knowledge graph) without
sending anything to a cloud LLM.

## Why this is private

- **Code parsing is always local.** Graphify maps code with tree-sitter
  (36 language grammars) — no LLM call, no network, regardless of backend.
- **No embeddings, no vector store.** It builds an explicit node/edge graph,
  not a RAG index, so there's no third-party embeddings API involved either.
- **The only network calls are LLM calls**, and only for semantic extraction
  of non-code files (docs/PDFs/images) and optional community naming. This
  setup points those calls at a local Ollama server instead of a cloud
  provider.
- **No telemetry.**

## Setup

On the machine you'll run graphify from (not a sandbox — it needs to reach
`ollama.com` once, to install Ollama and pull the model):

```bash
./setup.sh
```

This installs Ollama, pulls `qwen2.5-coder:7b` (override with
`OLLAMA_MODEL=... ./setup.sh`), installs `graphifyy[ollama]`, writes
`graphify.env`, and registers the graphify skill for Claude Code.

## Using it

```bash
source graphify/graphify.env
graphify extract <path> --backend ollama
```

Code-only extraction never needs an LLM at all:

```bash
graphify extract <path> --code-only
```

## Staying 100% local

Graphify auto-detects a backend by checking for cloud API keys first
(Gemini → Kimi → Claude → OpenAI → DeepSeek → Azure → Bedrock) and only
falls back to Ollama last, so a paid key already in your shell silently wins
over `OLLAMA_BASE_URL`. To guarantee every run stays local:

- always pass `--backend ollama` explicitly, **or**
- make sure `ANTHROPIC_API_KEY`, `GEMINI_API_KEY`, `GOOGLE_API_KEY`,
  `MOONSHOT_API_KEY`, `OPENAI_API_KEY`, `DEEPSEEK_API_KEY`, and
  `AZURE_OPENAI_API_KEY` are unset in the environment you run it from.

`graphify.env.example` documents each variable; `setup.sh` copies it to
`graphify.env` (gitignored, not committed) with your chosen model filled in.
